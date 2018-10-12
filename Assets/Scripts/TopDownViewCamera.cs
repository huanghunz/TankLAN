using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownViewCamera : MonoBehaviour {

    public static bool ENABLE_VIEW;
    public GameObject ViewUI;
    private Camera _topDownCam;

    private SetupMaze _maze;

    private void Awake()
    {
        _maze = FindObjectOfType<SetupMaze>();
        _topDownCam = this.GetComponent<Camera>();

        _topDownCam.orthographicSize = (int)(_maze.FinalScale * 1.5)+1;
    }

    private void Update()
    {
        this.SetVisible(ENABLE_VIEW);
    }

    public void SetVisible(bool visible)
    {
        this.ViewUI.SetActive(visible);
        _topDownCam.enabled = visible;
    }
}
