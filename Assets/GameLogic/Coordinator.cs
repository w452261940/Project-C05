using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SKCell;

public class Coordinator : MonoBehaviour
{

    public Vector3[] startPoints;

    void Start()
    {
        InitializeMap();
    }

    void InitializeMap()
    {
        startPoints = new Vector3[10];
        Transform startPointContainer = GameObject.Find("StartPoints").transform;
        for (int i = 0; i < startPointContainer.childCount; i++)
        {
            startPoints[i] = startPointContainer.GetChild(i).position;
        }
    }

}
