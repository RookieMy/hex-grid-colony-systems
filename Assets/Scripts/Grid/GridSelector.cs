using UnityEngine;

public class GridSelector : MonoBehaviour
{
    private HexGrid<PlanetTile> grid;
    public Transform hoverIndicator;
    public int currentX = -1;
    public int currentY = -1;
    public PlanetTile currentTile;


    public void SetGrid(HexGrid<PlanetTile> grid)
    {
        this.grid = grid;
    }

    private void Update()
    {
        Vector3 worldPos = Utils.GetMouseWorldPositionIzometrical();
        grid.GetXY(worldPos, out int x, out int y);

        var node = grid.GetGridObject(x, y);
        if (node != null)
        {
            currentX = x;
            currentY = y;
            currentTile = node;

            Vector3 center = grid.GetWorldPosition(x, y);
            hoverIndicator.gameObject.SetActive(true);
            hoverIndicator.position = center + Vector3.up * 0.05f;

        }
        else
        {
            currentX = -1;
            currentY = -1;
            currentTile = null;

            hoverIndicator.gameObject.SetActive(false);
        }

    }
}
