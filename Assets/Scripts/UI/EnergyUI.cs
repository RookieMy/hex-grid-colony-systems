using TMPro;
using UnityEngine;

public class EnergyUI : MonoBehaviour
{
    public GridManager gridManager;

    public TextMeshProUGUI productionText;
    public TextMeshProUGUI consumptionText;
    public TextMeshProUGUI netText;

    private void Update()
    {
        if(gridManager == null || gridManager.grid == null)
            return;

        float prod = gridManager.totalEnergyProduction;
        float cons = gridManager.totalEnergyConsumption;
        float net = gridManager.netEnergy;

        if(productionText != null)
            productionText.text = $"Prod: {prod:0.0}";
        if (consumptionText != null)
            consumptionText.text = $"Cons: {cons:0.0}";
        if (netText != null)
        {
            netText.text = $"Net: {net:0.0}";

            if(net > .01f)
                netText.color = Color.green;
            else if(net < -.01f)
                netText.color = Color.red;
            else
                netText.color = Color.white;

        }


    }
}
