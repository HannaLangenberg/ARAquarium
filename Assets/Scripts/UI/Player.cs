using Scriptable_Objects.Inventory.Scripts;
using UnityEngine;

/// <summary>
/// Class <c>Player</c> stores the inventories for tank, fish, plant and decor, which in turn store all
/// available ...Item ScriptableObjects.
/// TODO Move to UI namespace
/// </summary>
public class Player : MonoBehaviour
{
    public InventoryObject tankInventory;
    public InventoryObject fishInventory;
    public InventoryObject plantInventory;
    public InventoryObject decorInventory;
}