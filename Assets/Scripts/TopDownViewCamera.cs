using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownViewCamera : MonoBehaviour {

    public bool EnableView;
    public GameObject ViewUI;
    public SetupMaze Maze;

    private Camera _topDownCam;

    private void Awake()
    {
        _topDownCam = this.GetComponent<Camera>();
        if (Maze == null)
        {
            return;
        }
        _topDownCam.orthographicSize = (int)(Maze.FinalScale * 1.5) + 1;
    }

    private void Update()
    {
        if (_topDownCam.enabled != EnableView)
        {
            this.SetVisible(EnableView);
        }
    }

    public void SetVisible(bool visible)
    {
        this.ViewUI.SetActive(visible);
        _topDownCam.enabled = visible;
    }
}
