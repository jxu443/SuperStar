using System;
using System.Collections;
using System.Collections.Generic;
using Obi;
using UnityEngine;


public class CityRender : MonoBehaviour
{
    [SerializeField, Range(1, 20)] // make private var configurable
    private int width = 10;

    [SerializeField, Range(1, 20)]
    private int height = 10;
    
    [SerializeField]
    private Transform roadPrefab = null;
    
    [SerializeField]
    private Transform crossPrefab = null;
    
    [SerializeField]
    private Transform blockPrefab = null;

    [SerializeField, Range(1, 50)] // make private var configurable
    private float gridSize = 32;

    MazeGenerator gen;
    private WallState[,] maze;
    private bool toggle = false;
    
    
    private void Awake()
    {
        
    }

    void Start()
    {
        gen = new MazeGenerator(width, height);
        maze = gen.Generate();
        DrawMaze();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            Debug.Log("R is pressed");
            toggle = !toggle;
            DestroyMaze();
            DrawMaze();
        }
    }
    
    
    private async void DrawMaze() {
        WallState oneMask = WallState.LEFT | WallState.RIGHT | WallState.UP | WallState.DOWN | WallState.VISITED; //1111
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                var cell = maze[i,j] ;
                if (toggle) cell ^= oneMask; // flip the bit
                //Debug.Log(maze[i, j] + " " + cell);
                
                var position = new Vector3((-width/2 + i)*gridSize, 0, (-height/2 + j)*gridSize); // center the maze at (0,0)

                
                bool upWall = false;
                if (blockPrefab != null) {
                    var block = Instantiate(blockPrefab) as Transform;
                    block.position = position;
                    block.SetParent(transform, false);
                }
                
                if (cell.HasFlag(WallState.UP)) {
                    var wall = Instantiate(roadPrefab) as Transform;
                    wall.name = "up" + i + j;
                    wall.position = position + new Vector3(0, 0, 0.5f) * gridSize; 
                    wall.SetParent(transform, false);
                    
                    upWall = true;
                }
                if (cell.HasFlag(WallState.LEFT)) {
                    var wall = Instantiate(roadPrefab) as Transform;
                    wall.name = "left" + i + j;
                    wall.position = position + new Vector3(-0.5f, 0, 0) * gridSize; 
                    wall.eulerAngles = new Vector3(0, 90, 0);
                    wall.SetParent(transform, false);

                    if (upWall && crossPrefab != null)
                    {
                        var intersection = Instantiate(crossPrefab) as Transform;
                        intersection.name = "CROSS" + i + j;
                        intersection.position = position + new Vector3(-gridSize/2, 0, gridSize/2);
                        intersection.SetParent(transform, false);
                    }
        
                }
                
                if (i == width -1) {
                    if (cell.HasFlag(WallState.RIGHT)) {
                        var wall = Instantiate(roadPrefab) as Transform;
                        wall.name = "right" + i + j;
                        wall.position = position + new Vector3(0.5f, 0, 0) * gridSize;
                        wall.eulerAngles = new Vector3(0, 90, 0); 
                        wall.SetParent(transform, false); 
                        
                    }
                }

                if (j == 0) {
                    if (cell.HasFlag(WallState.DOWN)) {
                        var wall = Instantiate(roadPrefab) as Transform;
                        wall.name = "down" + i + j;
                        wall.position = position + new Vector3(0, 0, -0.5f)* gridSize; 
                        wall.SetParent(transform, false);
                    }
                }
                
            }
        }
    }

    void DestroyMaze()
    {
        Debug.Log("maze redraw");

        // destroy all children
        while (transform.childCount > 0) {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
}

