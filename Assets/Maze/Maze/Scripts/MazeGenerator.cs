using UnityEngine;
using System.Collections;

namespace MyMaze
{
    public class MazeGenerator : MonoBehaviour
    {
        public delegate void FinishGeneration();
        public event FinishGeneration OnFinishGeneration;

        public IntVector2 mazeSize;

        public float FinalScale;

        public bool generateOnStart = false;

        private Maze _mazeInstance;

        void Start()
        {
            if (generateOnStart)
            {
                this.Generate();
            }
        }

        void OnDestroy()
        {
            if (_mazeInstance != null)
            {
                _mazeInstance.OnMazeCreated -= this.OnMazeCreated;
            }
        }

        public MazeCell[,] GetMazPositions()
        {
            return _mazeInstance.GetAllCell();
        }

        public void Generate()
        {
            var go = Instantiate(Resources.Load("Prefabs/Maze")) as GameObject;

            _mazeInstance = go.GetComponent<Maze>();

            _mazeInstance.size = mazeSize;
            _mazeInstance.OnMazeCreated += this.OnMazeCreated;

            go.transform.SetParent(this.transform);
            go.transform.localPosition = Vector3.one;
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.identity;

            _mazeInstance.GenerateInstantly();
           // StartCoroutine(_mazeInstance.Generate());
        }

        private void OnMazeCreated()
        {
            StartCoroutine(this.ChangeSize());
        }

        private IEnumerator ChangeSize()
        {
            float t = 0;
            Vector3 finalScale = Vector3.one * this.FinalScale;
            Vector3 finalPos = new Vector3(0, -this.FinalScale, 0);
            while (t < 1f)
            {
                t += Time.deltaTime / 0.5f;

                this.transform.localScale = Vector3.Lerp(this.transform.localScale, finalScale, t);
                this.transform.position = Vector3.Lerp(this.transform.position, finalPos, t);

                yield return new WaitForEndOfFrame();
            }

            this.transform.localScale = finalScale;
            this.transform.position = finalPos;

            if (this.OnFinishGeneration != null)
            {
                Debug.Log("Finished Maze Creation");
                this.OnFinishGeneration();
            }
        }

        private void RestartGame()
        {
            StopAllCoroutines();
            _mazeInstance.OnMazeCreated -= this.OnMazeCreated;
            Destroy(_mazeInstance.gameObject);
            Generate();
        }
    }
}