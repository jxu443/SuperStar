using System.Collections;
using System.Collections.Generic;
using Obi;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnTrigger : MonoBehaviour
{
    public ObiSolver solver;
    public ObiSoftbody softbody;
    public FluidMovement fluidmovement;
    public ObiCollider respawnTrigger;
    public Transform respawPoint = null;


    void Start()
    {
     
        
        solver.OnCollision += Solver_OnCollision;
    }

    private void OnDestroy()
    {
        solver.OnCollision -= Solver_OnCollision;
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
                if (respawnTrigger == col)
                {
                    if (respawPoint != null)
                    {
                        softbody.Teleport(respawPoint.position, Quaternion.identity);
                        Invoke(nameof(fluidmovement.SycnFluid), .2f);
                    }
                    else
                    {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
                    }
                }
            }
        }
    }

}
