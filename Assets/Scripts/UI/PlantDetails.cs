using UnityEngine.UI;
using Scriptable_Objects.Items.Scripts;

namespace UI
{
    public class PlantDetails : Details
    {
        public PlantObject plantObject;

        public Text biologicalNameText;
        public Text speciesText;
        public Text height;
        
        private void Start()
        {
            plantObject = itemObject as PlantObject;
            biologicalNameText.text = "(" + plantObject.biologicalName + ")";
            speciesText.text = plantObject.species;
            height.text = plantObject.height;
        }
        public void UpdateChanges(Details details)
        {
            details.UpdateChanges();
            Start();
        }
    }
}