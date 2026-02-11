using System.Runtime.CompilerServices;
using UnityEngine;

public class BuildingConfig : MonoBehaviour
{
    [Header("Kimlik")]
    public BuildingType buildingType;
    public BuildingCategory buildingCategory;

    [Header("Maliyet")]
    public int costMetal = 10;
    public int costIce = 0;
    public int costWater = 0;
    public int costOre = 0;

    [Header("Enerji")]
    public float energyProduction = 0f;
    public float energyConsumption = 0f;

    [Header("Üretim Döngüsü (per one tick)")]
    public int oreToMetalPerTick = 0;
    public int iceToWaterPerTick = 0;
    public int oreExtractionPerTick = 0;
    public int iceExtractionPerTick = 0;

    [Header("Yerleþim Þartlarý")]
    public bool requiresFlatTile = false;
    public bool onlyOnRock = false;
    public bool onlyOnIce = false;

    [Header("Terraform Döngüsü (per one tick)")]
    [Tooltip("Bu bina her tick sýcaklýðý kaç °C arttýrýr/azaltýr")]
    public float temperatureDeltaPerTick = 0f;

    [Tooltip("Bu bina her tick atmosfer basýncýný kaç kPa deðiþtirir")]
    public float pressureDeltaPerTick = 0f;

    [Tooltip("Bu bina her tick O2 fraksiyonunu kaç birim deðiþtirir (ör: 0.0001)")]
    public float oxygenDeltaPerTick = 0f;

}
