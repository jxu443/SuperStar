using System.Collections.Generic;
using Obi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CatcherArriveTrigger : MonoBehaviour
{
    public ObiSolver FluidSolver;
    public Text completionLabel;
    public ObiCollider finishLine;
    
    public ObiSoftbody softbody;
    public ObiEmitter fluid;
    public MazeController mazeController;
    public ThirdPersonCam thirdPersonCam;
    
    public GameObject walls;
    
    private HashSet<int> finishedParticles = new HashSet<int>();
    private int completion = 100;
    
    
    void Start()
    {
        completionLabel.text = "0% 到达, 目标 > 70% ";
        completionLabel.gameObject.SetActive(true);
        
        FluidSolver.OnCollision += Solver_OnCollision;
    }

    private void OnDestroy()
    {
        FluidSolver.OnCollision -= Solver_OnCollision;
    }
    
    
    private void Solver_OnCollision(ObiSolver s, ObiSolver.ObiCollisionEventArgs e)
    {
        var world = ObiColliderWorld.GetInstance();
        foreach (Oni.Contact contact in e.contacts)
        {
            // look for actual contacts only:
            if (contact.distance < 0.01f)
            {
                var col = world.colliderHandles[contact.bodyB].owner;
                if (finishLine == col)
                {
                    if (finishedParticles.Add(contact.bodyA))
                        UpdateScore(finishedParticles.Count);
                }
            }
        }
    }


    public void UpdateScore(int finishedParticlesCount)
    {
        completion = Mathf.CeilToInt(finishedParticlesCount / 720.0f * 100);
        completionLabel.text = completion + "% 到达, 目标 > 70% ";
        
        if (mazeController.enabled && completion > 70)
        {
            thirdPersonCam.SwitchCameraStyle(ThirdPersonCam.CameraStyle.Basic);
            mazeController.enabled = false;
            
            softbody.GetComponent<SoftbodyMovement>().enabled = true;
            fluid.GetComponent<FluidMovement>().enabled = true;

            completionLabel.gameObject.SetActive(false);
            
            walls.SetActive(false);
            Vector3 posToSet = new Vector3(4/2, -4/2 - 2 , 0.1f);
            var debug = new GameObject("debug");
            debug.transform.position = posToSet;
            
            softbody.Teleport(posToSet, Quaternion.Inverse(Quaternion.identity));
            //Invoke(nameof(delayUpdate),.5f);
        }
       
    }
    
    
    void delayUpdate()
    {
        fluid.GetComponent<FluidMovement>().SycnFluid();
    }

}
