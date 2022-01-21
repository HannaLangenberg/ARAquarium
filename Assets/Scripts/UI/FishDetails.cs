using UnityEngine.UI;
using Scriptable_Objects.Items.Scripts;

namespace UI
{
    /// <summary>
    /// Class <c>FishDetails</c> inherits from Details. Stores a FishObject, which is a specific ItemObject, and
    /// additional serializable UI elements. 
    /// </summary>
    public class FishDetails : Details
    {
        public FishObject fishObject;

        public Text biologicalNameText;
        public Text speciesText;
        public Text sizeText;
        
        /// <summary>
        /// Whenever a details panel in the UI is opened or updated Start updates its UI elements to
        /// display the information stored in the FishObject. 
        /// </summary>
        private void Start()
        {
            fishObject = itemObject as FishObject;
            biologicalNameText.text = "(" + fishObject.biologicalName + ")";
            speciesText.text = fishObject.species;
            sizeText.text = fishObject.length;
        }
        /// <summary>
        /// Called if the details panel needs to be refreshed or opened.
        /// Calls UpdateChanges in Details and Start in FishDetails to update common and additional UI.
        /// </summary>
        /// <param name="details"></param>
        public void UpdateChanges(Details details)
        {
            details.UpdateChanges();
            Start();
        }
    }
}