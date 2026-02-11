using UnityEngine;

public class PlanetVisualizer : MonoBehaviour
{
    public GridManager gridManager;

    [Header("Prefabs")]
    public GameObject rockTilePrefab;
    public GameObject iceTilePrefab;
    public GameObject dustTilePrefab;

    public GameObject oreNodePrefab;
    public GameObject iceNodePrefab;

    public void DrawPlanet()
    {
        var grid= gridManager.grid;
        Quaternion quaternion = grid.isPointyTopped ? Quaternion.Euler(0, 30, 0) : Quaternion.identity;
        for (int x = 0; x < gridManager.width; x++)
        {
            for (int y = 0; y < gridManager.height; y++)
            {
                PlanetTile tile = grid.GetGridObject(x, y);
                Vector3 worldPos = grid.GetWorldPosition(x, y);

                GameObject tilePrefab = GetTilePrefab(tile.tileType);
                if (tilePrefab != null)
                {
                    Instantiate(tilePrefab, worldPos, quaternion, transform);
                }

                if (tile.hasOre && oreNodePrefab != null)
                {
                    Instantiate(oreNodePrefab, worldPos + Vector3.up * 0.1f, quaternion, transform);
                }

                if (tile.hasIce && iceNodePrefab != null)
                {
                    Instantiate(iceNodePrefab, worldPos + Vector3.up * 0.1f, quaternion, transform);
                }
            }
        }
    }
    private GameObject GetTilePrefab(TileType type)
    {
        switch (type)
        {
            case TileType.Rock: return rockTilePrefab;
            case TileType.Dust: return dustTilePrefab;
            case TileType.Ice: return iceTilePrefab;
        }
        return null;
    }
}
