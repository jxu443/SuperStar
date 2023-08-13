using System.Collections.Generic;
using Obi;
using UnityEngine;
using UnityEngine.UI;

public class PhaseChangeTrigger : MonoBehaviour
{
    public ObiSolver solver;
    public Text completionLabel;
    // public Text finishLabel;
    public ObiCollider finishLine;
    
    private HashSet<int> finishedParticles = new HashSet<int>();
    private int completion = 100;
    
    
    void Start()
    {
        solver.OnCollision += Solver_OnCollision;
    }

    private void OnDestroy()
    {
        solver.OnCollision -= Solver_OnCollision;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            // finishLabel.text = "Done...but not that good.";
            // finishLabel.color = new Color(0.8f, 0.2f, 0.2f);
            // finishLabel.gameObject.SetActive(true);
           
            
            var cmp = solver.GetComponent<SwitchMultipleActor>();
            cmp.VolumeScale = completion / 100.0f;
            
            solver.OnCollision -= Solver_OnCollision;
            
            cmp.SwitchParticleState = true;
            
        }
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


    public void UpdateScore(int finishedParticles)
    {
        completion = Mathf.CeilToInt(finishedParticles / 720.0f * 100);
        completionLabel.text = completion + "% Completed";
    }
    

}
