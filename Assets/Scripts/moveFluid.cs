// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Obi;
// using System;
//
// using Unity.Burst;
// using Unity.Collections;
// using Unity.Jobs;
// using static Unity.Mathematics.math;
//
// public class moveFluid : MonoBehaviour
// {
//     public Transform _referenceFrame;
//     public float _acceleration;
//     public ObiEmitter fluid;
//     public ObiSolver solver;
//     //public Rigidbody container;
//     
//     // hash
//     public int seed;
//     NativeArray<uint> hashes;
//     ComputeBuffer hashesBuffer;
//
//     [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
//     struct HashJob : IJobFor { // IJobFor interface, which means that the job is going to run in parallel for each index in the array.
//
//         [WriteOnly] 
//         public NativeArray<uint> hashes;
//
//         public int resolution;
//         public float invResolution;
//         public SmallXXHash hash;
//         
//         public void Execute(int i) {
//             // float v = floor(invResolution * i + 0.00001f); // use integer division instead? no, int div cannot be vectorized, a lot less efficient
//             // float u = i - resolution * v;
//             // hashes[i] = (uint)(frac(u*v * 0.381f) * 256f); //  (uint)i;
// 			
//             float vf = floor(invResolution * i + 0.00001f);
//             float uf = invResolution * (i - resolution * vf + 0.5f) - 0.5f;
//             vf = invResolution * (vf + 0.5f) - 0.5f;
//
//             int u = (int)floor(uf * 32f / 4f);
//             int v = (int)floor(vf * 32f / 4f);
//
//             // method chaining
//             hashes[i] = hash.Eat(u).Eat(v);	
//         }
//     }
//     
//     void Start()
//     {
//         // Debug.Log("fluid.solverIndices.Length " + fluid.solverIndices.Length);
//         //
//         // int solverIndex = fluid.solverIndices[0];
//         // var vel = solver.velocities[solverIndex];
//         // Debug.Log("vel " + vel);
//
//         Debug.Log(" fluid particle len is " + fluid.solverIndices.Length);
//
//         // int resolution = 40;
//         // int length = fluid.solverIndices.Length;
//         // hashes = new NativeArray<uint>(length, Allocator.Persistent); 
//         // hashesBuffer = new ComputeBuffer(length, 4); // (number of elements, stride)
//         // new HashJob {
//         //     hashes = hashes,
//         //     resolution = resolution,
//         //     invResolution = 1f / resolution,
//         //     hash = SmallXXHash.Seed(seed)
//         // }.ScheduleParallel(hashes.Length, resolution, default).Complete();
//         
//     }
//
//     // Update is called once per frame
//     void Update()
//     {
//         bool rightPressed = false;
//         Vector3 direction = Vector3.zero;
//
//         // Determine movement direction:
//         if (Input.GetKey(KeyCode.W))
//         {
//             direction += _referenceFrame.forward * _acceleration;
//         }
//         if (Input.GetKey(KeyCode.A))
//         {
//             direction += -_referenceFrame.right * _acceleration;
//         }
//         if (Input.GetKey(KeyCode.S))
//         {
//             direction += -_referenceFrame.forward * _acceleration;
//         }
//         if (Input.GetKey(KeyCode.D))
//         {
//             direction += _referenceFrame.right * _acceleration;
//             rightPressed = true;
//         }
//         
//         if (direction == Vector3.zero) return;
//
//         // flatten out the direction so that it's parallel to the ground:
//         direction.y = 0;
//
//         
//         // move independently
//         for (int i = 0; i < fluid.solverIndices.Length; ++i) {
//             var rand = new System.Random();
//             var randOffset = new Vector3((float)rand.NextDouble() * 2.0f - 1.0f, (float)rand.NextDouble() * 2.0f - 1.0f, (float)rand.NextDouble() * 2.0f - 1.0f);
//             
//             // retrieve the particle index in the solver:
//             int solverIndex = fluid.solverIndices[i];
//             var dir = direction.normalized + randOffset.normalized * 0.2f;
//             solver.velocities[solverIndex] = dir.normalized * _acceleration;
//         
//         }
//         
//         // more container: by AddForce (no ccd) + obi advection (randomness)
//         //container.AddForce(direction.normalized * _acceleration, ForceMode.Acceleration);
//         
//         if (rightPressed)
//         {
//             Debug.Log("rightPressed, [0] vel is " +  solver.velocities[0]);
//         }
//     }
// }
