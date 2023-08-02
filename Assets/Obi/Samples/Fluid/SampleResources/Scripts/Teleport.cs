using UnityEngine;

namespace Obi
{
    public class Teleport: MonoBehaviour
    {
        public ObiSolver solver;
        public ObiEmitter emitter;
        void Update()
        {
            if (Input.GetKey(KeyCode.L))
            {
                
                // solver.positions[emitter.solverIndices[index]];
                // solver.actors[0].transform.position = new Vector3(0, 0, 0);
            }
                
        }
    }
}