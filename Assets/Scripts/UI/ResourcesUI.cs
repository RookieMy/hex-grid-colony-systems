using TMPro;
using UnityEngine;

public class ResourcesUI : MonoBehaviour
{
    public GridManager gridManager;

    public TextMeshProUGUI oreText;
    public TextMeshProUGUI iceText;
    public TextMeshProUGUI waterText;
    public TextMeshProUGUI metalText;

    private void Update()
    {
        if (gridManager == null) return;

        var r = gridManager.resources;

        if (oreText != null)
            oreText.text = $"Ore: {r.ore}/{r.oreCapacity}";
        if (metalText != null)
            metalText.text = $"Metal: {r.metal}/{r.metalCapacity}";
        if (iceText != null)
            iceText.text = $"Ice: {r.ice}/{r.iceCapacity}";
        if (waterText != null)
            waterText.text = $"Water: {r.water}/{r.waterCapacity}";

    }
}
