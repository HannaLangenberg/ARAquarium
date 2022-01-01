using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
public class PlacementButton : MonoBehaviour
{
    public GameObject placementIndicator;
    public GameObject Object;
    private GameObject tank;

    private ARRaycastManager ARraycastManager;
    private Pose placementPose; //Pose describes position and rotation of a point in 3D space
    private bool placementPoseIsValid = false;
    
    private int index;
    private bool disabled = false;

    void Start()
    {
        ARraycastManager = FindObjectOfType<ARRaycastManager>();
        tank = Instantiate(Object, placementIndicator.transform);
    }

    void Update()
    {
        if (!disabled)
        {
            UpdatePlacementPose();
            UpdatePlacementIndicator();
        }
    }
    
    private void UpdatePlacementPose()
    {
        Vector3 screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f,0));
        List<ARRaycastHit> hitsPlane = new List<ARRaycastHit>();
        ARraycastManager.Raycast(screenCenter, hitsPlane, TrackableType.Planes);

        placementPoseIsValid = hitsPlane.Count > 0;
        
        if (placementPoseIsValid)
        {
            placementPose = hitsPlane[0].pose;
            
            var cameraForward = Camera.main.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
    
    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    public void PlaceObject()
    {
        if (!placementPoseIsValid) return;
        tank.transform.SetParent(null);
        placementIndicator.SetActive(false);
        disabled = true;

        // Object.transform.SetParent(null);
        // Instantiate(Object, placementPose.position, placementPose.rotation);
    }
    
    
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
