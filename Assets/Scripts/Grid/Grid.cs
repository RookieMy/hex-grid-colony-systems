using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Grid<TGridObject> {

    public const int HEAT_MAP_MAX_VALUE = 100;
    public const int HEAT_MAP_MIN_VALUE = 0;

    public event EventHandler<OnGridValueChangedEventsArgs> OnGridValueChanged;
    public class OnGridValueChangedEventsArgs : EventArgs
    {
        public int x;
        public int y;
    }

    protected int width;
    protected int height;
    protected float cellSize;
    protected Vector3 originPosition;

    protected TGridObject[,] gridArray;

    // Debug
    protected bool showDebug;
    protected TextMesh[,] debugTextArray;

    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject, bool showDebug = false)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        this.showDebug = showDebug;

        gridArray = new TGridObject[width, height];
        
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                gridArray[x, y] = createGridObject(this,x,y);
            }
        }

        
        if (showDebug)
        {
           InitializeDebugVisuals();
        }

        OnGridValueChanged += (object sender, OnGridValueChangedEventsArgs eventArgs) =>
        {
            
            if (showDebug) debugTextArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y]?.ToString();
            RefreshDebug();
        };
    }

    public virtual Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, 0, y) * cellSize + originPosition;
    }


    public virtual void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x= Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y= Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
    }

    public void SetGridObject(int x, int y, TGridObject value)
    {
        if (x < 0 || y < 0 || x >= width || y >= height) return;
        gridArray[x, y] = value;
        if (showDebug) debugTextArray[x, y].text = gridArray[x, y].ToString();
        if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventsArgs { x = x, y = y });
    }

    public void TriggerGridObjectChanged(int x, int y)
    {
        if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventsArgs { x = x, y = y });
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetGridObject(x, y, value);
    }

    public TGridObject GetGridObject(int x, int y)
    {
        return (!IsInBounds(x,y)? default(TGridObject) : gridArray[x, y]);
    }

    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }

    public int GetWidth()=> width;
    public int GetHeight()=> height;
    public float GetCellSize()=> cellSize;


    protected bool IsInBounds(int x, int y) => x >= 0 && y >= 0 && x < width && y < height;

    protected virtual void InitializeDebugVisuals()
    {
        debugTextArray = new TextMesh[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 worldPos = GetWorldPosition(x, y);
                debugTextArray[x, y] = Utils.CreateWorldText(
                    gridArray[x, y]?.ToString(),
                    null,
                    worldPos + new Vector3(cellSize, 0, cellSize) * 0.5f,
                    20,
                    Color.white,
                    TextAnchor.MiddleCenter
                );
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
            }
        }
        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.gray, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.gray, 100f);
    }

    protected void RefreshDebug()
    {
        if (!showDebug || debugTextArray == null) return;
        
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                debugTextArray[x, y].text = gridArray[x, y]?.ToString();
            }
        }
    }

    public GameObject GetWorld() => GameObject.Find("Grid World");

}
