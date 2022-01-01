using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsTracker : MonoBehaviour
{
    public GameObject interaction;

    [SerializeField]    
    public Text nrval;

    [SerializeField]
    public Text posval;

    [SerializeField]
    public Text tval;
    
    [SerializeField]
    public Text hitval;

    // Update is called once per frame
    void Update()
    {
        nrval.text  = interaction.GetComponent<SelectTank>().index + " / " 
                    // + interaction.GetComponent<PlacementButton>().placedTanks.Count;
                    + GameObject.FindGameObjectsWithTag("Tank").Length;
        if (interaction.GetComponent<SelectTank>().hitAccomplished)
        {
            posval.text = interaction.GetComponent<SelectTank>().index.ToString();
            /*posval.text = interaction.GetComponent<SelectTank>().t.position.ToString()
                          + " - " + interaction.GetComponent<SelectTank>().currentTank.ID
                          + " - " + interaction.GetComponent<SelectTank>().currentTank.transform.position
                          + " - " + interaction.GetComponent<SelectTank>().currentTank.transform.rotation;*/
        }
        else
        {
            posval.text = interaction.GetComponent<SelectTank>().t.position.ToString();
        }
        tval.text   = interaction.GetComponent<SelectTank>().t.phase.ToString();
        hitval.text = interaction.GetComponent<SelectTank>().hitAccomplished.ToString();
    }
}
