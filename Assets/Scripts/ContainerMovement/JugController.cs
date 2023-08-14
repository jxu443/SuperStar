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

    public FluidColorizer suger;
    public ObiEmitter emitter;
    public Text purityLabel;

    float angularSpeed = 0;
    float angle = 0;
    private int concentration = 0;
    
    HashSet<int> coloredParticles = new HashSet<int>();


    private void OnEnable()
    {
        Debug.Log("Jug controller is enabled");
    }

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
                    emitter.solver.userData[contact.bodyA] = suger.color;
                    
                    if (coloredParticles.Add(contact.bodyA))
                        UpdateScore(coloredParticles.Count, contact.bodyA);
                }
            }
        }
    }
    
    public void UpdateScore(int coloredParticles, int k)
    {
        concentration = Mathf.CeilToInt((coloredParticles / 720.0f) * 100);
        purityLabel.text = concentration + "% 溶解度";
        
        emitter.solver.colors[k] = emitter.solver.userData[k];
        Debug.Log("Suger color is " + suger.color);

        if (concentration >= 95)
        {
            Debug.Log("finished, concentration >= 95");
            for (int i = 0; i < emitter.solverIndices.Length; ++i)
            {
                int idx = emitter.solverIndices[i];
                //emitter.solver.colors[idx] = emitter.solver.userData[idx];
                emitter.solver.viscosities[idx] = 1f;
            }

            gameObject.GetComponent<JugController>().enabled = false;
            emitter.GetComponent<FluidMovement>().enabled = true;
            gameObject.SetActive(false);
        }
    }
    
    // void LateUpdate()
    // {
    //     for (int i = 0; i < emitter.solverIndices.Length; ++i)
    //     {
    //         int k = emitter.solverIndices[i];
    //         emitter.solver.colors[k] = emitter.solver.userData[k];
    //         emitter.solver.viscosities[k] = Mathf.Lerp(0.5f, 1f, concentration / 100f);
    //     }
    // }

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
