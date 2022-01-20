using System;
using UnityEngine;

/// <summary>
/// Unused class. Ignore for now. Originally to test AR raycast selection during early development.
/// </summary>
public class SelectTank : MonoBehaviour
{
    [HideInInspector]
    public bool hitAccomplished;
    [HideInInspector]
    public String index;

    private GameObject presentTank;

    public Touch t;

    void Update()
    {
        CheckRay();
    }

    private void CheckRay()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            t = touch;

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hitTankInstance;
                
                if (Physics.Raycast(ray, out hitTankInstance))
                {
                    hitAccomplished = true;
                    if (hitTankInstance.transform.CompareTag("Tank"))
                    {
                        presentTank = hitTankInstance.transform.gameObject;
                    }
                    else
                    {
                        presentTank = null;
                    }
                    if (presentTank != null)
                    {
                        index = presentTank.name;
                    }
                }
                else
                {
                    hitAccomplished = false;
                }
            }
        }
    }
}
