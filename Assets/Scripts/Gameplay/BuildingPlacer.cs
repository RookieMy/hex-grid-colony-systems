using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager;
    public GridSelector gridSelector;    // hover için

    [Header("Bina Prefabları")]
    public GameObject landerPrefab;
    public GameObject solarPanelPrefab;
    public GameObject batteryPrefab;
    public GameObject storagePrefab;
    public GameObject refineryPrefab;
    public GameObject extractorPrefab;

    [Header("Ghost Settings")]
    public Material ghostValidMaterial;
    public Material ghostInvalidMaterial;

    private GameObject ghostInstance;
    private BuildingType ghostType = BuildingType.None;


    [Header("Durum")]
    public BuildingType activeBuildingType = BuildingType.None;

    private HexGrid<PlanetTile> grid;

    [Header("Test")]
    public bool hotkeysEnabled = false;

    public void SetGrid(HexGrid<PlanetTile> grid)
    {
        this.grid = grid;
    }

    private void Update()
    {
        HandleHotkeys();
        UpdateGhostVisual();
        HandlePlacementClick();
    }

    private void HandleHotkeys()
    {
        if(!hotkeysEnabled) return;
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetActiveBuildingType(BuildingType.Lander);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetActiveBuildingType(BuildingType.SolarPanel);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetActiveBuildingType(BuildingType.Battery);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetActiveBuildingType(BuildingType.Storage);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SetActiveBuildingType(BuildingType.Refinery);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SetActiveBuildingType(BuildingType.Extractor);

        // Sağ tık → iptal
        if (Input.GetMouseButtonDown(1))
        {
            SetActiveBuildingType(BuildingType.None);
        }
    }

    private void HandlePlacementClick()
    {
        if (activeBuildingType == BuildingType.None)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPos = Utils.GetMouseWorldPositionIzometrical();
            grid.GetXY(worldPos, out int x, out int y);

            PlanetTile tile = grid.GetGridObject(x, y);
            if (tile == null)
                return;

            if (!CanPlaceOnTile(tile))
            {
                Debug.Log("Cannot place building here.");
                return;
            }

            GameObject prefab = GetPrefabForType(activeBuildingType);
            if (prefab == null)
            {
                Debug.LogWarning($"No prefab assigned for {activeBuildingType}");
                return;
            }

            if(prefab.GetComponent<Animator>() != null )
                prefab.GetComponent<Animator>().enabled = true;

            Vector3 center = grid.GetWorldPosition(x, y);
            GameObject buildingInstance = Instantiate(
                prefab,
                center + Vector3.up * 0.2f,
                Quaternion.identity
            );

            tile.isOccupied = true;
            tile.placedBuilding = buildingInstance;

            BuildingConfig config = buildingInstance.GetComponent<BuildingConfig>();
            if (config != null)
            {
                gridManager.resources.Spend(
                    config.costOre,
                    config.costMetal,
                    config.costIce,
                    config.costWater
                );
            }

            gridManager.RecalculateStorageCapacities();
            gridManager.RecalculateEnergy();
        }
    }

    private void UpdateGhostVisual()
    {
        if(activeBuildingType == BuildingType.None)
        {
            if (ghostInstance != null)
                ghostInstance.SetActive(false);
            return;
        }

        Vector3 worldPos = Utils.GetMouseWorldPositionIzometrical();
        grid.GetXY(worldPos, out int x, out int y);

        PlanetTile tile = grid.GetGridObject(x, y);
        if (tile == null)
        {
            if (ghostInstance != null)
                ghostInstance.SetActive(false);
            return;
        }

        if(ghostInstance==null|| ghostType!=activeBuildingType)
        {
            if (ghostInstance != null)
                Destroy(ghostInstance);

            GameObject prefab = GetPrefabForType(activeBuildingType);
            if (prefab == null) return;

            ghostInstance = Instantiate(
                prefab,
                Vector3.zero,
                Quaternion.identity
            );

            if(ghostInstance.GetComponent<Animator>()!=null)
                ghostInstance.GetComponent<Animator>().enabled = false;

            ghostType = activeBuildingType;

            // Collider varsa kapatalım ki etkileşmesin
            foreach (var col in ghostInstance.GetComponentsInChildren<Collider>())
            {
                col.enabled = false;
            }
        }

        ghostInstance.SetActive(true);
        Vector3 center = grid.GetWorldPosition(x, y);
        ghostInstance.transform.position = center + Vector3.up * 0.2f;

        bool canPlace = CanPlaceOnTile(tile);

        ApplyGhostMaterial(canPlace);
    }

    private void ApplyGhostMaterial(bool canPlace)
    {
        if (ghostInstance == null) return;

        Material targetMat = canPlace ? ghostValidMaterial : ghostInvalidMaterial;
        if (targetMat == null) return;

        var renderers = ghostInstance.GetComponentsInChildren<Renderer>();
        foreach (var rend in renderers)
        {
            rend.material = targetMat;
        }
    }

    private bool CanPlaceOnTile(PlanetTile tile)
    {
        // Temel validasyonlar
        if (!tile.isDiscovered) return false;  // FOW kapalıysa yerleştirme
        if (tile.isOccupied) return false;

        // Zemin kuralları (ileride detaylandırılabilir)
        GameObject prefab = GetPrefabForType(activeBuildingType);
        if (prefab == null) return false;

        BuildingConfig config = prefab.GetComponent<BuildingConfig>();
        if (config == null) return true; // kural yoksa serbest

        if (config.onlyOnRock && tile.tileType != TileType.Rock) return false;
        if (config.onlyOnIce && tile.tileType != TileType.Ice) return false;

        if(activeBuildingType==BuildingType.Extractor)
        {
            if (!tile.hasOre && !tile.hasIce) return false; 
        }

        // Şimdilik eğim vs. yok, o yüzden requiresFlatTile kontrolünü es geçiyoruz.

        float futureProd = gridManager.totalEnergyProduction + config.energyProduction;
        float futureCons= gridManager.totalEnergyConsumption + config.energyConsumption;
        float futureNet = futureProd - futureCons;

        if (futureNet < 0f) return false; // enerji yetersiz

        if (!gridManager.resources.CanAfford(
                config.costOre,
                config.costMetal,
                config.costIce,
                config.costWater))
        {
            return false;
        }

        return true;
    }

    private GameObject GetPrefabForType(BuildingType type)
    {
        switch (type)
        {
            case BuildingType.Lander: return landerPrefab;
            case BuildingType.SolarPanel: return solarPanelPrefab;
            case BuildingType.Battery: return batteryPrefab;
            case BuildingType.Storage: return storagePrefab;
            case BuildingType.Refinery: return refineryPrefab;
            case BuildingType.Extractor: return extractorPrefab;
        }
        return null;
    }

    public void SetActiveBuildingType(BuildingType newType)
    {
        activeBuildingType = newType;
    }
}
