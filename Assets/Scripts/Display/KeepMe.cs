using UnityEngine;  

namespace Display
{
    /// <summary>
    /// This class <c>KeepMe</c> sets the ARSessionOrigin to DontDestroyOnLoad so it with the customized aquarium
    /// can be transferred to the next scene (full ARMode).
    /// </summary>
    public class KeepMe : MonoBehaviour
    {
        /// <summary>
        /// Static GameObject <c>_aquariumInstance</c> stores the aquarium instance itself when switching the scene.
        /// </summary>
        private static GameObject _aquariumInstance;
        /// <summary>
        /// Sets the GameObject to which it is attached to DontDestroyOnLoad and sets the aquarium instance.
        /// </summary>
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            _aquariumInstance = transform.GetChild(0).GetChild(0).gameObject;
        }
        /// <summary>
        /// GET function to retrieve the private _aquariumInstance. 
        /// </summary>
        /// <returns>The _aquariumInstance</returns>
        public static GameObject GETAquariumInstance()
        {
            return _aquariumInstance;
        }
    }
}
