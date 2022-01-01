using UnityEngine.UI;
using Scriptable_Objects.Items.Scripts;

namespace UI
{
    public class FishDetails : Details
    {
        public FishObject fishObject;

        public Text biologicalNameText;
        public Text speciesText;
        public Text sizeText;
        
        private void Start()
        {
            fishObject = itemObject as FishObject;
            biologicalNameText.text = "(" + fishObject.biologicalName + ")";
            speciesText.text = fishObject.species;
            sizeText.text = fishObject.length;
        }
        
        public void UpdateChanges(Details details)
        {
            details.UpdateChanges();
            Start();
        }
    }
}