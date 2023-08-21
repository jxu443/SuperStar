
using UnityEngine;
using UnityEngine.SceneManagement;
using Obi;

public class SceneLoader : MonoBehaviour
{
    
    public string sceneName;
    public ObiSolver softbodySolver = null;
    public ObiCollider nextLevelCollider = null;
   
    void Start()
    {
        softbodySolver.OnCollision += Solver_OnCollision;
    }

    private void OnDestroy()
    {
        softbodySolver.OnCollision -= Solver_OnCollision;
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
                if (nextLevelCollider == col)
                {
                    Debug.Log("Obi collider Load Scene");
                    SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Level Trigger");
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Load Scene");
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
       
    }
}
