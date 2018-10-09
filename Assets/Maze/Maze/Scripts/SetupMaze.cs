using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyMaze;
using UnityEngine.Networking;
using System.Runtime.InteropServices;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Object = System.Object;

public class SetupMaze : MonoBehaviour {

    public delegate void FinishGeneration();
    public event FinishGeneration OnFinishGeneration;

    public GameObject MazePrefab;
    public IntVector2 Size;
    private Maze _mazeInstance;

    
    public float FinalScale;

    public bool generateOnStart = false;

    public void CreateMaze()
    {
        var go = Instantiate(MazePrefab) as GameObject;

        _mazeInstance = go.GetComponent<Maze>();
        _mazeInstance.size = Size;
       // 

        go.transform.SetParent(this.transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.transform.localRotation = Quaternion.identity;

        //this.transform.localScale = Vector3.one * this.FinalScale;
        //_mazeInstance.GenerateInstantly();
        StartCoroutine(_mazeInstance.GenerateEnumerator());

        //StartCoroutine(this.ChangeSize());

        _mazeInstance.OnMazeCreated += this.SpwanServerObjects;
        //this.SpwanServerObject();
    }


    void OnDestroy()
    {
        if (_mazeInstance != null)
        {
            _mazeInstance.OnMazeCreated -= this.SpwanServerObjects;
            Destroy(_mazeInstance.gameObject);
        }
    }

    private IEnumerator ChangeSize()
    {
        float t = 0;
        Vector3 finalScale = Vector3.one * this.FinalScale;
        while (t < 1f)
        {
            t += Time.deltaTime;

            this.transform.localScale = Vector3.Lerp(this.transform.localScale, finalScale, t);

            yield return new WaitForEndOfFrame();
        }

        this.transform.localScale = finalScale;

        GameObject go = this.gameObject;
        NetworkServer.Spawn(go);
        NetworkTransform[] allChildren = go.GetComponentsInChildren<NetworkTransform>();

        foreach (NetworkTransform child in allChildren)
        {
            NetworkServer.Spawn(child.gameObject);
            child.transform.SetParent(_mazeInstance.transform);
        }

        if (this.OnFinishGeneration != null)
        {
            this.OnFinishGeneration();
        }
    }

    private void SpwanServerObjects()
    {
        StartCoroutine(this.ChangeSize());
    }
}
