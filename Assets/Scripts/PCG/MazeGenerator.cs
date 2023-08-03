using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Flags] //allow a nice representation by the .ToString() method
public enum WallState {
    LEFT = 1,
    RIGHT = 2,
    UP = 4,
    DOWN = 8, 
    VISITED = 16, // 10000
}

public struct Position {
    public int X;
    public int Y;
}

public struct Neighbor {
    public Position pos;
    public WallState SharedWall;
}

public class MazeGenerator 
{
    public Vector2 entrance;
    int width;
    int height;
    WallState[,] maze;
    // Constructor
    
    public MazeGenerator(int w, int h) 
    {
        this.width = w;
        this.height = h;
        maze = new WallState[width, height];
    }
    public WallState[,] Generate() {
        //init
        WallState initial = WallState.LEFT | WallState.RIGHT | WallState.UP | WallState.DOWN; //1111
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                maze[i,j] = initial;  
            }
        }
        IterativeBackTracker();
        
        // safeguard for no exit or entrance
        var rng = new System.Random();
        int randX;
        // if (!hasExit) {
        //     randX = rng.Next(0, width);
        //     maze[randX, height-1] &= ~WallState.UP;
        //     entrance = new Vector2(randX, height-1);
        // } 
        if (!hasEntrance) {
            randX = rng.Next(0, width);
            maze[randX, 0] &= ~WallState.DOWN;
        }
        return maze;
    }
    
    //bool hasExit = false;
    bool hasEntrance = false;
    private void IterativeBackTracker() {
        var rng = new System.Random();
        var position = new Position{X = rng.Next(0, width), Y = rng.Next(0, height)};
        //Debug.Log("init pos is " + position.X + " and " + position.Y);
        maze[position.X, position.Y] |= WallState.VISITED;
        var stack = new Stack<Position>();
        stack.Push(position);
        List<Neighbor> ls;
        while (stack.Count != 0) {
            Position curr = stack.Pop();
            ls = getUnvisitedNeighbors(curr);

            //add exit and entrance
            if (ls.Count == 0) {
                // if (!hasExit && curr.Y + 1 == height) {
                //     maze[curr.X, curr.Y] &= ~WallState.UP; // remove the UP wall;
                //     hasExit = true;
                //     entrance = new Vector2(curr.X, curr.Y);
                // }
                if (!hasEntrance && curr.Y == 0) {
                    maze[curr.X, curr.Y] &= ~WallState.DOWN; // remove the DOWN wall;
                    hasEntrance = true;
                }
            }

            if (ls.Count > 0) {
                if (ls.Count > 1) stack.Push(curr);

                var randIndex = rng.Next(0, ls.Count);
                var randomNeighbour = ls[randIndex];

                var nPosition = randomNeighbour.pos;
                maze[curr.X, curr.Y] &= ~randomNeighbour.SharedWall;
                maze[nPosition.X, nPosition.Y] &= ~GetOppositeWall(randomNeighbour.SharedWall);
                maze[nPosition.X, nPosition.Y] |= WallState.VISITED;

                stack.Push(nPosition);
            }
        }
    }

    private WallState GetOppositeWall(WallState wall)
    {
        switch (wall)
        {
            case WallState.RIGHT: return WallState.LEFT;
            case WallState.LEFT: return WallState.RIGHT;
            case WallState.UP: return WallState.DOWN;
            case WallState.DOWN: return WallState.UP;
            default: return WallState.LEFT;
        }
    }
    private List<Neighbor> getUnvisitedNeighbors(Position p) { 
        // new list
        var list = new List<Neighbor>();
        if (p.X > 0) { //left
            if (!maze[p.X-1, p.Y].HasFlag(WallState.VISITED)) list.Add(new Neighbor{
                pos = new Position {
                    X = p.X-1,
                    Y = p.Y,
                },
                SharedWall = WallState.LEFT,
            });
        }
        if (p.Y > 0) { // down
            if (!maze[p.X, p.Y-1].HasFlag(WallState.VISITED)) list.Add(new Neighbor{
                pos = new Position {
                    X = p.X,
                    Y = p.Y-1,
                },
                SharedWall = WallState.DOWN,
                
            });
        }
        if (p.X < width -1) { // right
            if (!maze[p.X+1, p.Y].HasFlag(WallState.VISITED)) list.Add(new Neighbor{
                pos = new Position {
                    X = p.X+1,
                    Y = p.Y,
                },
                SharedWall = WallState.RIGHT,
                
            });
        }
        if (p.Y < height -1) { // up
            if (!maze[p.X, p.Y+1].HasFlag(WallState.VISITED)) list.Add(new Neighbor{
                pos = new Position {
                    X = p.X,
                    Y = p.Y+1,
                },
                SharedWall = WallState.UP,
            });
        }

        return list;
    }
}
