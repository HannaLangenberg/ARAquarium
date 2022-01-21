using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scriptable_Objects.Inventory.Scripts;
using Scriptable_Objects.Items.Scripts;
using Display;

namespace UI
{
    /// <summary>
    /// Class <c>OtherSelection</c> handles all actions that take place on the OtherSelectionUI regardless of
    /// currently displayed inventory.
    /// </summary>
    public class OtherSelection : MonoBehaviour
    {
        /// <summary>
        /// GameObject <c>slotHolder</c>. Scrollpanel that contains all available slots in inventory as prefabs
        /// </summary>
        [SerializeField]
        private GameObject slotHolder;
        /// <summary>
        /// List[InventorySlot] <c>_items</c>. Stores all items in the player's active inventory.
        /// </summary>
        private List<InventorySlot> _items;

        /// <summary>
        /// GameObject[] <c>_slots</c>. Stores all SlotPanel prefabs separately written from slotHolder for easier access.
        /// </summary>
        private GameObject[] _slots;
        /// <summary>
        /// Currently unsued
        /// </summary>
        private GameObject _slot;
        
        /// <summary>
        /// InventoryObject <c>inventoryToDisplay</c>. Currently active inventory.
        /// </summary>
        public InventoryObject inventoryToDisplay;
        /// <summary>
        /// Stores the DetailsPanel to show and the SelectionPanel to hide.
        /// </summary>
        public GameObject detailsPanelToShow, selectionPanelToHide;
        /// <summary>
        /// Details <c>detailsComponent</c>. The Details belonging to the DetailsPanel to show.
        /// </summary>
        public Details detailsComponent;
        /// <summary>
        /// InventorySlot <c>activeInventorySlot</c>. Stores the currently selected InventorySlot so
        /// that its stats can be tweaked. 
        /// </summary>
        public InventorySlot activeInventorySlot;
        /// <summary>
        /// GameObject <c>controls</c>. The controls on the SelectionPanel to show or hide them whenever necessary.
        /// </summary>
        public GameObject controls;
        /// <summary>
        /// Text <c>amountText</c> to show the amount of this object according to the values in the inventory.
        /// </summary>
        public Text amountText;

        /// <summary>
        /// - instantiates _slots with length of slotHolders childCount (all available slots on UI)
        /// - in a for loop places each child (SlotPanel prefab) separately in the _slots array
        /// - at the same time accesses the prefab's button and adds an onClickListener with a ClickAction function
        /// - calls RefreshSelectionUI to initially display thumbnail and amount
        /// - calls DisableFromFish to disable a fish if there is another present that would not harmonize with the
        ///   other when placed in an aquarium together.
        /// </summary>
        void Start()
        {
            _items = inventoryToDisplay.container;
            _slots = new GameObject[slotHolder.transform.childCount];
            slotHolder.GetComponentInParent<ScrollRect>().velocity = Vector2.zero;
            slotHolder.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                    slotHolder.GetComponent<RectTransform>().sizeDelta.x,
                    0);
            
            for (int i = 0; i < slotHolder.transform.childCount; i++)
            {
                _slots[i] = slotHolder.transform.GetChild(i).gameObject;
                try
                {
                    slotHolder.transform.GetChild(i).gameObject.SetActive(true);
                    InventorySlot inventorySlot = _items[i];
                    Button imageButton = slotHolder.transform.GetChild(i).GetComponent<Button>();
                    imageButton.onClick.RemoveAllListeners();
                    imageButton.onClick.AddListener(()
                                => ImageClicked(inventorySlot));
                    Button informationButton = slotHolder.transform.GetChild(i).GetChild(2).GetComponent<Button>();
                    informationButton.onClick.RemoveAllListeners();
                    informationButton.onClick.AddListener(()
                                => InformationClicked(inventorySlot));
                }
                catch
                {
                    slotHolder.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            RefreshSelectionUI();
            
            // CheckToDisable();
            // Disable();
            DisableFromFish(GetComponent<PlaceInScene>().activeTankInventorySlot);
        }

        /// <summary>
        /// Disables controls if touched fish is disabled or the SelectionUI is switched.
        /// </summary>
        private void DisableControls()
        {
            controls.SetActive(false);
        }

        // old function TODO see HandleStats
        /*private void CheckToDisable()
        {
            if (!inventoryToDisplay.Equals(GetComponent<Player>().fishInventory)) return;
            
            TankObject selectedTank = GetComponent<PlaceInScene>().activeTankObject;
                
            foreach (FishObject fishObject in selectedTank.disabledFish)
            {
                int index = GetComponent<Player>().fishInventory.Contains(fishObject,
                    GetComponent<Player>().fishInventory.container); 
                if (index >= 0)
                {
                    slotHolder.transform.GetChild(index).GetComponent<Image>().color = new Color(.7f, .7f, .7f);
                    GetComponent<Player>().fishInventory.container[index].isActive = false;
                }
            }
        }*/

        /// <summary>
        /// TODO see HandleStats
        /// </summary>
        public void Disable()
        {
            if (!inventoryToDisplay.Equals(GetComponent<Player>().fishInventory)) return;

            var inventorySlot = GetComponent<PlaceInScene>().activeTankInventorySlot;

            foreach (var disabledSlot in inventorySlot.item.disabledFishInventorySlots)
            {
                slotHolder.transform.GetChild(disabledSlot.slotIndex).GetComponent<Image>().color = new Color(.7f, .7f, .7f);
                GetComponent<Player>().fishInventory.container[disabledSlot.slotIndex].isActive = false;
            }
        }
        
        /// <summary>
        /// Disables a fish if there is another present that would not harmonize with the other when placed
        /// in an aquarium together. Returns if currently displayed inventorySlot is not a fishInventory.
        /// </summary>
        /// <param name="inventorySlot"></param>
        public void DisableFromFish(InventorySlot inventorySlot)
        {
            if (!inventoryToDisplay.Equals(GetComponent<Player>().fishInventory)) return;

            foreach (var disabledSlot in inventorySlot.item.disabledFishInventorySlots)
            {
                slotHolder.transform.GetChild(disabledSlot.slotIndex).GetComponent<Image>().color = new Color(.7f, .7f, .7f);
                GetComponent<Player>().fishInventory.container[disabledSlot.slotIndex].isActive = false;
            }
        }

        /// <summary>
        /// Called if the fish that caused the DisableFromFish is removed and enables the slot again.
        /// </summary>
        /// <param name="inventorySlot"></param>
        public void EnableFromFish(InventorySlot inventorySlot)
        {
            if (!inventoryToDisplay.Equals(GetComponent<Player>().fishInventory)) return;

            foreach (var disabledSlot in inventorySlot.item.disabledFishInventorySlots)
            {
                slotHolder.transform.GetChild(disabledSlot.slotIndex).GetComponent<Image>().color = Color.white;
                GetComponent<Player>().fishInventory.container[disabledSlot.slotIndex].isActive = true;
            }
        }

        /// <summary>
        /// Called whenever the image of an item is pressed.
        /// - sets the activeInventorySlot to the pressed one
        /// - refreshes the amount text
        /// - enables the controls
        /// </summary>
        /// <param name="inventorySlot"></param>
        private void ImageClicked(InventorySlot inventorySlot)
        {
            activeInventorySlot = inventorySlot;
            amountText.text = inventorySlot.amount.ToString("n0");
            controls.SetActive(inventorySlot.isActive);
        }

        /// <summary>
        /// Called whenever the information icon on an item is pressed.
        /// - sets the activeInventorySlot, the ItemObject and the amountTest to the pressed one
        /// - refreshes the DetailsUI
        /// - disables controls
        /// - shows DetailsPanel
        /// - places the scrollArea back to the start position (necessary if it was altered when the user opened the
        ///   DetailsPanel earlier.
        /// - Hides the SelectionUI
        /// </summary>
        /// <param name="inventorySlot"></param>
        private void InformationClicked(InventorySlot inventorySlot)
        {
            activeInventorySlot = inventorySlot;
            detailsComponent.itemObject = inventorySlot.item;
            detailsComponent.amountText.text = inventorySlot.amount.ToString();
            RefreshDetailsUI(inventorySlot);
            DisableControls();
            detailsPanelToShow.SetActive(true);
            detailsPanelToShow.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>().anchoredPosition =
            - new Vector2(0,
                detailsPanelToShow.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta.y);
            
            selectionPanelToHide.SetActive(false);
        }

        /// <summary>
        /// Refreshes the DetailsUI depending on which ItemType is to be displayed.
        /// </summary>
        /// <param name="inventorySlot"></param>
        private void RefreshDetailsUI(InventorySlot inventorySlot)
        {
            switch (inventorySlot.item.type)
            {
                case ItemType.Fish:
                    ((FishDetails)detailsComponent).UpdateChanges(detailsComponent);
                    break;
                case ItemType.Plant:
                    ((PlantDetails)detailsComponent).UpdateChanges(detailsComponent);
                    break;
                case ItemType.Decor:
                    ((DecorDetails)detailsComponent).UpdateChanges(detailsComponent);
                    break;
            }
        }

        /// <summary>
        /// Function attached to Plus button on the UI.
        /// The actual addition is done in PlaceInScene → AddNewOther which returns a boolean and updates the amount
        /// in the inventorySlot.
        /// If the operation was successful, the amountText on the controls and DetailsPanels is updated and the UI
        /// refreshed.
        /// </summary>
        public void Add()
        {
            if (GetComponent<PlaceInScene>().AddNewOther(activeInventorySlot))
            {
                amountText.text = activeInventorySlot.amount.ToString();
                detailsComponent.amountText.text = activeInventorySlot.amount.ToString();

                /*if (activeInventorySlot.item.tag.Equals("Discus") || activeInventorySlot.item.tag.Equals("Wetterschmerle"))
                {
                    DisableFromFish(activeInventorySlot);
                    GetComponent<PlaceInScene>().diskusWetterschmerle.gameObject.SetActive(true);
                }*/
                
                RefreshSelectionUI();
            }
        }
 

        /**
     * Function attached to Minus button on DetailsPanel. Forwards add operation to class intern function of
     * InventoryObject. Uses the returned InventorySlot to display the changes. Lastly refreshes UI.
     * Decision variable 1 → remove
     */
        
        public void Remove()
        {
            GetComponent<PlaceInScene>().RemoveExistingOther(activeInventorySlot);
            amountText.text = activeInventorySlot.amount.ToString();
            detailsComponent.amountText.text = activeInventorySlot.amount.ToString();
            
            if ((activeInventorySlot.item.tag.Equals("Discus")
                 || activeInventorySlot.item.tag.Equals("Wetterschmerle"))
                && ! (activeInventorySlot.amount > 0))
            {
                EnableFromFish(activeInventorySlot);
            }
            
            RefreshSelectionUI();
        }


        /// <summary>
        /// Called during Start() and whenever a change in the amounts of the objects occurs.
        /// For all present SlotPanels it enables the Image, sets the Image to the thumbnail, and sets the text to
        /// the amount.
        /// </summary>
        private void RefreshSelectionUI()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                _slots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                _slots[i].transform.GetChild(0).GetComponent<Image>().sprite = _items[i].item.thumbnail;
                _slots[i].transform.GetChild(1).GetComponent<Text>().text = _items[i].amount.ToString();
            }
        }
        /// <summary>
        /// Called by HandleTypeSelection if the inventory to display is changed.
        /// Enables all slots, disables the controls and calls Start.
        /// </summary>
        public void UpdateChanges()
        {
            EnableAllSlots();
            DisableControls();
            Start();
        }

        /// <summary>
        /// Resets all colors for the images to white and sets the InventorySlots in the inventory to display to active.
        /// </summary>
        private void EnableAllSlots()
        {
            for (int i = 0; i < inventoryToDisplay.container.Count; i++)
            {
                slotHolder.transform.GetChild(i).GetComponent<Image>().color = Color.white;
                inventoryToDisplay.container[i].isActive = true;
            }
        }
    }
}