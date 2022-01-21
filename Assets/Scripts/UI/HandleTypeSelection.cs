using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    /// <summary>
    /// Class <c>HandleTypeSelection</c> handles the user input when switching elements on the UI.
    /// </summary>
    public class HandleTypeSelection : MonoBehaviour
    {
        public GameObject player;
        public TankSelection tankSelection;
        public OtherSelection otherSelection;
        public GameObject tankSelectionPanel, otherSelectionPanel;

        public GameObject[] selectionPanels, detailPanels;
        // Arrays in order: Tank, Fish, Plant, Decor

        /// <summary>
        /// Called if tank icon on UI is pressed. Switches active scripts (disable OtherSelection and
        /// enable TankSelection if necessary). Sets the inventory, detailsPanel and Details. Sets all
        /// active DetailsPanels inactive and updates the SelectionUI.
        /// TODO 
        /// All ...Selected functions do the same on different inventories, detailsPanels, Details
        /// and SelectionUIs. Could be reduced to one function but time was missing. 
        /// </summary>
        public void AquariumSelected()
        {
            SwitchScripts(0);
            SwitchHighlight(0);

            tankSelection.inventoryToDisplay = tankSelection.GetComponent<Player>().tankInventory;
            tankSelection.detailsPanelToShow = detailPanels[0];
            tankSelection.detailsComponent = detailPanels[0].GetComponent<TankDetails>();
            SetAll_Detail_PanelsInactive();
            tankSelection.UpdateChanges();
        }
        /// <summary>
        /// Called if fish icon on UI is pressed. Switches active scripts (disable TankSelection and
        /// enable OtherSelection if necessary). Sets the inventory, detailsPanel and Details. Sets all
        /// active DetailsPanels inactive and updates the SelectionUI.
        /// TODO 
        /// All ...Selected functions do the same on different inventories, detailsPanels, Details
        /// and SelectionUIs. Could be reduced to one function but time was missing. 
        /// </summary>
        public void BesatzSelected()
        {
            SwitchScripts(1);
            SwitchHighlight(1);
            otherSelection.inventoryToDisplay = otherSelection.GetComponent<Player>().fishInventory;
            otherSelection.detailsPanelToShow = detailPanels[1];
            otherSelection.detailsComponent = detailPanels[1].GetComponent<FishDetails>();
            SetAll_Detail_PanelsInactive();
            otherSelection.UpdateChanges();
        }
        /// <summary>
        /// Called if plant icon on UI is pressed. Switches active scripts (disable TankSelection and
        /// enable OtherSelection if necessary). Sets the inventory, detailsPanel and Details. Sets all
        /// active DetailsPanels inactive and updates the SelectionUI.
        /// TODO 
        /// All ...Selected functions do the same on different inventories, detailsPanels, Details
        /// and SelectionUIs. Could be reduced to one function but time was missing. 
        /// </summary>
        public void PflanzenSelected()
        {
            SwitchScripts(1);
            SwitchHighlight(2);
            otherSelection.inventoryToDisplay = otherSelection.GetComponent<Player>().plantInventory;
            otherSelection.detailsPanelToShow = detailPanels[2];
            otherSelection.detailsComponent = detailPanels[2].GetComponent<PlantDetails>();
            SetAll_Detail_PanelsInactive();
            otherSelection.UpdateChanges();
        }
        /// <summary>
        /// Called if decor icon on UI is pressed. Switches active scripts (disable TankSelection and
        /// enable OtherSelection if necessary). Sets the inventory, detailsPanel and Details. Sets all
        /// active DetailsPanels inactive and updates the SelectionUI.
        /// TODO
        /// All ...Selected functions do the same on different inventories, detailsPanels, Details
        /// and SelectionUIs. Could be reduced to one function but time was missing. 
        /// </summary>
        public void DekoSelected()
        {
            SwitchScripts(1);
            SwitchHighlight(3);
            otherSelection.inventoryToDisplay = otherSelection.GetComponent<Player>().decorInventory;
            otherSelection.detailsPanelToShow = detailPanels[3];
            otherSelection.detailsComponent = detailPanels[3].GetComponent<DecorDetails>();
            SetAll_Detail_PanelsInactive();
            otherSelection.UpdateChanges();
        }
        /// <summary>
        /// Called if start icon on UI is pressed. Switches the scenes from AR selection mode to full AR mode.
        /// </summary>
        public void StartSelected()
        {
            SwitchHighlight(4);
            Debug.Log("Load Scene NR.:" + (SceneManager.GetActiveScene().buildIndex + 1));
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        /// <summary>
        /// Switches the highlight in the menu bar to show currently active selection.
        /// </summary>
        /// <param name="index">Which icon to highlight</param>
        private void SwitchHighlight(int index)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
            transform.GetChild(index).GetChild(0).gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Iterates over all DetailsPanels and sets them to inactive.
        /// </summary>
        private void SetAll_Detail_PanelsInactive()
        {
            foreach (GameObject panel in detailPanels)
            {
                panel.SetActive(false);
            }
        }

        /// <summary>
        /// Switches between tankSelection and otherSelection.
        /// </summary>
        /// <param name="decision"></param>
        private void SwitchScripts(int decision)
        {
            // 0 = use Tank
            // 1 = use Other
            switch (decision)
            {
                case 0:
                    if (!tankSelection.enabled)
                    {
                        tankSelection.enabled = true;
                        otherSelection.enabled = false;
                        tankSelectionPanel.SetActive(true);
                        otherSelectionPanel.SetActive(false);
                    }
                    break;
                
                case 1:
                    // if (!otherSelection.enabled)
                    if (!otherSelectionPanel.activeInHierarchy)
                    {
                        tankSelection.enabled = false;
                        otherSelection.enabled = true;
                        tankSelectionPanel.SetActive(false);
                        otherSelectionPanel.SetActive(true);
                    }
                    break;
            }
        }
    }
}