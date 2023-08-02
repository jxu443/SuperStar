using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FunctionLibrary {

    public delegate Vector3 Function(float x, float z, float t); 
    // delegate: reference to methods with a particular parameter list and return type
    public enum FunctionName { Wave, MultiWave, Ripple, Sphere, Torus}
    static Function[] functions = { Wave, MultiWave, Ripple, Sphere, Torus };

    static float PI = Mathf.PI;

    public static Vector3 Morph (
		float u, float v, float t, Function from, Function to, float progress
	) {
        // Lerp scales to [0,1] which we are already is, so Unclamped
        return Vector3.LerpUnclamped(from(u, v, t), to(u, v, t), Mathf.SmoothStep(0f, 1f, progress));
    }
	
    public static FunctionName GetRandomFunctionNameExcept (FunctionName curr) {
		int choice = (int) Random.Range(1, functions.Length);
		return choice == (int) curr? 0: (FunctionName)choice;
	}

	public static Function GetFunction (FunctionName name) {
		return functions[(int)name];
	}

    public static Vector3 Wave (float u, float v, float t) {
        Vector3 p;
        p.x = u;
        p.y = Mathf.Sin(Mathf.PI * (u + v + t));
        p.z = v;
        return p;
	}

    public static Vector3 MultiWave(float u, float v, float t) {
        Vector3 p;
		p.x = u;
		p.y = Mathf.Sin(Mathf.PI * (u + 0.5f * t));
		p.y += 0.5f * Mathf.Sin(2f * Mathf.PI * (v + t));
		p.y += Mathf.Sin(Mathf.PI * (u + v + 0.25f * t));
		p.y *= 1f / 2.5f;
		p.z = v;
		return p;
    }

    public static Vector3 Ripple (float u, float v, float t) {
		float d = Mathf.Sqrt(u * u + v * v);
		Vector3 p;
		p.x = u;
		p.y = Mathf.Sin(Mathf.PI * (4f * d - t));
		p.y /= 1f + 10f * d;
		p.z = v;
		return p;
	}

    public static Vector3 Sphere (float u, float v, float t) {
        Vector3 p;

        // normal Sphere 
        // float r = 1f;
        // float s = r * Mathf.Cos(0.5f * PI * v);
        
        // Scaling sphere.
        //float r = 0.5f + 0.5f * Mathf.Sin(PI * t); // radius in [0,1] 
		//float s = r * Mathf.Cos(0.5f * PI * v); // // on [-0.5, 0.5] cos is positive and max at 0
		
        // Sphere with vertical bands
        // float r = 0.9f + 0.1f * Mathf.Sin(8f * PI * u);
        // float s = r * Mathf.Cos(0.5f * PI * v);

        // Sphere with horizontal bands
        // float r = 0.9f + 0.1f * Mathf.Sin(8f * PI * v);
        // float s = r * Mathf.Cos(0.5f * PI * v);

        // Rotating twisted sphere.
        float r = 0.9f + 0.1f * Mathf.Sin(PI * (6f * u + 4f* v + t));
        float s = r * Mathf.Cos(0.5f * PI * v);

        p.x = s * Mathf.Sin(PI * u);
		p.y = r * Mathf.Sin(PI * 0.5f * v);
		p.z = s * Mathf.Cos(PI * u);
		return p;
	}

    public static Vector3 Torus(float u, float v, float t) {
        Vector3 p;
        // Torus 
        // float r1 = 0.75f; // major r
        // float r2 = 0.25f; // r1 + r2 = 1;

        float r1 = 0.7f + 0.1f * Mathf.Sin(PI * (6f * u + 0.5f * t));
		float r2 = 0.15f + 0.05f * Mathf.Sin(PI * (8f * u + 4f * v + 2f * t));
        float s = r1 + r2 * Mathf.Cos(PI * v); // pulling its vertical half-circles away 

        p.x = s * Mathf.Sin(PI * u);
		p.y = r2 * Mathf.Sin(PI * v); // instead of Sin( 0.5 * PI * v)
		p.z = s * Mathf.Cos(PI * u);
		return p;
    }
    
}