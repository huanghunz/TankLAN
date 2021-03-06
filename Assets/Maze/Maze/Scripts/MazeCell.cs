﻿using UnityEngine;
using System.Collections;

namespace MyMaze
{
    public class MazeCell : MonoBehaviour
    {

        public IntVector2 coordinates;

        private MazeCellEdge[] edges = new MazeCellEdge[MazeDirections.Count];


        private int initializedEdgeCount;

        public bool IsFullyInitialized
        {
            get
            {
                return initializedEdgeCount == MazeDirections.Count;
            }
        }

        public MazeDirection RandomUninitializedDirection
        {
            get
            {
                int skips = Random.Range(0, MazeDirections.Count - initializedEdgeCount);
                for (int i = 0; i < MazeDirections.Count; i++)
                {
                    if (edges[i] == null)
                    {
                        if (skips == 0)
                        {
                            return (MazeDirection)i;
                        }
                        skips -= 1;
                    }
                }
                throw new System.InvalidOperationException("MazeCell has no uninitialized directions left.");
            }
        }

        public void SetEdge(MazeDirection direction, MazeCellEdge edge)
        {
            if (IsFullyInitialized)
            {
                Debug.LogWarning("Cell is four way connected. Over writing direction: " + direction);
            }

            edges[(int)direction] = edge;
            initializedEdgeCount += 1;
        }

        public MazeCellEdge GetEdge(MazeDirection direction)
        {
            return edges[(int)direction];
        }
    }
}