using UnityEngine;

[System.Serializable]
public class AtmosphereComposition
{
    [Header("Genel")]
    [Tooltip("Toplam yüzey basýncý (kPa). Dünya ~101, Mars ~0.6")]
    public float totalPressureKPa = 0.6f;

    [Header("Ana gazlar (fraksiyon, toplam ~1 olmalý)")]
    [Range(0f, 1f)] public float n2Fraction = 0.02f;   // Azot
    [Range(0f, 1f)] public float o2Fraction = 0f;      // Oksijen
    [Range(0f, 1f)] public float co2Fraction = 0.95f;  // Karbondioksit
    [Range(0f, 1f)] public float h2oVaporFraction = 0.01f; // Su buharý
    [Range(0f, 1f)] public float ch4Fraction = 0f;     // Metan
    [Range(0f, 1f)] public float arFraction = 0.02f;   // Argon

    [Header("Toksik / özel gazlar")]
    [Range(0f, 1f)] public float so2Fraction = 0f;  // Kükürt dioksit
    [Range(0f, 1f)] public float h2sFraction = 0f;  // Hidrojen sülfür
    [Range(0f, 1f)] public float nh3Fraction = 0f;  // Amonyak
    [Range(0f, 1f)] public float coFraction = 0f;   // Karbon monoksit
    [Range(0f, 1f)] public float o3Fraction = 0f;   // Ozon
}

public enum StarType
{
    G, // Güneþ benzeri
    K,
    M,
    F,
    WhiteDwarf,
    Other
}

[System.Serializable]
public class OrbitalParameters
{
    [Header("Yýldýz")]
    public StarType starType = StarType.G;
    [Tooltip("Yýldýz parlaklýðý, Güneþ = 1")]
    public float stellarLuminosity = 1f;

    [Header("Yörünge")]
    [Tooltip("Yarý büyük eksen, AU cinsinden")]
    public float orbitSemiMajorAxisAU = 1.5f;
    [Range(0f, 0.9f)] public float orbitEccentricity = 0.05f;

    [Header("Dönme / Eksen")]
    [Tooltip("Eksen eðikliði (derece). Dünya ~23.5")]
    public float axialTiltDeg = 25f;
    [Tooltip("Bir günün uzunluðu (saat)")]
    public float dayLengthHours = 24.6f;
    [Tooltip("Bir yýlýn uzunluðu (gezegen günü cinsinden)")]
    public float yearLengthDays = 687f;
    [Range(0f, 1f)] public float seasonalVariation = 0.5f;
}

[System.Serializable]
public class SurfaceParameters
{
    [Header("Topoðrafya")]
    [Range(0f, 1f)] public float surfaceRoughness = 0.4f;
    [Range(0f, 1f)] public float craterDensity = 0.6f;
    [Range(0f, 1f)] public float plateTectonicsActivity = 0f; // Mars benzeri

    [Header("Mineral Daðýlýmý")]
    [Tooltip("Genel maden bolluðu çarpaný")]
    [Range(0f, 2f)] public float mineralAbundance = 1f;
    [Tooltip("Nadir element bolluðu")]
    [Range(0f, 2f)] public float rareElementAbundance = 0.8f;

    [Header("Enerji Ýliþkili")]
    [Tooltip("Gezegen albedosu, 0 = siyah, 1 = ayna")]
    [Range(0f, 1f)] public float albedo = 0.25f;
    [Tooltip("Jeotermal aktivite seviyesi")]
    [Range(0f, 1f)] public float geothermalActivity = 0.2f;
}

[System.Serializable]
public class HydrosphereParameters
{
    [Header("Toplam Su")]
    [Tooltip("Gezegendeki toplam su eþdeðeri (göreli birim)")]
    public float totalWaterEquivalent = 0.3f;

    [Header("Daðýlým")]
    [Range(0f, 1f)] public float iceCapCoverage = 0.4f;
    [Range(0f, 1f)] public float oceanCoverage = 0f;
    [Range(0f, 1f)] public float groundwaterLevel = 0.2f;
    [Tooltip("Ortalama buz derinliði çarpaný")]
    [Range(0f, 3f)] public float iceDepthFactor = 1f;
}

[System.Serializable]
public class BiosphereParameters
{
    [Header("Toprak & Biyoloji")]
    [Range(0f, 1f)] public float soilFertility = 0.1f;
    [Range(0f, 1f)] public float organicCarbonContent = 0.05f;
    [Tooltip("Mikrobiyal yaþam varlýðý (0 = yok, 1 = geliþmiþ)")]
    [Range(0f, 1f)] public float microbialLifePresence = 0f;
    [Tooltip("Uyku hâlinde spor / tohum bankasý")]
    [Range(0f, 1f)] public float sporeSeedBank = 0f;
}

[System.Serializable]
public class HazardParameters
{
    [Header("Atmosferik")]
    [Tooltip("Toz fýrtýnasý sýklýðý")]
    [Range(0f, 1f)] public float stormFrequency = 0.4f;
    [Tooltip("Mikro-meteor / meteor olayý sýklýðý")]
    [Range(0f, 1f)] public float meteorFrequency = 0.1f;
    [Tooltip("Atmosferdeki toz / aerosol yoðunluðu")]
    [Range(0f, 1f)] public float dustLoad = 0.5f;

    [Header("Radyasyon")]
    [Tooltip("Yüzey radyasyon seviyesi")]
    [Range(0f, 1f)] public float radiationLevel = 0.7f;
    [Tooltip("Güneþ patlamalarýna maruziyet")]
    [Range(0f, 1f)] public float solarFlareExposure = 0.5f;

    [Header("Elektriksel")]
    [Tooltip("Yýldýrým / elektrik aktivitesi sýklýðý")]
    [Range(0f, 1f)] public float lightningFrequency = 0.1f;
}

[System.Serializable]
public class PlanetEnvironment
{
    [Header("Kimlik")]
    public string planetName = "Unnamed Planet";

    [Header("Makro Parametreler")]
    [Tooltip("Ortalama yüzey sýcaklýðý (°C)")]
    public float baseTemperature = -55f;
    [Tooltip("Solar panel verim çarpaný (0–1+)")]
    public float solarIntensity = 0.8f;
    [Tooltip("Rüzgar türbini verim çarpaný (0–1)")]
    public float windStrength = 0.5f;

    [Header("Alt Sistemler")]
    public AtmosphereComposition atmosphere = new AtmosphereComposition();
    public OrbitalParameters orbit = new OrbitalParameters();
    public SurfaceParameters surface = new SurfaceParameters();
    public HydrosphereParameters hydrosphere = new HydrosphereParameters();
    public BiosphereParameters biosphere = new BiosphereParameters();
    public HazardParameters hazards = new HazardParameters();

    /// <summary>
    /// Mars benzeri bir gezegen için kabaca rasgeleleþtirilmiþ parametre doldurur.
    /// Þu an sadece seed üretimi; gameplay baðlantýsý ileriki haftalarda yapýlacak.
    /// </summary>
    public void RandomizeMarsLike()
    {
        // Sýcaklýk
        baseTemperature = Random.Range(-70f, -40f);

        // Enerji
        solarIntensity = Random.Range(0.6f, 0.9f);
        windStrength = Random.Range(0.3f, 0.8f);

        // Atmosfer
        atmosphere.totalPressureKPa = Random.Range(0.3f, 1.5f);
        atmosphere.co2Fraction = Random.Range(0.8f, 0.97f);
        atmosphere.n2Fraction = Random.Range(0.01f, 0.1f);
        atmosphere.o2Fraction = 0f;
        atmosphere.h2oVaporFraction = Random.Range(0.0f, 0.02f);
        atmosphere.ch4Fraction = Random.Range(0f, 0.02f);
        atmosphere.arFraction = Random.Range(0.01f, 0.05f);

        // Toksik gazlar (þimdilik düþük)
        atmosphere.so2Fraction = Random.Range(0f, 0.01f);
        atmosphere.h2sFraction = Random.Range(0f, 0.01f);
        atmosphere.nh3Fraction = Random.Range(0f, 0.01f);
        atmosphere.coFraction = Random.Range(0f, 0.01f);
        atmosphere.o3Fraction = 0f;

        // Yörünge
        orbit.starType = StarType.G;
        orbit.stellarLuminosity = Random.Range(0.7f, 1.2f);
        orbit.orbitSemiMajorAxisAU = Random.Range(1.2f, 1.8f);
        orbit.orbitEccentricity = Random.Range(0f, 0.2f);
        orbit.axialTiltDeg = Random.Range(10f, 35f);
        orbit.dayLengthHours = Random.Range(20f, 40f);
        orbit.yearLengthDays = Random.Range(250f, 800f);
        orbit.seasonalVariation = Random.Range(0.2f, 0.9f);

        // Yüzey
        surface.surfaceRoughness = Random.Range(0.3f, 0.8f);
        surface.craterDensity = Random.Range(0.3f, 0.9f);
        surface.plateTectonicsActivity = 0f;
        surface.mineralAbundance = Random.Range(0.7f, 1.5f);
        surface.rareElementAbundance = Random.Range(0.5f, 1.3f);
        surface.albedo = Random.Range(0.2f, 0.4f);
        surface.geothermalActivity = Random.Range(0f, 0.4f);

        // Hidrosfer
        hydrosphere.totalWaterEquivalent = Random.Range(0.1f, 0.6f);
        hydrosphere.iceCapCoverage = Random.Range(0.2f, 0.8f);
        hydrosphere.oceanCoverage = 0f;
        hydrosphere.groundwaterLevel = Random.Range(0f, 0.4f);
        hydrosphere.iceDepthFactor = Random.Range(0.8f, 2f);

        // Biyosfer (terraform öncesi genelde zayýf)
        biosphere.soilFertility = Random.Range(0f, 0.3f);
        biosphere.organicCarbonContent = Random.Range(0f, 0.2f);
        biosphere.microbialLifePresence = Random.Range(0f, 0.3f);
        biosphere.sporeSeedBank = Random.Range(0f, 0.2f);

        // Tehlikeler
        hazards.stormFrequency = Random.Range(0.2f, 0.9f);
        hazards.meteorFrequency = Random.Range(0f, 0.3f);
        hazards.dustLoad = Random.Range(0.3f, 0.9f);
        hazards.radiationLevel = Random.Range(0.4f, 0.95f);
        hazards.solarFlareExposure = Random.Range(0.2f, 0.9f);
        hazards.lightningFrequency = Random.Range(0f, 0.3f);
    }
}
