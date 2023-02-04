using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SKCell;

public class Coordinator : MonoBehaviour
{
    public LevelState levelState = LevelState.Ready;
    public Vector3[] startPoints;

    public Animator p1CFAnim, p2CFAnim, startScreenAnim;
    bool player0Ready = false;
    bool player1Ready = false;

    PlayerControl player0, player1;
    void Awake()
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
            startPointContainer.GetChild(i).gameObject.SetActive(false);
        }

        GameObject r0 = GameObject.Find("Root_0");
        GameObject r1 = GameObject.Find("Root_1");
        player0 = r0.GetComponent<PlayerControl>();
        player1 = r1.GetComponent<PlayerControl>();
        r0.transform.position = new Vector3( startPoints[0].x, startPoints[0].y, r0.transform.position.z);
        r1.transform.position = new Vector3( startPoints[1].x, startPoints[1].y, r1.transform.position.z);
    }

    private void Update()
    {
        if(levelState == LevelState.Ready)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                PlayerReady(0);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                PlayerReady(1);
            }
        }
    }

    public void PlayerReady(int index)
    {
        if(index == 0)
        {
            player0Ready = true;
            p1CFAnim.Play("CFIN");
        }
        else
        {
            player1Ready = true;
            p2CFAnim.Play("CFIN");
        }
        if(player0Ready && player1Ready)
        {
            CommonUtils.InvokeAction(1f,  StartLevel);
        }
    }

    public void StartLevel()
    {
        levelState = LevelState.Start;
        startScreenAnim.Play("StartLevelOut");

        player0.active = true;
        player1.active = true;
    }

}

public enum LevelState
{
    Ready,
    Start
}
