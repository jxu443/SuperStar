using System.Collections;
using System.Collections.Generic;
using Obi;
using System;
using UnityEditor;
using UnityEngine;

public class tryFluidMovement : MonoBehaviour
{
    public Transform _referenceFrame;
    public float _acceleration = 4f;
    //public Rigidbody rigidBall;
    public ObiEmitter fluid;
    public ObiSoftbody softbody;

    private bool freezed = false;
    private float duration = 0;
    private float transitionDuration = -1;

    void Start()
    {
        Debug.Log(" fluid.solverIndices.Length: " + fluid.solverIndices.Length);
        Debug.Log(" softbody.solverIndices.Length: " + softbody.solverIndices.Length);
    }
    
    
    void Update()
    {
        Vector3 direction = Vector3.zero;

        // Determine movement direction:
        if (Input.GetKey(KeyCode.W))
        {
            direction += _referenceFrame.forward * _acceleration;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += -_referenceFrame.right * _acceleration;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += -_referenceFrame.forward * _acceleration;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += _referenceFrame.right * _acceleration;
        }
        
        if (direction == Vector3.zero)
        {
            if (!freezed && softbody.solver.positions[0].magnitude < 2f)
            {
                Debug.Log("freeze");
                for (int i = 0; i < softbody.solverIndices.Length; ++i)
                {
                    int SBsolverIndex = softbody.solverIndices[i];
                    int fluidSolverIndex = fluid.solverIndices[i];
                    softbody.solver.velocities[SBsolverIndex] = new Vector4(0,0,0,0);
                    fluid.solver.velocities[fluidSolverIndex] = new Vector4(0,0,0,0);
                }
                freezed = true;
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
                Debug.Log("ratio" + duration/ transitionDuration);
                freezed = false;
                duration = 0;
                transitionDuration = -1;
            }
            return;
        }

        // flatten out the direction so that it's parallel to the ground:
        direction.y = 0;

        // apply ground/air movement:
        float effectiveAcceleration = _acceleration;

        softbody.AddForce(direction.normalized * effectiveAcceleration, ForceMode.Acceleration);

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
