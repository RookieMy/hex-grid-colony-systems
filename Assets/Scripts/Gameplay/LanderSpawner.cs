using UnityEngine;

public class LanderSpawner : MonoBehaviour
{
    public GridManager gridManager;
    public FOWManager fowManager;

    public GameObject landerPrefab;

    [Header("Spawn Settings")]
    public bool tryFindResourceRichArea = true;
    public int searchRadius = 5;
    public int searchIndexRadius = 3;

    public void StartToSpawn()
    {
        Vector2Int spawnCoords = FindSpawnCoords();
        Vector3 spawnPos =gridManager.grid.GetWorldPosition(spawnCoords.x, spawnCoords.y);

        GameObject lander = Instantiate(landerPrefab, spawnPos + Vector3.up * .13f, Quaternion.identity);

        PlanetTile tile =gridManager.grid.GetGridObject(spawnCoords.x, spawnCoords.y);
        
        if(tile != null)
        {
            tile.isOccupied = true;
            tile.placedBuilding = lander;
        }

        if (fowManager != null)
        {
            fowManager.RevealAround(spawnPos);
        }

        gridManager.RecalculateEnergy();
    }

    private Vector2Int FindSpawnCoords()
    {
        int centerX = gridManager.width / 2;
        int centerY = gridManager.height / 2;

        if(!tryFindResourceRichArea)
            return new Vector2Int(centerX, centerY);

        int bestX = centerX;
        int bestY = centerY;
        int bestScore = -1;

        //Search in a diamond shape around the center for the tile with the most resources in its vicinity, prioritizing closer tiles in case of ties
        for (int x=centerX-searchRadius; x<=centerX+searchRadius;x++)
        {
            for(int y = centerY - searchRadius; y <= centerY + searchRadius; y++)
            {
                if (x < 0 || y < 0 || x >= gridManager.width || y >= gridManager.height)
                    continue;

                int score = 0;
                PlanetTile tile = gridManager.grid.GetGridObject(x, y);
                for(int i=centerX-searchIndexRadius;i<=centerX+searchIndexRadius;i++)
                {
                    for (int j = centerY - searchIndexRadius - Mathf.Abs(i - centerX); j <= centerY + searchIndexRadius - Mathf.Abs(i - centerX); j++)
                    {
                        if (i < 0 || j < 0 || i >= gridManager.width || j >= gridManager.height)
                            continue;

                        PlanetTile nearbyTile=gridManager.grid.GetGridObject(i, j);
                        if (nearbyTile != null)
                        {
                            if (nearbyTile.hasOre) score++;
                            if (nearbyTile.hasIce) score++;
                        }
                    }
                }

                if (score > bestScore)
                {
                    bestScore = score;
                    bestX = x;
                    bestY = y;
                }

            }
        }

        return new Vector2Int(bestX, bestY);
    }
}
