using UnityEngine;  

namespace Display
{
    public class KeepMe : MonoBehaviour
    {
        private static GameObject _aquariumInstance;
        void Start()
        {
            DontDestroyOnLoad(gameObject);
            _aquariumInstance = transform.GetChild(0).GetChild(0).gameObject;
        }

        public GameObject GETAquariumInstance()
        {
            return _aquariumInstance;
        }
    }
}
