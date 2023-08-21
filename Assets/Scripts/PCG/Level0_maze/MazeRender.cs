using System;
using System.Collections;
using System.Collections.Generic;
using Obi;
using UnityEngine;


public class MazeRender : MonoBehaviour
{
    [SerializeField, Range(1, 20)] // make private var configurable
    private int width = 10;

    [SerializeField, Range(1, 20)]
    private int height = 10;
    
    [SerializeField]
    private ObiSoftbody player = null;

    public ObiSolver solverSoftbody;
    public ObiSolver solverFluid;
    
    [SerializeField]
    private GameObject catcher = null; 
        
    [SerializeField]
    private Transform wallPrefab = null;
    [SerializeField]
    private Transform entrancePrefab = null;
        
    [SerializeField]
    private Material backgroundMaterial = null;
    
    [SerializeField]
    private Material frontMaterial = null;
    
    public float functionDuration = 60f; // time to change the maze
    
    private float duration;
    int cnt; // how many times has reset

    MazeGenerator gen;
    

    void Start()
    {
        cnt = 0;
        gen = new MazeGenerator(width, height);
        DrawMaze(gen.Generate());
        
        // draw back plane
        var backPlane = GameObject.CreatePrimitive(PrimitiveType.Cube);
        backPlane.AddComponent<BoxCollider>();
        backPlane.AddComponent<ObiCollider>();
        backPlane.GetComponent<MeshRenderer>().material = backgroundMaterial;
        backPlane.transform.localRotation = Quaternion.Euler(-90, 0, 0);
        backPlane.transform.localPosition = new Vector3(0, -0.5f, 1.1f);
        backPlane.transform.localScale = new Vector3(width, 0.15f, height);
        backPlane.transform.SetParent(transform.parent, false);

        // draw front plane
        var frontPlane  = GameObject.Instantiate(backPlane);
        frontPlane.GetComponent<MeshRenderer>().material = frontMaterial;
        frontPlane.transform.localPosition =new Vector3(0, -0.5f, -0.1f);
        frontPlane.transform.SetParent(transform.parent, false);
        
        // maze catcher
        catcher.transform.position = new Vector3(width/2, -height/2 - 4, 0.2f);
        catcher.transform.localScale = new Vector3(width + 10f, 1, 3);
        catcher.SetActive(true);
        
        // player pos
        player.Teleport(new Vector3(-width/2 + 0.5f, height/2 - 1f , 0.6f), Quaternion.identity);
        //Invoke(nameof(movePlayer), .5f);
        
    }

    void movePlayer()
    {
        // solverFluid.transform.position = new Vector3(-width/2 + 0.5f, height/2 - 1f , 0.6f);
        // solverSoftbody.transform.position = new Vector3(-width/2 + 0.5f, height/2 - 1f , 0.6f);
        player.Teleport(new Vector3(-width/2 + 0.5f, height/2 - 1f , 0.6f), Quaternion.identity);

    }

    // void Update()
    // {
    //     duration += Time.deltaTime;
    //     int currCnt = (int) (duration / functionDuration);
    //     if (currCnt > cnt) {
    //         Debug.Log("maze redraw");
    //         cnt++;
    //         
    //         // destroy all children
    //         while (transform.childCount > 0) {
    //             DestroyImmediate(transform.GetChild(0).gameObject);
    //         }
    //         
    //         gen = new MazeGenerator(width, height);
    //         DrawMaze(gen.Generate()); 
    //         
    //     }
    //     
    // }

    private async void DrawMaze(WallState[,] maze) {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                var cell = maze[i,j];
                var position = new Vector3(-width/2 + i, 0, -height/2+j); // center the maze at (0,0)

                if (cell.HasFlag(WallState.UP)) {
                    var wall = Instantiate(wallPrefab) as Transform;
                    wall.position = position + new Vector3(0, 0, 0.5f); 
                    wall.SetParent(transform, false); 
                }
                if (cell.HasFlag(WallState.LEFT)) {
                    var wall = Instantiate(wallPrefab) as Transform;
                    wall.position = position + new Vector3(-0.5f, 0, 0); 
                    wall.eulerAngles = new Vector3(0, 90, 0);
                    wall.SetParent(transform, false); 
                }
                
                if (i == width -1) {
                    if (cell.HasFlag(WallState.RIGHT)) {
                        var wall = Instantiate(wallPrefab) as Transform;
                        wall.position = position + new Vector3(0.5f, 0, 0);
                        wall.eulerAngles = new Vector3(0, 90, 0); 
                        wall.SetParent(transform, false); 
                    }
                }

                if (j == 0) {
                    if (cell.HasFlag(WallState.DOWN)) {
                        var wall = Instantiate(wallPrefab) as Transform;
                        wall.position = position + new Vector3(0, 0, -0.5f); 
                        wall.SetParent(transform, false); 
                    }
                }
            }
        }
        
        // //draw phase change trigger
        // var entrance = Instantiate(entrancePrefab) as Transform;
        // //entrance.position = new Vector3( -width/2 + gen.entrance.x, 0, -height/2  + gen.entrance.y + 0.5f);
        // entrance.position = new Vector3( -width/2, 0, height/2 - 0.5f);
        // entrance.SetParent(transform, false); 
        
        
    }
}

