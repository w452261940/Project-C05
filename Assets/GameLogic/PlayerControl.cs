using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float updateInterval = 0.1f;
    public float speed = 1.0f;
    public float angleForce = 0.4f;
    public float tolerance = 0.03f;


    public LineRenderer line;
    private float axis_x, axis_y;

    private float updateTimer = 0f;
    private float pos_delta = 0f;
    private Vector3 lastPos;
    private Vector3 direction;

    private int pCount = 0;
    void Start()
    {
        lastPos = transform.position;
    }
    void Update()
    {
        UpdateInput();
        UpdateLine();
    }

    private void UpdateInput()
    {
        axis_x = Input.GetAxis("Horizontal");
        axis_y = Input.GetAxis("Vertical");
    }

    void UpdateLine()
    {
        transform.position += new Vector3(axis_x + direction.x, axis_y+ direction.y, 0) * speed * Time.fixedDeltaTime;
        updateTimer += Time.fixedDeltaTime;
        if (pos_delta>0f && updateTimer > updateInterval)
        {
            int count = line.positionCount++;
            line.SetPosition(count, transform.position);
            updateTimer = 0f;
            pCount++;
            if(pCount>0 && pCount%60==0)
            line.Simplify(tolerance);
        }
        pos_delta = Vector3.Distance(transform.position, lastPos);
        direction =( transform.position - lastPos).normalized;
        lastPos = transform.position;
    }
}
