using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SKCell;

public class Branch : MonoBehaviour
{
    public string procedureID;

    public PlayerControl playerControl; 
    public PolygonCollider2D cld;
    public bool active = true;

    Vector3[] positions;

    private LineRenderer line;
    private float timer = 0;
    private void Awake()
    {
        line = GetComponent<LineRenderer>();    
    }
    private void Update()
    {
        if (active)
        {
            if (!playerControl)
                return;

            timer += Time.fixedDeltaTime;
            if (timer > 5f)
            {
                line.Simplify(playerControl.tolerance);
                timer = 0;
            }
            positions = new Vector3[line.positionCount];
            line.GetPositions(positions);
            List<Vector2> path =  playerControl.GetColliderPath(line, positions);
            cld.SetPath(0,path.ToArray());
            cld.offset = -transform.position;
        }
        else
        {
            Destroy(cld);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            active = false;
        }
    }
}
