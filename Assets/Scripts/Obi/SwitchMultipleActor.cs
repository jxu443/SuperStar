using System;
using System.Linq;
using UnityEngine;

namespace Obi
{
    public class SwitchMultipleActor : MonoBehaviour
    {
        public MazeController mcMazeController = null;
        public ObiSoftbody softbody;
        public ObiParticleRenderer softBodyRenderer;
        
        public ObiEmitter fluid;
        public ObiParticleRenderer fluidRenderer;
        public bool SwitchParticleState = false;
        private float volumeScale = 1f;
        private bool currentState = false; // False是固态
        
        
        // fluid to softbody tryout, not used
        public float transitionDuration = 1f;
        public float reconstructVelocityMultiplier = 0.02f;
        private float duration;
        private bool transitioningToSolid = false;


        void Start()
        {
            Debug.Log(" start fluid.solverIndices.Length: " + fluid.solverIndices.Length); // 0
            Debug.Log(" start softbody.solverIndices.Length: " + softbody.solverIndices.Length); // 720
        }
        
        public float VolumeScale
        {
            get => volumeScale;
            set => volumeScale = value;
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
                        softbody.enabled = true; 
                        particleMapFromFluidToSoftbody();
                        
                        
                        // scale the softbody
                        Debug.Log("volumeScale: " + volumeScale);
                        transform.localScale = new Vector3(volumeScale, volumeScale, volumeScale);

                        fluid.enabled = false;
                        fluidRenderer.enabled = false;
                        softBodyRenderer.enabled = true;
                        
                        if (mcMazeController is not null) 
                            mcMazeController.enabled = false;
                        
                        var mc = softbody.GetComponent<MovementController>();
                        if (mc is not null) 
                            mc.enabled = true;
                        
                        Debug.Log("fluid to softbody");
                        currentState = false;
                    }
                    else
                    {
                        SoftbodyToFluid();
                    }
                    //currentState = !currentState;

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
                    Debug.Log("softbody to fluid");
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

        public void SoftbodyToFluid()
        {
            if (!currentState)
            {
                softbody.enabled = true;
                softBodyRenderer.enabled = false;
                
                var mc = softbody.GetComponent<MovementController>();
                if (mc is not null) 
                    mc.enabled = false;
                
                fluidRenderer.enabled = false;
                fluid.enabled = true;
                
                softbody.enabled = false;
                fluidRenderer.enabled = true;

                Debug.Log("softbody to fluid");
                currentState = true;
            }
        }
        
        // ignore the particles that are not in the container
        private void particleMapFromFluidToSoftbody()
        {
            
            for (int i = 0; i < fluid.solverIndices.Length; ++i)
            {
                int fluidSolverIndex = fluid.solverIndices[i];
                int SBsolverIndex = softbody.solverIndices[i];
            
                softbody.solver.positions[SBsolverIndex] = fluid.solver.positions[fluidSolverIndex];
                softbody.solver.velocities[SBsolverIndex] *= reconstructVelocityMultiplier;
            }

            // int prevValidIndex = -1;
            // int firstValidIndex = -1;
            // Debug.Log("xxxx fluid.solverIndices.Length is " + fluid.solverIndices.Length);
            // for (int i = 0; i < fluid.solverIndices.Length; ++i)
            // {
            //     int fluidSolverIndex = fluid.solverIndices[i];
            //     int SBsolverIndex = softbody.solverIndices[i];
            //     
            //     var curFluidPos = fluid.solver.positions[fluidSolverIndex];
            //     if (curFluidPos.y > -7f && curFluidPos.y < -5f ) // TODO: hardcode
            //     {
            //         if (firstValidIndex == -1)
            //         {
            //             firstValidIndex = i;
            //         }
            //         prevValidIndex = i;
            //         softbody.solver.positions[SBsolverIndex] = curFluidPos;
            //         softbody.solver.velocities[SBsolverIndex] *= reconstructVelocityMultiplier;
            //     } 
            //     else if (prevValidIndex != -1)
            //     {
            //         fluidSolverIndex = fluid.solverIndices[prevValidIndex];
            //
            //         softbody.solver.positions[SBsolverIndex] = fluid.solver.positions[fluidSolverIndex];
            //         softbody.solver.velocities[SBsolverIndex] *= reconstructVelocityMultiplier;
            //     }
            //     
            // }
            //
            // Debug.Log("firstValidIndex: " + firstValidIndex);
            //
            // for (int i = 0; i < firstValidIndex; ++i)
            // {
            //     int fluidSolverIndex = fluid.solverIndices[prevValidIndex];
            //     int SBsolverIndex = softbody.solverIndices[prevValidIndex];
            //     
            //     softbody.solver.positions[SBsolverIndex] = fluid.solver.positions[fluidSolverIndex];
            //     softbody.solver.velocities[SBsolverIndex] *= reconstructVelocityMultiplier;
            //     
            // }
            
        }

    }
}
