using Scriptable_Objects.Inventory.Scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scriptable_Objects.Items.Scripts;
using Display;

namespace UI
{
    public class TankSelection : MonoBehaviour
    {
        [SerializeField] //Scrollpanel that contains all available slots in inventory as prefabs
        private GameObject slotHolder;

        [SerializeField] private Color activeColor, inactiveColor;
        [SerializeField] private Sprite activeImage, inactiveImage;

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

        public InventoryObject inventoryToDisplay;
        public GameObject detailsPanelToShow, selectionPanelToHide;
        public Details detailsComponent;
        public InventorySlot activeInventorySlot;


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
                        => ImageClicked(inventorySlot, imageButton));
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
        }


        /**
     * Called whenever a button that is a child of a SlotPanel is pressed.
     * - sets the fishObject of the detailsPanel to the selected one
     * - triggers to refresh the UI
     */
        private void ImageClicked(InventorySlot inventorySlot, Button button)
        {
            activeInventorySlot = inventorySlot;
            SelectActiveSlot();
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
            detailsPanelToShow.SetActive(true);
            selectionPanelToHide.SetActive(false);
            ((TankDetails) detailsComponent).UpdateChanges(detailsComponent);
        }

        public void SelectActiveSlot()
        {
            inventoryToDisplay.ResetAllButActive(activeInventorySlot);
            RefreshSelectionUI();
            GetComponent<PlaceInScene>().SwitchTank();
        }


        /**
     * Called during Start() and whenever a change in the amounts of the objects occurs.
     * For all present SlotPanels it tries to enable the Image, sets the Image to the thumbnail, and sets the Text to
     * the amount. If it fails, as items[i] is null, the image is set to null, disabled and the text ist set to an
     * empty string.
     */
        public void RefreshSelectionUI()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                _slots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                _slots[i].transform.GetChild(0).GetComponent<Image>().sprite = _items[i].item.thumbnail;
                _slots[i].transform.GetChild(1).GetComponent<Text>().text = "";
                if (_items[i].amount == 1)
                {
                    // _slots[i].GetComponent<Image>().color = activeColor;
                    _slots[i].GetComponent<Image>().sprite = activeImage;
                }
                else
                {
                    _slots[i].GetComponent<Image>().sprite = inactiveImage;
                    // _slots[i].GetComponent<Image>().color = inactiveColor;
                }
            }
        }

        /**
     * Function attached to Plus button on DetailsPanel. Forwards add operation to class intern function of
     * InventoryObject. Uses the returned InventorySlot to display the changes. Lastly refreshes UI.
     * Decision variable 0 → add
     */
        public void Add(ItemObject itemObject)
        {
            InventorySlot iSlot = inventoryToDisplay.AddOrRemoveAmount(itemObject, 0);

            detailsComponent.amountText.text = iSlot.amount.ToString();
            RefreshSelectionUI();
        }

        /**
     * Function attached to Minus button on DetailsPanel. Forwards add operation to class intern function of
     * InventoryObject. Uses the returned InventorySlot to display the changes. Lastly refreshes UI.
     * Decision variable 1 → remove
     */
        public void Remove(ItemObject itemObject)
        {
            InventorySlot iSlot = inventoryToDisplay.AddOrRemoveAmount(itemObject, 1);

            detailsComponent.amountText.text = iSlot.amount.ToString();
            RefreshSelectionUI();
        }

        public void UpdateChanges()
        {
            Start();
        }
    }
}