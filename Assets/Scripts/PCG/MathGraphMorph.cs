using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathGraphMorph : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform pointPrefab;
    [Range(10, 100)] public int resolution = 20;
    [SerializeField] FunctionLibrary.FunctionName function;

    [SerializeField, Min(0f)]
	float functionDuration = 1f, transitionDuration = 1f;
    public bool isAnimating = true;
    public bool isTrigger;
    bool transitioning = false;
    
    Transform[] points;
    float PI = Mathf.PI;
    float duration;
    FunctionLibrary.Function f; 
    FunctionLibrary.FunctionName functionOld;


    async void Awake() {
        points = new Transform[resolution * resolution];
        float step = 2f / resolution; 
        var scale = Vector3.one * step;
        //transform.position = new Vector3(-15, 0, 0); 
        for (int i = 0; i < points.Length; i++) {
            Transform point = points[i] = Instantiate(pointPrefab);
            point.localScale = scale;
			point.SetParent(transform, false);

        } 

    }
    void start() {
    
    }

    async void Update()
    {
        if (isAnimating) {
            f = FunctionLibrary.GetFunction(function);
            duration += Time.deltaTime;
		    if (!transitioning && duration >= functionDuration) {
                transitioning = true; // start transition
			    duration -= functionDuration;
                functionOld = function;
			    function = FunctionLibrary.GetRandomFunctionNameExcept(function);
		    } else if (transitioning && duration >= transitionDuration) {
                transitioning = false;
                duration -= transitionDuration;
            }
            
            float time = Time.time;
            float step = 2f / resolution; 

            float v = (0 + 0.5f) * step - 1f;
            for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++) { 
                // the logic behind is to loop over points instead of dimentions
			    if (x == resolution) {
				    x = 0;
				    z += 1;
                    v = (z + 0.5f) * step - 1f;
			    }
                float u = (x + 0.5f) * step - 1f;
                
                if (!transitioning) {
			        points[i].localPosition = f(u, v, time);
                } else {
                    points[i].localPosition = FunctionLibrary.Morph(u, v, time, 
                        FunctionLibrary.GetFunction(functionOld), f,  duration/transitionDuration);
                }
                if (!isTrigger) {
                    points[i].GetComponent<Collider>().isTrigger = false;
                }
            }
        }
    }

    public void toggleIsAnimating() {
        isAnimating = !isAnimating;
    }

    public void NotIsTrigger() {
        Debug.Log("NotIsTrigger is called");
        isTrigger = false;
    }
    
}
