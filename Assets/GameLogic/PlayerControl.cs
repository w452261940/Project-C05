using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SKCell;

public class PlayerControl : MonoBehaviour
{
    public int controlScheme = 0;
    public Color colorTop, colorDown, colorOutline;

    public bool active = false;
    public float updateInterval = 0.1f;
    public float speed = 1.0f;
    public float angleForce = 0.4f;
    public float tolerance = 0.03f;
    public float width = 0.3f;

    public float branchDensity = 1.0f;
    public int branchDepth = 3;
    public float branchLength = 2.0f;
    public float branchSpeed = 1.0f;
    private float oBranchDensity;

    public LineRenderer line;
    public PolygonCollider2D cld;
    public SKColliderResponder headCld;

    public GameObject branchPrefab;

    private float axis_x, axis_y;

    private float updateTimer = 0f;
    private float branchTimer = 0f;
    private float pos_delta = 0f;
    private Vector3 lastPos;
    private Vector3 direction;

    private int pCount = 0;
    private int bCount = 0;

    AnimationCurve widthCurve;
    private Vector3[] positions;
    private Material mat;
    private void Awake()
    {
        cld = GetComponent<PolygonCollider2D>();
        oBranchDensity = branchDensity;
        active = false;
    }

    void Start()
    {
        mat = new Material(CommonReference.instance.rootShader);
        mat.SetColor("_Color", colorTop);
        mat.SetColor("_Color2", colorDown);
        mat.SetColor("_OutlineColor", colorOutline);
        line.material = mat;
        lastPos = transform.position;
        widthCurve = new AnimationCurve();

        widthCurve.AddKey(new Keyframe(0, 1.4f));
        widthCurve.AddKey(new Keyframe(0.8f, 1f));
        widthCurve.AddKey(new Keyframe(1, 0.1f));
        line.widthCurve = widthCurve;
        line.widthMultiplier = width;

        headCld.onTriggerEnter2D += _OnTriggerEnter2D;
        direction = Vector3.down;
    }
    void Update()
    {
        if (!active)
            return;
        UpdateInput();

    }
    private void FixedUpdate()
    {
        if (!active)
            return;
        UpdateLine();
    }
    private void UpdateInput()
    {
        KeyCode up = controlScheme == 0 ? KeyCode.UpArrow : KeyCode.W;
        KeyCode down = controlScheme == 0 ? KeyCode.DownArrow : KeyCode.S;
        KeyCode left = controlScheme == 0 ? KeyCode.LeftArrow : KeyCode.A;
        KeyCode right = controlScheme == 0 ? KeyCode.RightArrow : KeyCode.D;
        if (Input.GetKey(up))
        {
            axis_y = 1;
        }
        else if (Input.GetKey(down))
        {
            axis_y = -1;
        }
        else
            axis_y = 0;
        if (Input.GetKey(left))
        {
            axis_x = -1;
        }
        else if (Input.GetKey(right))
        {
            axis_x = 1;
        }
        else
            axis_x = 0;
    }

    void UpdateLine()
    {
        Vector3 dir = direction;
        Vector3 inputDir = new Vector3(axis_x, axis_y, 0);
        dir = Vector3.Lerp(dir, inputDir, 0.02f);

        transform.position += (dir) * speed * Time.fixedDeltaTime;

        if(pos_delta > 0f)
        {
            updateTimer += Time.fixedDeltaTime;
            branchTimer += Time.fixedDeltaTime;
        }
        if (pos_delta>0f && updateTimer > updateInterval)
        {
            int count = line.positionCount++;
            line.SetPosition(count, transform.position);
            updateTimer = 0f;
            pCount++;
            if(pCount>0 && pCount%60==0)
            line.Simplify(tolerance);
        }

        if (pos_delta > 0f && branchTimer > 5/branchDensity)
        {
            StartNewBranch(transform.position, direction);
            branchTimer = 0f;
            branchDensity = oBranchDensity * Random.Range(0.6f, 1.4f);
        }

        if (pCount >= 1)
        {
            widthCurve = new AnimationCurve();

            widthCurve.AddKey(new Keyframe(0, 1.4f));
            widthCurve.AddKey(new Keyframe((pCount-20)/pCount, 1f));
            widthCurve.AddKey(new Keyframe(1, 0.2f));
            line.widthCurve = widthCurve;
        }

        pos_delta = Vector3.Distance(transform.position, lastPos);
        direction =( transform.position - lastPos).normalized;
        lastPos = transform.position;

        if(line.positionCount>=1)
        headCld.transform.position = line.GetPosition(line.positionCount - 1);

        positions = new Vector3[line.positionCount];
        line.GetPositions(positions);
        List <Vector2> colliderPath = GetColliderPath(line, positions);
        cld.SetPath(0, colliderPath.ToArray());
        cld.offset = -transform.position;
    }

    public List<Vector2> GetColliderPath(LineRenderer line, Vector3[] pointList3)
    {
        float colliderWidth;
        List<Vector2> pointList2 = new List<Vector2>();
        colliderWidth = line.startWidth;
        pointList2.Clear();
        for (int i = 0; i < pointList3.Length; i++)
        {
            pointList2.Add(pointList3[i]);
        }

        List<Vector2> edgePointList = new List<Vector2>();

        for (int j = 1; j < pointList2.Count-5; j+=3)
        {
            j = Mathf.Clamp(j, 0, pointList2.Count-1);
            colliderWidth = line.widthCurve.Evaluate((float)j / pointList2.Count)*line.widthMultiplier;
            colliderWidth *= 0.9f;
            Vector2 distanceVector = pointList2[j - 1] - pointList2[j];
            Vector3 crossVector = Vector3.Cross(distanceVector, Vector3.forward);
            Vector2 offectVector = crossVector.normalized;
            Vector2 up = pointList2[j - 1] + 0.5f * colliderWidth * offectVector;
            Vector2 down = pointList2[j - 1] - 0.5f * colliderWidth * offectVector;
            edgePointList.Insert(0, down);
            edgePointList.Add(up);
            if (j == pointList2.Count - 1)
            {
                up = pointList2[j] + 0.5f * colliderWidth * offectVector;
                down = pointList2[j] - 0.5f * colliderWidth * offectVector;
                edgePointList.Insert(0, down);
                edgePointList.Add(up);
            }
        }
        return edgePointList;
    }

    private LineRenderer StartNewBranch(Vector3 position, Vector3 direction, float startWidth = -1, int depth = 0)
    {
        GameObject branch = GameObject.Instantiate(branchPrefab, position, Quaternion.identity);
        LineRenderer lr = branch.GetComponent<LineRenderer>();
        if(startWidth>0)
        lr.startWidth = startWidth;
        float speed = branchSpeed * Random.Range(0.8f, 1.2f);
        Vector3 cVector = Mathf.Abs(direction.x) < Mathf.Abs(direction.y) ? new Vector3(direction.x, Random.value+0.2f, 0) : new Vector3(Random.value + 0.2f, direction.y, 0);

        float lerp1 = Random.Range(0.3f, 0.9f);

        Vector3 cross = Vector3.Cross(direction, Vector3.forward) * (Random.value-0.5f);
        Vector3 branchDir = Vector3.Lerp(direction, cross, lerp1);
        Vector3 branchDir2 = Vector3.Lerp(direction, cross, lerp1 * Random.Range(1.4f, 1.9f));

        string procedureID = gameObject.GetInstanceID() + bCount.ToString();
        Branch branchComp = branch.GetComponent<Branch>();
        branchComp.cld = branch.AddComponent<PolygonCollider2D>();
        branchComp.cld.isTrigger = true;
        branchComp.procedureID = procedureID;
        branchComp.playerControl = this;

        bool hasRecurse = depth<branchDepth?Random.value < 0.5f:false;
        bool recursed = false;
        float growTime = Random.Range(0.9f, 1.2f) *((float)branchDepth/(depth+1)) * branchLength +0.3f;
        CommonUtils.StartProcedure(SKCurve.QuadraticIn, growTime, (t) =>
        {
            if (!lr)
                return;
            int count = lr.positionCount++;
            branch.transform.position += Vector3.Lerp(branchDir, branchDir2, t) * speed * Time.fixedDeltaTime;
            lr.SetPosition(count, branch.transform.position);
            if(t>=0.6f)
            if (hasRecurse && !recursed)
            {
                recursed = true;
                Vector3 point = branch.transform.position;
                    point = new Vector3(point.x, point.y, point.z+5);
                Vector3 dir = Vector3.Lerp(branchDir, branchDir2, t * (Random.value - 0.5f));
                float w = lr.widthCurve.Evaluate(t / growTime) * 0.8f;
                CommonUtils.InvokeAction(0.5f, () =>
                {
                    LineRenderer r = StartNewBranch(point, dir, w, depth + 1);
                });
            }
        },(t)=>branchComp.active=false, procedureID);
        return lr;
    }

    private void _OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            active = false;
            InstantiateEffect(CommonReference.instance.hitEffect, transform.position);
            Coordinator.instance.OnPlayerDie();
        }
    }

    public void SetColor(Color c)
    {
        mat.SetColor("_Color", c);
    }

    public void InstantiateEffect(GameObject prefab, Vector3 position)
    {
        GameObject fx = CommonUtils.SpawnObject(prefab);
        fx.transform.position = position;
        CommonUtils.InvokeAction(3f, () =>
        {
            CommonUtils.ReleaseObject(fx);
        });
    }
}
