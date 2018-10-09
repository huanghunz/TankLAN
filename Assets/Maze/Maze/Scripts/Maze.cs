﻿using UnityEngine;
using System.Collections;

using System.Collections.Generic;

namespace MyMaze
{
    public class Maze : MonoBehaviour
    {
        public bool Enable = false;

        public delegate void MazeCreated();
        public event MazeCreated OnMazeCreated;

        public IntVector2 size;

        public MazeCell cellPrefab;
        public MazePassage passagePrefab;
        public MazeWall wallPrefab;

        public float generationStepDelay = 0.000f;

        private MazeCell[,] _cells;

        public MazeCell[,] GetAllCell()
        {
            return _cells;
        }

        public MazeCell GetCell(IntVector2 coordinates)
        {
            return _cells[coordinates.x, coordinates.z];
        }

        public IntVector2 RandomCoordinates
        {
            get
            {
                return new IntVector2(Random.Range(0, size.x), Random.Range(0, size.z));
            }
        }

        public bool ContainsCoordinates(IntVector2 coordinate)
        {
            return coordinate.x >= 0 && coordinate.x < size.x && coordinate.z >= 0 && coordinate.z < size.z;
        }


        public void GenerateInstantly()
        {
            _cells = new MazeCell[size.x, size.z];
            List<MazeCell> activeCells = new List<MazeCell>();
            DoFirstGenerationStep(activeCells);
            while (activeCells.Count > 0)
            {
                DoNextGenerationStep(activeCells);
            }

            if (this.OnMazeCreated != null)
            {
                this.OnMazeCreated();
            }
        }

        public IEnumerator GenerateEnumerator()
        {
            WaitForSeconds delay = new WaitForSeconds(0);
            _cells = new MazeCell[size.x, size.z];
            List<MazeCell> activeCells = new List<MazeCell>();
            DoFirstGenerationStep(activeCells);
            while (activeCells.Count > 0)
            {
                yield return delay;
                DoNextGenerationStep(activeCells);
            }

            if (this.OnMazeCreated != null)
            {
                this.OnMazeCreated();
            }
        }

        private void DoFirstGenerationStep(List<MazeCell> activeCells)
        {
            // Generate first random cell
            activeCells.Add(CreateCell(RandomCoordinates));
        }

        private void DoNextGenerationStep(List<MazeCell> activeCells)
        {
            int currentIndex = activeCells.Count - 1;
            MazeCell currentCell = activeCells[currentIndex];
            if (currentCell.IsFullyInitialized)
            {
                activeCells.RemoveAt(currentIndex);
                return;
            }

            MazeDirection direction = currentCell.RandomUninitializedDirection;
            IntVector2 coordinates = currentCell.coordinates + direction.ToIntVector2();
            if (ContainsCoordinates(coordinates))
            {
                MazeCell neighbor = GetCell(coordinates);
                if (neighbor == null)
                {
                    neighbor = CreateCell(coordinates);
                    CreatePassage(currentCell, neighbor, direction);
                    activeCells.Add(neighbor);
                }
                else
                {
                    CreateWall(currentCell, neighbor, direction);
                    // No longer remove the cell here.
                }
            }
            else
            {
                CreateWall(currentCell, null, direction);
                // No longer remove the cell here.
            }
        }

        private MazeCell CreateCell(IntVector2 coordinates)
        {
            MazeCell newCell = Instantiate(cellPrefab) as MazeCell;
            _cells[coordinates.x, coordinates.z] = newCell;
            newCell.coordinates = coordinates;
            newCell.name = "Maze Cell " + coordinates.x + ", " + coordinates.z;
            newCell.transform.parent = transform;
            newCell.transform.localPosition =
                new Vector3(coordinates.x - size.x * 0.5f + 0.5f, 0f, coordinates.z - size.z * 0.5f + 0.5f);
            return newCell;
        }

        private void CreatePassage(MazeCell cell, MazeCell otherCell, MazeDirection direction)
        {
            MazePassage passage = Instantiate(passagePrefab) as MazePassage;
            passage.Initialize(cell, otherCell, direction);
            passage = Instantiate(passagePrefab) as MazePassage;
            passage.Initialize(otherCell, cell, direction.GetOpposite());
        }

        private void CreateWall(MazeCell cell, MazeCell otherCell, MazeDirection direction)
        {
            MazeWall wall = Instantiate(wallPrefab) as MazeWall;
            wall.Initialize(cell, otherCell, direction);
            if (otherCell != null)
            {
                wall = Instantiate(wallPrefab) as MazeWall;
                wall.Initialize(otherCell, cell, direction.GetOpposite());
            }
        }
    }
}