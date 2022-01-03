using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scriptable_Objects.Inventory.Scripts;
using Scriptable_Objects.Items.Scripts;
using Display;

namespace UI
{
    public class OtherSelection : MonoBehaviour
    {
        [SerializeField] //Scrollpanel that contains all available slots in inventory as prefabs
        private GameObject slotHolder;

        /*
         * Differentiate: The ScrollPanel(aka slotHolder) shows all available items even if quantity is one.
         * So: Items.length should be exactly how many different fish are available. For testing and debugging items can be
         * set multiple times in different amounts. For final compilation each fish should be present once and set to an
         * amount of zero.
         * As a display for the player in AR which fish he selected use only fish that have an amount greater than 0.
         */
        private List<InventorySlot> _items;

        //_slots: array that stores all SlotPanel prefabs separately written from slotHolder for easier access
        private GameObject[] _slots;
        private GameObject _slot;
        
        public InventoryObject inventoryToDisplay;
        public GameObject detailsPanelToShow, selectionPanelToHide;
        public Details detailsComponent;
        public InventorySlot activeInventorySlot;
        public GameObject controls;

        public Text amountText;

        /**
     * - instantiates _slots with length of slotHolders childCount (all available slots on UI)
     * - in a for loop places each child (SlotPanel prefab) separately in the _slots array
     *      - at the same time accesses the prefab's button and adds an onClick Listener with a ClickAction function
     *        (later more)
     * - calls RefreshSelectionUI to initially display thumbnail and amount 
     */
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
            
            CheckToDisable();
        }

        private void DisableControls()
        {
            controls.SetActive(false);
        }

        private void CheckToDisable()
        {
            if (inventoryToDisplay.Equals(GetComponent<Player>().fishInventory))
            {
                TankObject selectedTank = GetComponent<PlaceInScene>().activeTankObject;
                
                foreach (FishObject fishObject in selectedTank.disabledFish)
                {
                    int index = GetComponent<Player>().fishInventory.Contains(fishObject,
                        GetComponent<Player>().fishInventory.container); 
                    if (index >= 0)
                    {
                        slotHolder.transform.GetChild(index).GetComponent<Image>().color = Color.gray;
                        GetComponent<Player>().fishInventory.container[index].isActive = false;
                    }
                }
            }
        }

        /**
     * Called whenever a button that is a child of a SlotPanel is pressed.
     * - sets the fishObject of the detailsPanel to the selected one
     * - triggers to refresh the UI
     */
        private void ImageClicked(InventorySlot inventorySlot)
        {
            activeInventorySlot = inventorySlot;
            amountText.text = inventorySlot.amount.ToString("n0");
            controls.SetActive(inventorySlot.isActive);
        }

        /**
     * Called whenever a button that is a child of a SlotPanel is pressed.
     * - sets the fishObject of the detailsPanel to the selected one
     * - triggers to refresh the UI
     */
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

        /**
     * Function attached to Plus button on DetailsPanel. Forwards add operation to class intern function of
     * InventoryObject. Uses the returned InventorySlot to display the changes. Lastly refreshes UI.
     * Decision variable 0 → add
     */
        public void Add()
        {
            if (GetComponent<PlaceInScene>().AddNewOther(activeInventorySlot))
            {
                amountText.text = activeInventorySlot.amount.ToString();
                detailsComponent.amountText.text = activeInventorySlot.amount.ToString();
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
            RefreshSelectionUI();
        }


        /**
     * Called during Start() and whenever a change in the amounts of the objects occurs.
     * For all present SlotPanels it tries to enable the Image, sets the Image to the thumbnail, and sets the Text to
     * the amount. If it fails, as items[i] is null, the image is set to null, disabled and the text ist set to an
     * empty string.
     */
        private void RefreshSelectionUI()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                _slots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                _slots[i].transform.GetChild(0).GetComponent<Image>().sprite = _items[i].item.thumbnail;
                _slots[i].transform.GetChild(1).GetComponent<Text>().text = _items[i].amount.ToString();
            }
        }
        
        public void UpdateChanges()
        {
            EnableAllSlots();
            DisableControls();
            Start();
        }

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