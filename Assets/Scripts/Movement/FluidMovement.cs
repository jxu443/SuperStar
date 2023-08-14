using System.Collections;
using System.Collections.Generic;
using Obi;
using System;
using UnityEditor;
using UnityEngine;

public class FluidMovement : MonoBehaviour
{
    public Transform _referenceFrame;
    public float _acceleration = 4f;
    public ObiEmitter fluid;
    public ObiSoftbody softbody;
    public ObiSolver fluidSolver;
    public ObiFluidEmitterBlueprint fluidBlueprint;
    
    // trigger list
    public ObiCollider sugerTrigger;
    public JugController jc;

    public ObiCollider respawnTrigger = null;

    private bool freezed = false;
    private float duration = 0;
    private float transitionDuration = -1;
    private bool moveable = false;

    void Start()
    {
        Debug.Log(" fluid.solverIndices.Length: " + fluid.solverIndices.Length);
        Debug.Log(" softbody.solverIndices.Length: " + softbody.solverIndices.Length);
        fluidSolver.OnCollision += Solver_OnCollision;
    }
    
    private void OnDestroy()
    {
        fluidSolver.OnCollision -= Solver_OnCollision;
    }
    
    private void Solver_OnCollision(ObiSolver s, ObiSolver.ObiCollisionEventArgs e)
    {
        var world = ObiColliderWorld.GetInstance();
        foreach (Oni.Contact contact in e.contacts)
        {
            if (contact.distance < 0.01f)
            {
                var col = world.colliderHandles[contact.bodyB].owner;
                if (sugerTrigger == col)
                {
                    Debug.Log("FluidMOvement: Suger trigger, switch to JugController");
                    
                    this.enabled = false;
                    jc.enabled = true;
                } 
                // else if (respawnTrigger == col)
                // {
                //     Debug.Log("FluidMOvement: respawn");
                // } 
            }
        }
    }

    private void OnEnable()
    {
        for (int i = 0; i < fluid.solverIndices.Length; ++i)
        {
            int fluidSolverIndex = fluid.solverIndices[i];
            int len = softbody.solverIndices.Length;
            int SBsolverIndex = softbody.solverIndices[i % len];

            fluid.solver.positions[fluidSolverIndex] = softbody.solver.positions[SBsolverIndex];
            fluid.solver.velocities[fluidSolverIndex] = softbody.solver.velocities[SBsolverIndex];
        }
        
        if (fluidSolver.viscosities[0] >= 1f)
        {
            moveable = true;
            freezeMovement();
        }
    }

    private void OnDisable()
    {
        //Debug.Log("OnDisable");
        freezed = false;
        duration = 0;
        transitionDuration = -1;
        
        freezeMovement();
        moveable = false;
    }

    
    // freeze the velocity of the softbody and fluid 
    void freezeMovement()
    {
        for (int i = 0; i < softbody.solverIndices.Length; ++i)
        {
            int SBsolverIndex = softbody.solverIndices[i];
            int fluidSolverIndex = fluid.solverIndices[i];
            softbody.solver.velocities[SBsolverIndex] = new Vector4(0,0,0,0);
            fluid.solver.velocities[fluidSolverIndex] = new Vector4(0,0,0,0);
        }
        freezed = true;
    }
       


    void Update()
    {
        if (!moveable) return;

        // fluid is moveable 
        bool keyPress = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) 
                        || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.Space);

        if (!keyPress)
        {
            if (!freezed && softbody.solver.velocities[0].magnitude < 2f)
            {
                freezeMovement();
            }
            return;
        }
        

        if (freezed)
        {
            if (transitionDuration < 0)
            {
                float s = (softbody.solver.positions[0] - fluid.solver.positions[0]).magnitude;
                float v0 = fluid.solver.positions[0].magnitude;
                transitionDuration = (float)Math.Sqrt(2 * s / _acceleration);
                //transitionDuration = (-v0 +  (float)Math.Sqrt(v0 * v0 + 2 * s * _acceleration) ) / _acceleration;
                //Debug.Log("transitionDuration: " + transitionDuration);
            }

            duration += Time.deltaTime;
            for (int i = 0; i < fluid.solverIndices.Length; ++i){

                // retrieve the particle index in the solver:
                int fluidSolverIndex = fluid.solverIndices[i];
                int SBsolverIndex = softbody.solverIndices[i];
                    
                Vector3 from = fluid.solver.positions[fluidSolverIndex];
                Vector3 to = softbody.solver.positions[SBsolverIndex];
                    
                Vector3 positionToSet = Vector3.LerpUnclamped(from, to,
                    Mathf.SmoothStep(0f, 1f, duration / transitionDuration));
                    
                fluid.solver.positions[fluidSolverIndex] = positionToSet;
            }

            float dif = (softbody.solver.positions[0] - fluid.solver.positions[0]).magnitude;
            if ( dif < 0.1f)
            {
                freezed = false;
                duration = 0;
                transitionDuration = -1;
            }
            return;
        }
        

        for (int i = 0; i < fluid.solverIndices.Length; ++i)
        {
            int fluidSolverIndex = fluid.solverIndices[i];
            int len = softbody.solverIndices.Length;
            int SBsolverIndex = softbody.solverIndices[i % len];

            fluid.solver.positions[fluidSolverIndex] = softbody.solver.positions[SBsolverIndex];
            fluid.solver.velocities[fluidSolverIndex] = softbody.solver.velocities[SBsolverIndex];

        }

    }
}
