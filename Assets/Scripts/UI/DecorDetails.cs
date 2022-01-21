using Scriptable_Objects.Items.Scripts;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Class <c>DecorDetails</c> inherits from Details. Stores a DecorObject, which is a specific ItemObject, and
    /// additional serializable UI elements. 
    /// </summary>
    public class DecorDetails : Details
    {
        public DecorObject decorObject;
        public Text width, height, depth;
        
        /// <summary>
        /// Whenever a details panel in the UI is opened or updated Start updates its UI elements to
        /// display the information stored in the DecorObject. 
        /// </summary>
        private void Start()
        {
            decorObject = (DecorObject) itemObject;

            width.text = decorObject.width;
            height.text = decorObject.height;
            depth.text = decorObject.depth;
        }
        /// <summary>
        /// Called if the details panel needs to be refreshed or opened.
        /// Calls UpdateChanges in Details and Start in DecorDetails to update common and additional UI.
        /// </summary>
        /// <param name="details"></param>
        public void UpdateChanges(Details details)
        {
            details.UpdateChanges();
            Start();
        }
    }
}
