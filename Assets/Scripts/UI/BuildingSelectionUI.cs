using UnityEngine;

public class BuildingSelectionUI : MonoBehaviour
{
    public BuildingPlacer buildingPlacer;

    public void SelectBuilding(int typeIndex)
    {
        if(buildingPlacer==null) return;

        BuildingType type = (BuildingType)typeIndex;
        buildingPlacer.activeBuildingType = type;
    }

    public void ClearSelection()
    {
        if(buildingPlacer==null) return;
        buildingPlacer.SetActiveBuildingType(BuildingType.None);
    }
}
