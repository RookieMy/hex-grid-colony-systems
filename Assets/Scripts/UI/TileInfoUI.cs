using TMPro;
using UnityEngine;

public class TileInfoUI : MonoBehaviour
{
    [Header("References")]
    public GridSelector gridSelector;

    [Header("UI")]
    public GameObject panelRoot;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI detailText;

    private void Update()
    {
        if (gridSelector == null)
            return;

        PlanetTile tile = gridSelector.currentTile;

        // Geçerli bir tile yoksa veya FOW altýndaysa paneli gizle
        if (tile == null || !tile.isDiscovered)
        {
            if (panelRoot != null && panelRoot.activeSelf)
                panelRoot.SetActive(false);
            return;
        }

        if (panelRoot != null && !panelRoot.activeSelf)
            panelRoot.SetActive(true);

        // Baþlýk: koordinat + tileType
        if (titleText != null)
        {
            titleText.text = $"Tile ({tile.x}, {tile.y}) - {tile.tileType}";
        }

        // Detay: kaynaklar + bina
        if (detailText != null)
        {
            // Deposit alanlarýný sen ekledin; isimler farklýysa uyarlarsýn.
            string oreInfo = tile.hasOre ? tile.oreAmount.ToString() : "-";
            string iceInfo = tile.hasIce ? tile.iceAmount.ToString() : "-";

            string buildingInfo = "Empty";
            if (tile.isOccupied && tile.placedBuilding != null)
            {
                var cfg = tile.placedBuilding.GetComponent<BuildingConfig>();
                if (cfg != null)
                    buildingInfo = cfg.buildingType.ToString();
                else
                    buildingInfo = tile.placedBuilding.name;
            }

            detailText.text =
                $"Ore: {oreInfo}\n" +
                $"Ice: {iceInfo}\n" +
                $"Occupied: {tile.isOccupied}\n" +
                $"Building: {buildingInfo}";
        }
    }
}
