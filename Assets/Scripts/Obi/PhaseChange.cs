using System.Collections;
using System.Collections.Generic;
using Obi;
using UnityEngine;

public class PhaseChange : MonoBehaviour
{
    public ObiSoftbody softbody;
    public GameObject fluidSolver;

    private bool isFluid = false; 
    public bool switchPhase = true;
    
    // setter and getter
    public bool IsFluid
    {
        get => isFluid;
        set => isFluid = value;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) || switchPhase)
        {
            switchPhase = false;
            if (isFluid) // fluid to soft body
            {
                fluidSolver.GetComponentInChildren<FluidMovement>().enabled = false;
                fluidSolver.SetActive(false);


                softbody.deformationResistance = 1f;
                softbody.GetComponent<ObiParticleRenderer>().enabled = true;

            }
            else // soft body to fluid
            {
                softbody.GetComponent<ObiParticleRenderer>().enabled = false;
                softbody.deformationResistance = 0.5f;
                
                fluidSolver.transform.GetChild(0).gameObject.transform.localPosition = softbody.transform.localPosition;
                fluidSolver.SetActive(true);
                fluidSolver.GetComponentInChildren<FluidMovement>().enabled = true;

                //Debug.Log("softbody to fluid");
            }
            
            isFluid = !isFluid;
        }
    }
    
    public void SwitchPhase()
    {
        switchPhase = true;
    }
}
