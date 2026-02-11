using System.Collections.Generic;
using UnityEngine;

public class FOWManager : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager;
    public GameObject fogTilePrefab;

    [Header("FOW Settings")]
    public float revealRadius = 3f;
    public bool startRevealed = false;

    private HexGrid<PlanetTile> grid;
    private Dictionary<PlanetTile,GameObject> fogObjects= new Dictionary<PlanetTile, GameObject>();

    public void CreateFog()
    {
        if(startRevealed) return;
        grid = gridManager.grid;
        Quaternion quaternion = grid.isPointyTopped ? Quaternion.Euler(0, 30, 0) : Quaternion.identity;
        for (int x=0;x<gridManager.width;x++)
        {
            for(int y=0;y<gridManager.height;y++)
            {
                PlanetTile tile = grid.GetGridObject(x, y);
                Vector3 worldPos = grid.GetWorldPosition(x, y);

                GameObject fog = Instantiate(fogTilePrefab, worldPos + Vector3.up * 1f, quaternion, transform);
                fogObjects[tile] = fog;
            }
        }
    }

    public void RevealAround(Vector3 worldPosition)
    {
        for(int x=0;x<gridManager.width;x++)
        {
            for(int y=0;y<gridManager.height;y++)
            {
                PlanetTile tile = grid.GetGridObject(x, y);
                Vector3 tilePos = grid.GetWorldPosition(x, y);

                float dist = Vector3.Distance(new Vector3(worldPosition.x,0,worldPosition.z), new Vector3(tilePos.x,0,tilePos.z));

                if(dist <= revealRadius * gridManager.cellSize)
                {
                    tile.isDiscovered = true;
                    if (fogObjects.TryGetValue(tile, out GameObject fogObj))
                    {
                        Destroy(fogObj);
                        fogObjects.Remove(tile);
                    }
                }
            }
        }
    }

}
