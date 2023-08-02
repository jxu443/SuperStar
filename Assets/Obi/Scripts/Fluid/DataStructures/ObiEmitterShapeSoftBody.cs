using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Obi
{
    [AddComponentMenu("Physics/Obi/Emitter shapes/From SoftBody", 870)]
    [ExecuteInEditMode]
    public class ObiEmitterShapeSoftBody : ObiEmitterShape
    {
        public ObiSoftbody softBody;

        public override void GenerateDistribution()
        {
            if (null != softBody)
            {
                distribution.Clear();
                var solver = softBody.solver;
                for (int i = 0; i < softBody.particleCount; i++)
                {
                    var solverIndex = softBody.solverIndices[i];
                    var solverPos = softBody.solver.positions[solverIndex];
                    var solverQuat = softBody.solver.orientations[solverIndex];
                    distribution.Add(new DistributionPoint(solverPos, Vector3.zero));
                }
            }
        }
    }
}