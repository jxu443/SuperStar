using System;
using System.Collections;
using System.Collections.Generic;
using Obi;
using UnityEngine;
using UnityEngine.UI;

public class JugController : MonoBehaviour
{
    public Transform referenceFrame;
    public float angularAcceleration = 5;

    [Range(0, 1)]
    public float angularDrag = 0.2f;

    float angularSpeed = 0;
    float angle = 0;

    
    public FluidColorizer suger;
    public ObiEmitter emitter;
    public Text purityLabel;
    public GameObject jug;
    public PhaseChange phaseChange;
    public ThirdPersonCam thirdPersonCam;
    public GameObject respawn;
    public ObiCollider counter;
    public ObiSoftbody softbody;
    
    HashSet<int> coloredParticles = new HashSet<int>();
    private int concentration = 0;
    
    HashSet<int> totalParticles = new HashSet<int>();
    private int totalParticle = 720;
    
    void Start()
    {
        emitter.solver.OnCollision += Solver_OnCollision;
    }

    private void OnDestroy()
    {
        emitter.solver.OnCollision -= Solver_OnCollision;
    }
    
    
    private void Solver_OnCollision(ObiSolver s, ObiSolver.ObiCollisionEventArgs e)
    {
        var world = ObiColliderWorld.GetInstance();
        foreach (Oni.Contact contact in e.contacts)
        {
            if (contact.distance < 0.01f)
            {
                var col = world.colliderHandles[contact.bodyB].owner;
                if (suger.collider == col)
                {
                    respawn.SetActive(false);
                    purityLabel.gameObject.SetActive(true);
                    thirdPersonCam.SwitchCameraStyle(ThirdPersonCam.CameraStyle.FixedJug);
                    
                    emitter.solver.userData[contact.bodyA] = suger.color;
                    if (coloredParticles.Add(contact.bodyA))
                        UpdateScore(coloredParticles.Count, contact.bodyA);
                }

                if (counter == col)
                {
                    if(totalParticles.Add(contact.bodyA))
                        totalParticle = totalParticles.Count;
                }
            } 
        }
    }
    
    public void UpdateScore(int coloredParticles, int k)
    {
        concentration = Mathf.CeilToInt((coloredParticles / (float)totalParticle) * 100f);
        purityLabel.text = concentration + "% 溶解度, 请继续搅拌至90%以上";
        
        emitter.solver.colors[k] = emitter.solver.userData[k];

        if (concentration >= 90)
        {
            for (int i = 0; i < emitter.solverIndices.Length; ++i)
            {
                int idx = emitter.solverIndices[i];
                //emitter.solver.colors[idx] = emitter.solver.userData[idx];
                emitter.solver.viscosities[idx] = 1f;
            }
            
            
            gameObject.GetComponent<JugController>().enabled = false;
            emitter.GetComponent<FluidMovement>().enabled = true;
            
            thirdPersonCam.SwitchCameraStyle(ThirdPersonCam.CameraStyle.Basic);
            purityLabel.gameObject.SetActive(false);
            jug.SetActive(false);
            phaseChange.SwitchPhase();
            
            softbody.Teleport(this.transform.position + new Vector3(0,2,0), Quaternion.Inverse(Quaternion.identity));
            
            
        }
    }
    

    void Update()
    {
        Vector3 dir = Vector3.zero;
        if (Input.GetKey(KeyCode.A))
        {
            dir = referenceFrame.right;
            angularSpeed += angularAcceleration * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            dir = referenceFrame.right;
            angularSpeed -= angularAcceleration * Time.deltaTime;
        }
        
        transform.Translate(dir * (Time.deltaTime * angularSpeed * 0.1f));
        
        angularSpeed *= Mathf.Pow(1 - angularDrag, Time.deltaTime);
        angle += angularSpeed * Time.deltaTime;
        transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.up);
        
    }
}
