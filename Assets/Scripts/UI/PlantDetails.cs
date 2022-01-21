using UnityEngine.UI;
using Scriptable_Objects.Items.Scripts;

namespace UI
{
    /// <summary>
    /// Class <c>PlantDetails</c> inherits from Details. Stores a PlantObject, which is a specific ItemObject, and
    /// additional serializable UI elements. 
    /// </summary>
    public class PlantDetails : Details
    {
        public PlantObject plantObject;
        
        public Text biologicalNameText;
        public Text speciesText;
        public Text height;
        
        /// <summary>
        /// Whenever a details panel in the UI is opened or updated Start updates its UI elements to
        /// display the information stored in the PlantObject. 
        /// </summary>
        private void Start()
        {
            plantObject = itemObject as PlantObject;
            biologicalNameText.text = "(" + plantObject.biologicalName + ")";
            speciesText.text = plantObject.species;
            height.text = plantObject.height;
        }
        /// <summary>
        /// Called if the details panel needs to be refreshed or opened.
        /// Calls UpdateChanges in Details and Start in PlantDetails to update common and additional UI.
        /// </summary>
        /// <param name="details"></param>
        public void UpdateChanges(Details details)
        {
            details.UpdateChanges();
            Start();
        }
    }
}