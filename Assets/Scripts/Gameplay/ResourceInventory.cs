using UnityEngine;

[System.Serializable]
public class ResourceInventory
{
    [Header("Resource Inventory")]
    public int ore;
    public int ice;
    public int water;
    public int metal;

    [Header("Capacity Limits")]
    public int oreCapacity = 50;
    public int iceCapacity = 50;
    public int waterCapacity = 50;
    public int metalCapacity = 50;

    public bool CanAfford(int oreCost, int metalCost, int iceCost, int waterCost)
    {
        if(oreCost > ore) return false;
        if(iceCost > ice) return false;
        if(waterCost > water) return false;
        if(metalCost > metal) return false;
        return true;
    }

    public void Spend(int costOre, int costMetal, int costWater, int costIce)
    {
        ore = Mathf.Max(0, ore - costOre);
        metal = Mathf.Max(0, metal - costMetal);
        ice = Mathf.Max(0, ice - costIce);
        water = Mathf.Max(0, water - costWater);
    }

    public void AddOre(int amount)
    {
        ore = Mathf.Min(ore + amount, oreCapacity);
    }

    public void AddMetal(int amount)
    {
        metal = Mathf.Clamp(metal + amount, 0, metalCapacity);
    }

    public void AddIce(int amount)
    {
        ice = Mathf.Clamp(ice + amount, 0, iceCapacity);
    }

    public void AddWater(int amount)
    {
        water = Mathf.Clamp(water + amount, 0, waterCapacity);
    }

    public void ClampToCapacity()
    {
        ore = Mathf.Min(ore, oreCapacity);
        ice = Mathf.Min(ice, iceCapacity);
        water = Mathf.Min(water, waterCapacity);
        metal = Mathf.Min(metal, metalCapacity);
    }
}
