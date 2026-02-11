using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("References")]
    public PlanetVisualizer planetVisualizer;
    public GridSelector gridSelector;
    public FOWManager fowManager;
    public LanderSpawner landerSpawner;
    public BuildingPlacer buildingPlacer;

    [Header("Grid Settings")]
    public int width = 50;
    public int height = 50;
    public float cellSize = 1.5f;
    public bool showDebug = false;
    public float noiseScale = 0.07f;
    public int seed;
    public int maxOreAmountPerNode = 200;
    public int minOreAmountPerNode = 50;
    public int maxIceAmountPerNode = 200;
    public int minIceAmountPerNode = 50;

    [Header("Resources")]
    public ResourceInventory resources = new ResourceInventory();

    [Header("Storage Settings")]
    public int baseOreCapacity = 50;
    public int baseIceCapacity = 50;
    public int baseWaterCapacity = 50;
    public int baseMetalCapacity = 50;

    public int storageOreBonus = 100;
    public int storageIceBonus = 100;
    public int storageWaterBonus = 100;
    public int storageMetalBonus = 100;

    [Header("Production Settings")]
    public float refineryTickInterval = 1f;
    private float refineryTickTimer;

    public HexGrid<PlanetTile> grid { get; private set; }

    

    [Header("Generation Settings")]
    [Range(0f, 1f)]public float oreChance = 0.06f;
    [Range(0f, 1f)]public float iceChance = 0.04f;

    [Header("Global Environment (generated)")]
    public PlanetEnvironment environment = new PlanetEnvironment();

    [Header("Energy (calculated)")]
    public float totalEnergyProduction;
    public float totalEnergyConsumption;
    public float netEnergy;

    [Header("Terraforming Targets")]
    [Tooltip("Hedef sýcaklýk (°C) - yaþanýlabilir ortalama")]
    public float targetTemperature = 5f;
    [Tooltip("Sýcaklýk minimum görünür aralýk (°C)")]
    public float minTemperature = -80f;

    [Tooltip("Hedef atmosfer basýncý (kPa). Dünya ~101, biz belki 60-80 arasý isteriz.")]
    public float targetPressureKPa = 70f;
    [Tooltip("UI için alt sýnýr (örneðin vakuma yakýn)")]
    public float minPressureKPa = 0f;

    [Tooltip("Hedef O2 fraksiyonu (Dünya ~0.21)")]
    public float targetO2Fraction = 0.21f;
    [Tooltip("Yaþanabilir alt sýnýr (örneðin 0.15)")]
    public float minO2Fraction = 0f;

    [Header("Terraforming Tick")]
    public float terraformingTickInterval = 1f;
    private float terraformingTickTimer;


    private void Start()
    {
        
        grid = new HexGrid<PlanetTile>(width, height, cellSize, Vector3.zero,
            (g, x, y) => new PlanetTile(g, x, y),
            isPointyTopped: true,
            this.showDebug
        );

        gridSelector.SetGrid(grid);

        GenerateEnvironment();

        GenerateTiles();

        planetVisualizer.DrawPlanet();
        fowManager.CreateFog();
        landerSpawner.StartToSpawn();
        buildingPlacer.SetGrid(grid);

        RecalculateStorageCapacities();
        RecalculateEnergy();

        terraformingTickTimer = terraformingTickInterval;
    }
    private void Update()
    {
        refineryTickTimer -= Time.deltaTime;

        if(refineryTickTimer <= 0f)
        {
            refineryTickTimer = refineryTickInterval;

            if(netEnergy<=0f)
                return;

            ProcessProductionTick();
        }

        terraformingTickTimer -= Time.deltaTime;
        if (terraformingTickTimer <= 0f)
        {
            terraformingTickTimer = terraformingTickInterval;

            // Enerji yoksa terraform makineleri de çalýþmasýn istersen:
            if (netEnergy <= 0f)
                return;

            ProcessTerraformingTick();
        }
    }

    private void ProcessTerraformingTick()
    {

        if (environment == null)
            return;

        float deltaTemp = 0f;
        float deltaPressure = 0f;
        float deltaO2 = 0f;

        // Aktif binalarý tara
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                PlanetTile tile = grid.GetGridObject(x, y);
                if (tile == null || tile.placedBuilding == null)
                    continue;

                BuildingConfig config = tile.placedBuilding.GetComponent<BuildingConfig>();
                if (config == null)
                    continue;

                // Bina çalýþýyor mu? (ileride power on/off gibi durumlar eklenebilir)
                // Þimdilik netEnergy pozitifse hepsini aktif kabul ediyoruz.

                deltaTemp += config.temperatureDeltaPerTick;
                deltaPressure += config.pressureDeltaPerTick;
                deltaO2 += config.oxygenDeltaPerTick;
            }
        }

        // Global environment’ý güncelle
        environment.baseTemperature += deltaTemp;
        environment.atmosphere.totalPressureKPa = Mathf.Max(
            0f,
            environment.atmosphere.totalPressureKPa + deltaPressure
        );

        environment.atmosphere.o2Fraction = Mathf.Clamp01(
            environment.atmosphere.o2Fraction + deltaO2
        );
    }

    private void ProcessProductionTick()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                PlanetTile tile = grid.GetGridObject(x, y);
                if(tile == null || tile.placedBuilding == null)
                    continue;
                
                BuildingConfig config = tile.placedBuilding.GetComponent<BuildingConfig>();
                if(config == null)
                    continue;

                if (config.oreToMetalPerTick > 0)
                {
                    if (resources.ore >= config.oreToMetalPerTick && resources.metal < resources.metalCapacity)
                    {
                        resources.ore -= config.oreToMetalPerTick;
                        resources.metal = Mathf.Min(resources.metal + config.oreToMetalPerTick, resources.metalCapacity);
                    }
                }

                if (config.iceToWaterPerTick > 0)
                {
                    if (resources.ice >= config.iceToWaterPerTick &&
                       resources.water < resources.waterCapacity)
                    {
                        resources.ice -= config.iceToWaterPerTick;
                        resources.water = Mathf.Min(
                            resources.water + config.iceToWaterPerTick,
                            resources.waterCapacity
                        );
                    }
                }

                if(config.oreExtractionPerTick > 0)
                {
                    if(tile.hasOre && resources.ore < resources.oreCapacity && tile.oreAmount > 0)
                    {
                        resources.ore = Mathf.Min(
                            resources.ore + config.oreExtractionPerTick,
                            resources.oreCapacity
                        );

                        tile.oreAmount = Mathf.Max(tile.oreAmount - config.oreExtractionPerTick, 0);
                    }
                }

                if(config.iceExtractionPerTick > 0)
                {
                    if (tile.hasIce && resources.ice < resources.iceCapacity && tile.iceAmount > 0)
                    {
                        resources.ice = Mathf.Min(
                            resources.ice + config.iceExtractionPerTick,
                            resources.iceCapacity
                        );

                        tile.iceAmount = Mathf.Max(tile.iceAmount - config.iceExtractionPerTick, 0);
                    }
                }
            }
        }
    }

    public void RecalculateStorageCapacities()
    {
        int storageCount = 0;

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                PlanetTile tile = grid.GetGridObject(x, y);
                if(tile == null || tile.placedBuilding == null)
                    continue;

                BuildingConfig config = tile.placedBuilding.GetComponent<BuildingConfig>();
                if(config == null)
                    continue;
                
                if(config.buildingType == BuildingType.Storage)
                {
                    storageCount++;
                }
            }
        }

        resources.oreCapacity = baseOreCapacity + storageCount * storageOreBonus;
        resources.iceCapacity = baseIceCapacity + storageCount * storageIceBonus;
        resources.waterCapacity = baseWaterCapacity + storageCount * storageWaterBonus;
        resources.metalCapacity = baseMetalCapacity + storageCount * storageMetalBonus;

        resources.ClampToCapacity();
    }

    public void RecalculateEnergy()
    {
        totalEnergyProduction = 0f;
        totalEnergyConsumption = 0f;
        
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                PlanetTile tile = grid.GetGridObject(x, y);
                if(tile == null || tile.placedBuilding == null)
                    continue;

                BuildingConfig config = tile.placedBuilding.GetComponent<BuildingConfig>();
                if(config == null)
                    continue;

                totalEnergyProduction += config.energyProduction;
                totalEnergyConsumption += config.energyConsumption;
            }
        }

        netEnergy = totalEnergyProduction - totalEnergyConsumption;

        Debug.Log($"Energy: prod={totalEnergyProduction}, cons={totalEnergyConsumption}, net={netEnergy}");
    }

    private PlanetTile CreateTile(Grid<PlanetTile> g, int x, int y)
    {
        return new PlanetTile(g, x, y);
    }

    private void GenerateEnvironment()
    {
        if (environment == null)
            environment = new PlanetEnvironment();

        environment.RandomizeMarsLike();

        // Debug amaçlý birkaç temel deðeri loglayalým
        Debug.Log(
            $"Environment: T={environment.baseTemperature:F1}°C, " +
            $"P={environment.atmosphere.totalPressureKPa:F2}kPa, " +
            $"Solar={environment.solarIntensity:F2}, " +
            $"Minerals={environment.surface.mineralAbundance:F2}"
        );
    }

    private void GenerateTiles()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                PlanetTile tile = grid.GetGridObject(x, y);

                float noise = Mathf.PerlinNoise(x * 0.07f, y * 0.07f);
                if (noise > .75f)
                    tile.tileType = TileType.Ice;
                else if (noise > .45f)
                    tile.tileType = TileType.Dust;
                else
                    tile.tileType = TileType.Rock;

                // MineralAbundance çarpaný ile ore daðýlýmýný etkileyelim
                float oreRoll = Random.value;
                float iceRoll = Random.value;

                float oreChanceEffective = oreChance * environment.surface.mineralAbundance;
                float iceChanceEffective = iceChance * environment.hydrosphere.iceDepthFactor;

                if (oreRoll < oreChanceEffective)
                    tile.hasOre = true;

                if (iceRoll < iceChanceEffective)
                    tile.hasIce = true;

                tile.isDiscovered = false;

                if (tile.hasIce)
                    tile.iceAmount = Random.Range(minIceAmountPerNode, maxIceAmountPerNode);
                if (tile.hasOre)
                    tile.oreAmount = Random.Range(minOreAmountPerNode, maxOreAmountPerNode);
            }
        }
    }


    public void RevealAreaAt(Vector3 worldPosition)
    {
        fowManager.RevealAround(worldPosition);
    }
}
