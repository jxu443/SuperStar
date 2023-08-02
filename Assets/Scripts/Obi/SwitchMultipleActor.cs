using System;
using UnityEngine;

namespace Obi
{
    public class SwitchMultipleActor : MonoBehaviour
    {
        public ObiSoftbody softbody;
        public ObiParticleRenderer softBodyRenderer;
        
        public ObiEmitter fluid;
        public ObiParticleRenderer fluidRenderer;
        public bool SwitchParticleState = false;
        private bool currentState = false; // False是固态

        public float transitionDuration = 1f;
        public float reconstructVelocityMultiplier = 0.05f;
        private float duration;
        private bool transitioningToSolid = false;

        void Start()
        {
            Debug.Log(" fluid.solverIndices.Length: " + fluid.solverIndices.Length);
            Debug.Log(" softbody.solverIndices.Length: " + softbody.solverIndices.Length);
        }
        

        private void Update()
        {
            if (SwitchParticleState)
            {
                SwitchParticleState = false;
                if (null != softbody && null != fluid && null != softBodyRenderer && null != fluidRenderer)
                {
                    if (currentState)
                    {
                        //transitioningToSolid = true;

                        softbody.enabled = true;
                        for (int i = 0; i < fluid.solverIndices.Length; ++i)
                        {
                            int fluidSolverIndex = fluid.solverIndices[i];
                            int SBsolverIndex = softbody.solverIndices[i];

                            softbody.solver.positions[SBsolverIndex] = fluid.solver.positions[fluidSolverIndex];
                            softbody.solver.velocities[SBsolverIndex] *= reconstructVelocityMultiplier;
                        }
                        
                        fluid.enabled = false;
                        fluidRenderer.enabled = false;
                        softBodyRenderer.enabled = true;
                        Debug.Log("切换粒子为固态");
                        
                        // softbody.solver.velocities.Clear();
                        // fluid.SetSelfCollisions(false);
                        // fluid.surfaceCollisions = false;

                        // softBodyRenderer.enabled = true;
                        // Debug.Log("切换粒子为固态");

                        //Debug.Log(" fluid.solverIndices.Length: " + fluid.solverIndices.Length);
                        //Debug.Log(" softbody.solverIndices.Length: " + softbody.solverIndices.Length);

                    }
                    else
                    {
                        softbody.enabled = true;
                        softBodyRenderer.enabled = false;
                        fluidRenderer.enabled = false;
                        fluid.enabled = true;
                        //fluid.ReEmitShape();
                        softbody.enabled = false;
                        fluidRenderer.enabled = true;
                        Debug.Log("切换粒子为液态");

                    }
                    currentState = !currentState;
                }
            }

            
            // lerp from fluid to softbody, but not used
            if (transitioningToSolid)
            {
                if (duration >= transitionDuration)
                {
                    transitioningToSolid = false;
                    duration -= transitionDuration;
                    
                    fluid.surfaceCollisions = true;
                    fluid.SetSelfCollisions(true);
                    fluid.enabled = false;
                    fluidRenderer.enabled = false;

                    softBodyRenderer.enabled = true;
                    Debug.Log("切换粒子为固态");
                    return;
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
            }
        }
    }
}