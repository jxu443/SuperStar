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

    private bool freezed = false;
    private float duration = 0;
    private float transitionDuration = -1;

    void Start()
    {
        Debug.Log(" fluid.solverIndices.Length: " + fluid.solverIndices.Length);
        Debug.Log(" softbody.solverIndices.Length: " + softbody.solverIndices.Length);
    }

    private void OnEnable()
    {
        //Debug.Log("OnEnable");
        
        for (int i = 0; i < fluid.solverIndices.Length; ++i)
        {
            int fluidSolverIndex = fluid.solverIndices[i];
            int len = softbody.solverIndices.Length;
            int SBsolverIndex = softbody.solverIndices[i % len];

            fluid.solver.positions[fluidSolverIndex] = softbody.solver.positions[SBsolverIndex];
            fluid.solver.velocities[fluidSolverIndex] = softbody.solver.velocities[SBsolverIndex];
        }
        
    }
    
    private void OnDisable()
    {
        //Debug.Log("OnDisable");
        freezed = false;
        duration = 0;
        transitionDuration = -1;
    }


    void Update()
    {
        bool keyPress = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) 
                        || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.Space);

        if (!keyPress)
        {
            if (!freezed && softbody.solver.velocities[0].magnitude < 2f)
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
