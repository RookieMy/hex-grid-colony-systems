using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TerraformingUI : MonoBehaviour
{
    public GridManager gridManager;

    [Header("Temperature")]
    public Slider temperatureSlider;
    public TextMeshProUGUI temperatureLabel;

    [Header("Atmosphere")]
    public Slider atmosphereSlider;
    public TextMeshProUGUI atmosphereLabel;

    [Header("Oxygen")]
    public Slider oxygenSlider;
    public TextMeshProUGUI oxygenLabel;

    private void Update()
    {
        if (gridManager == null || gridManager.environment == null)
            return;

        var env = gridManager.environment;

        // 🔹 Temperature
        float t = env.baseTemperature;
        float tNorm = Mathf.InverseLerp(
            gridManager.minTemperature,
            gridManager.targetTemperature,
            t
        );
        tNorm = Mathf.Clamp01(tNorm);

        if (temperatureSlider != null)
            temperatureSlider.value = tNorm;

        if (temperatureLabel != null)
        {
            temperatureLabel.text = $"T: {t:0.0}°C / {gridManager.targetTemperature:0}°C";
        }

        // 🔹 Atmosphere (basit: mevcut basınç / hedef basınç)
        float p = env.atmosphere.totalPressureKPa;
        float pNorm = Mathf.InverseLerp(
            gridManager.minPressureKPa,
            gridManager.targetPressureKPa,
            p
        );
        pNorm = Mathf.Clamp01(pNorm);

        if (atmosphereSlider != null)
            atmosphereSlider.value = pNorm;

        if (atmosphereLabel != null)
        {
            atmosphereLabel.text = $"Atmo: {p:0.#} kPa / {gridManager.targetPressureKPa:0.#} kPa";
        }

        // 🔹 Oxygen (fraksiyon / hedef fraksiyon)
        float o2 = env.atmosphere.o2Fraction;
        float o2Norm = Mathf.InverseLerp(
            gridManager.minO2Fraction,
            gridManager.targetO2Fraction,
            o2
        );
        o2Norm = Mathf.Clamp01(o2Norm);

        if (oxygenSlider != null)
            oxygenSlider.value = o2Norm;

        if (oxygenLabel != null)
        {
            float o2Percent = o2 * 100f;
            float targetPercent = gridManager.targetO2Fraction * 100f;
            oxygenLabel.text = $"O₂: {o2Percent:0.0}% / {targetPercent:0.0}%";
        }
    }
}
