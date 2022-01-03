using System;
using System.Collections.Generic;
using UnityEngine;
using Scriptable_Objects.Items.Scripts;

namespace Scriptable_Objects.Inventory.Scripts
{
    [CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
    public class InventoryObject : ScriptableObject
    {
        public List<InventorySlot> container = new List<InventorySlot>();

        public InventorySlot Contains(ItemObject itemObject)
        {
            foreach (InventorySlot inventorySlot in container)
            {
                if (inventorySlot.item == itemObject)
                {
                    return inventorySlot;
                }
            }
            return null;
        }

        public int Contains(ItemObject itemObject, List<InventorySlot> inventory)
        {
            foreach (InventorySlot inventorySlot in inventory)
            {
                if (inventorySlot.item == itemObject)
                {
                    return inventory.IndexOf(inventorySlot);
                }
            }
            return -1;
        }

        public InventorySlot AddOrRemoveAmount(ItemObject item, int decision)
        {
            InventorySlot inventorySlot = Contains(item);
            switch (decision)
            {
                case 0:
                    inventorySlot.AddAmount();
                    break;
                case 1:
                    if (inventorySlot.amount > 0)
                    {
                        inventorySlot.RemoveAmount();
                    }

                    break;
            }
            return inventorySlot;
        }

        public void ResetAllButActive(InventorySlot inventorySlot)
        {
            foreach (InventorySlot iSlot in container)
            {
                if (!iSlot.item.Equals(inventorySlot.item))
                {
                    iSlot.amount = 0;
                }
            }

            inventorySlot.amount = 1;
        }
    }

    [System.Serializable]
    public class InventorySlot
    {
        public ItemObject item;
        public int amount;
        public bool isActive;

        public InventorySlot(ItemObject item, int amount, bool isActive)
        {
            this.item = item;
            this.amount = amount;
            this.isActive = isActive;
        }
        public InventorySlot(ItemObject item, int amount)
        {
            this.item = item;
            this.amount = amount;
            isActive = true;
        }

        public void AddAmount()
        {
            if (amount > ((FishObject) item).minCount)
            {
                amount++;
            }
        }

        public void RemoveAmount()
        {
            amount--;
        }
    }
}