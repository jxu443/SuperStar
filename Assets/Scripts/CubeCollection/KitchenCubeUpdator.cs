using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
public class KitchenCubeUpdator : MonoBehaviour
{
    
    public ObiSolver FluidSolver; 
    public ObiCollider KitchenCube;
    public LifeUpdater lifeUpdater;
    public int lifePerCube = 10;
    public AudioSource audioSource;
    public MeshRenderer meshRenderer;
    void Start()
    {
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
                if (KitchenCube == col)
                {
                    meshRenderer.enabled = false;
                    audioSource.enabled = true;
                    lifeUpdater.AddLife(lifePerCube);
                    Invoke(nameof(myDestry), .8f);
                    
                }
            }
        }
    }

    void myDestry()
    {
        Destroy(KitchenCube.gameObject);
    }
    
    
    // rotation
    void FixedUpdate()
    {
        transform.Rotate(0, 0, 50 * Time.deltaTime);
    }
}
