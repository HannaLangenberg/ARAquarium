using System.Collections.Generic;
using Display;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Indicator
{
    public class PlacementButton : MonoBehaviour
    {
        private GameObject _keepMeARSessionOrigin;
        private KeepMe _keepMe;
        
        public GameObject placementIndicator;
        private GameObject _aquariumInstance;

        private ARRaycastManager _aRraycastManager;
        private Pose _placementPose; //Pose describes position and rotation of a point in 3D space
        private bool _placementPoseIsValid;
        
        private bool _disabled;

        void Start()
        {
            _keepMeARSessionOrigin = GameObject.Find("AR Session Origin");
            _keepMeARSessionOrigin.GetComponentInChildren<ARPoseDriver>().enabled = true;
            _keepMeARSessionOrigin.transform.localScale = Vector3.one;
            _keepMe = _keepMeARSessionOrigin.GetComponent<KeepMe>();
            _aRraycastManager = FindObjectOfType<ARRaycastManager>();
            
            _aquariumInstance = KeepMe.GETAquariumInstance();
            _keepMeARSessionOrigin.transform.GetChild(0).DetachChildren();
            _aquariumInstance.transform.position = Vector3.zero;
            _aquariumInstance.transform.rotation = Quaternion.identity;
            _aquariumInstance.transform.localScale = Vector3.one;
            _aquariumInstance.transform.GetChild(0).transform.position = Vector3.zero;
            _aquariumInstance.transform.GetChild(0).transform.rotation = Quaternion.identity;
            _aquariumInstance.transform.GetChild(0).transform.localScale = Vector3.one;
            _aquariumInstance.transform.parent = placementIndicator.transform;
        }

        void Update()
        {
            if (!_disabled)
            {
                UpdatePlacementPose();
                UpdatePlacementIndicator();
            }
        }
    
        private void UpdatePlacementPose()
        {
            Vector3 screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f,0));
            List<ARRaycastHit> hitsPlane = new List<ARRaycastHit>();
            _aRraycastManager.Raycast(screenCenter, hitsPlane, TrackableType.Planes);

            _placementPoseIsValid = hitsPlane.Count > 0;
        
            if (_placementPoseIsValid)
            {
                _placementPose = hitsPlane[0].pose;
            
                var cameraForward = Camera.main.transform.forward;
                var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
                _placementPose.rotation = Quaternion.LookRotation(cameraBearing);
            }
        }
    
        private void UpdatePlacementIndicator()
        {
            if (_placementPoseIsValid)
            {
                placementIndicator.SetActive(true);
                placementIndicator.transform.SetPositionAndRotation(_placementPose.position, _placementPose.rotation);
            }
            else
            {
                placementIndicator.SetActive(false);
            }
        }

        public void PlaceObject()
        {
            if (!_placementPoseIsValid) return;
            _aquariumInstance.transform.SetParent(null);
            placementIndicator.SetActive(false);
            _disabled = true;
        }
    
    
        public void QuitGame()
        {
            Debug.Log("Quit Game");
            Application.Quit();
        }
    }
}
