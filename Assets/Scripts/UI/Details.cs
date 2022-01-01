using UnityEngine;
using UnityEngine.UI;
using Scriptable_Objects.Items.Scripts;

namespace UI
{
    public class Details : MonoBehaviour
    {
        public ItemObject itemObject;

        public Image thumbnailImage;
        public Text nameText;
        public Text furtherInformationText;
        public Text amountText;
        public GameObject player;

        void Start()
        {
            thumbnailImage.sprite = itemObject.thumbnail;
            nameText.text = itemObject.name;
            furtherInformationText.text = itemObject.furtherInformation;
        }

        public void UpdateChanges()
        {
            Start();
        }
    }
}