using UnityEngine;
using System;

public class HexGrid<TGridObject> : Grid<TGridObject>
{
    private float hexWidth;
    private float hexHeight;
    public bool isPointyTopped { get; private set; }

    public HexGrid(
        int width,
        int height,
        float cellSize,
        Vector3 originPosition,
        Func<Grid<TGridObject>, int, int, TGridObject> createGridObject,
        bool isPointyTopped = true,
        bool showDebug = false)
        : base(width, height, cellSize, originPosition, createGridObject)
    {
        this.isPointyTopped = isPointyTopped;
        this.showDebug = showDebug;
        if (isPointyTopped)
        {
            hexHeight = cellSize;
            hexWidth = Mathf.Sqrt(3) / 2 * hexHeight; // Width for pointy-topped hex
        }
        else
        {
            hexWidth = cellSize;
            hexHeight = Mathf.Sqrt(3) / 2 * hexWidth; // Height for flat-topped hex
        }

        if(this.showDebug)InitializeDebugVisuals();
    }

    protected override void InitializeDebugVisuals()
    {
        debugTextArray = new TextMesh[width, height];
        for (int x = 0; x < width; x++)
        {
            for(int y=0;y<height;y++)
            {
                Vector3 center = GetWorldPosition(x, y);

                Vector3[] corners = GetHexCorners(center, cellSize,isPointyTopped);

                for(int i = 0; i < 6; i++)
                {
                    Vector3 p1 = corners[i];
                    Vector3 p2 = corners[(i + 1) % 6];
                    Debug.DrawLine(p1, p2, Color.white, 100f);
                }

                

                debugTextArray[x, y] = Utils.CreateWorldText(
                    gridArray[x, y]?.ToString(),
                    null,
                    GetWorldPosition(x, y) + new Vector3(0, 2, 0),
                    20,
                    Color.white,
                    TextAnchor.MiddleCenter
                );
            }
        }
    }

    private Vector3[] GetHexCorners(Vector3 center, float size, bool pointy)
    {
        Vector3[] corners = new Vector3[6];
        float startAngle = pointy ? 30f : 0f;

        for (int i = 0; i < 6; i++)
        {
            float angleDeg = startAngle + 60f * i;
            float angleRad = Mathf.Deg2Rad * angleDeg;
            corners[i] = new Vector3(
                center.x + size * Mathf.Cos(angleRad),
                center.y,
                center.z + size * Mathf.Sin(angleRad)
            );
        }

        return corners;
    }


    public override Vector3 GetWorldPosition(int x, int y)
    {
        if (isPointyTopped)
        {
            float xPos=GetCellSize() * (Mathf.Sqrt(3) * x + Mathf.Sqrt(3)/2 * (y % 2));
            float yPos=GetCellSize() * (3f/2 * y);
            return new Vector3(xPos, 0, yPos) + GetOriginPosition();
        }
        else
        {
            float xPos = GetCellSize() * 1.5f * x;
            float zPos = GetCellSize() * Mathf.Sqrt(3f) * (y + 0.5f * (x % 2));
            return new Vector3(xPos, 0, zPos) + GetOriginPosition();
        }
    }

    public override void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        Vector3 pos = worldPosition - originPosition;

        if (isPointyTopped)
        {
            // ------------------------
            // POINTY-TOP HEX FORMÜLÜ
            // ------------------------
            float q = (Mathf.Sqrt(3f) / 3f * pos.x - 1f / 3f * pos.z) / GetCellSize();
            float r = (2f / 3f * pos.z) / GetCellSize();

            Vector3 cube = CubeRound(q, -q - r, r);
            int qR = Mathf.RoundToInt(cube.x);
            int rR = Mathf.RoundToInt(cube.z);

            // Odd-r offset mapping
            x = qR + (rR - (rR & 1)) / 2;
            y = rR;
        }
        else
        {
            // ------------------------
            // FLAT-TOP HEX FORMÜLÜ
            // ------------------------
            float q = (2f / 3f * pos.x) / GetCellSize();
            float r = (-1f / 3f * pos.x + Mathf.Sqrt(3f) / 3f * pos.z) / GetCellSize();

            Vector3 cube = CubeRound(q, -q - r, r);
            int qR = Mathf.RoundToInt(cube.x);
            int rR = Mathf.RoundToInt(cube.z);

            // Odd-q offset mapping
            x = qR;
            y = rR + (qR - (qR & 1)) / 2;
        }
    }

    private Vector3 CubeRound(float x, float y, float z)
    {
        float rx = Mathf.Round(x);
        float ry = Mathf.Round(y);
        float rz = Mathf.Round(z);

        float xDiff = Mathf.Abs(rx - x);
        float yDiff = Mathf.Abs(ry - y);
        float zDiff = Mathf.Abs(rz - z);

        if (xDiff > yDiff && xDiff > zDiff) rx = -ry - rz;
        else if (yDiff > zDiff) ry = -rx - rz;
        else rz = -rx - ry;

        return new Vector3(rx, ry, rz);
    }


    private Vector3 GetOriginPosition()
    {
        return Vector3.zero;
    }
}
