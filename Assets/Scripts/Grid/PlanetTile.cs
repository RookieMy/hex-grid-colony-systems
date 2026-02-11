using UnityEngine;


public enum TileType
{
    Rock,
    Dust,
    Ice
}

public class PlanetTile
{
    public int x;
    public int y;

    public TileType tileType;

    public bool hasOre;
    public bool hasIce;

    public int oreAmount;
    public int iceAmount;

    public GameObject oreVisual;
    public GameObject iceVisual;

    public bool isDiscovered;

    public bool isOccupied;
    public GameObject placedBuilding;

    private Grid<PlanetTile> grid;

    public PlanetTile(Grid<PlanetTile> grid , int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;

        tileType = TileType.Rock;
        hasOre = false;
        hasIce = false;
        isDiscovered = false;

        isOccupied = false;
        placedBuilding = null;
    }

    public override string ToString()
    {
        return $"{x},{y}";
    }
}
