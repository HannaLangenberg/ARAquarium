using Scriptable_Objects.Items.Scripts;
using UnityEngine.UI;

namespace UI
{
    public class DecorDetails : Details
    {
        public DecorObject decorObject;
        public Text width, height, depth;
        
        private void Start()
        {
            decorObject = (DecorObject) itemObject;

            width.text = decorObject.width;
            height.text = decorObject.height;
            depth.text = decorObject.depth;
        }
        public void UpdateChanges(Details details)
        {
            details.UpdateChanges();
            Start();
        }
    }
}
