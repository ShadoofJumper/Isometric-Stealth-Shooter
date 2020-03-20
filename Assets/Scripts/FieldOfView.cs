using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    // ray cout resolution
    public float meshResolution;
    // resolution of how many time check edge of obstical, for smooth look
    public int obsticalCheckResolution;

    public float edgeDistanceThresh;

    // mask for obsticals and targets
    public LayerMask obsticalsMask;
    public LayerMask targetsMask;

    // mesh of field of view visualization
    public MeshFilter fieldMeshFilter;
    private Mesh fieldMesh;

    // list of target we can see
    [HideInInspector]
    public List<Transform> targetsInField = new List<Transform>();



    private void Start()
    {
        // create empty mesh
        fieldMesh = new Mesh();
        fieldMesh.name = "View field";
        fieldMeshFilter.mesh = fieldMesh;
        StartCoroutine("FindTargetWithDelay", .2f);
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
    }

    //corutin for delay search of targets
    IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }


    // method for tagets of filed of view
    public void FindVisibleTargets()
    {
        targetsInField.Clear();
        // find all targets in our range using standart method
        Collider[] alltargets = Physics.OverlapSphere(transform.position, viewRadius, targetsMask);
        for (int i = 0; i < alltargets.Length; i++)
        {
            Transform target    = alltargets[i].transform;
            // get direction to target
            Vector3 dirTotarget   = (target.position - transform.position).normalized;
            //find angle to ratget
            float angleToTarget = Vector3.Angle(transform.forward, dirTotarget);

            // if target in field of view
            if (angleToTarget < viewAngle / 2)
            {            
                float dst = Vector3.Distance(target.position, transform.position);
                // make raycast to taget, if between not obsticals, then we can see
                if (!Physics.Raycast(transform.position, dirTotarget, dst, obsticalsMask))
                {
                    targetsInField.Add(target);
                }
            }
        }

    }


    // method for get direction from angle
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    // method for drawing area we can see
    public void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float angleStep = viewAngle / stepCount;

        List<Vector3> pointsHit = new List<Vector3>();

        // for all steps
        ViewCastInfo oldViewCast = new ViewCastInfo();

        int testCount = 0;
        for (int i = 0; i <= stepCount; i++)
        {
            testCount += 1;
            // get ray !rotate! angle
            // rotate angle + left edge of field + step
            // start from left and go right
            float rayAngle = transform.eulerAngles.y - viewAngle / 2 + angleStep * i;
            
            ViewCastInfo castInfo = ViewCast(rayAngle);
            //Debug.DrawLine(transform.position, castInfo.point);

            if (i > 0)
            {
                // check if our rays collide different walls
                // check by watch distance of two rays, on different walls distance will be big
                bool edgeDistanceThreshHolll = Mathf.Abs(oldViewCast.dst - castInfo.dst) > edgeDistanceThresh;
                // check if (right hit and left not) or (right not hit and left hit) 
                if (oldViewCast.hit != castInfo.hit || (oldViewCast.hit && oldViewCast.hit && edgeDistanceThreshHolll))
                {
                    // then when we have this situation, we nead find where is obstical edge
                    EdgeInfo edge = FindObsticalEdge(oldViewCast, castInfo);
                    // if parametrs not steal zero
                    if (edge.pointA != Vector3.zero)
                    {
                        pointsHit.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        pointsHit.Add(edge.pointB);
                    }
                }
            }

            //add poin we hit in list
            pointsHit.Add(castInfo.point);

            oldViewCast = castInfo;
        }
        

        //create array of triangls we need to paint
        //cout of vertex
        int vertexCount = pointsHit.Count + 1;
        // array for all vertexes // for example {0,1,2,3}
        Vector3[] verteces = new Vector3[vertexCount];
        //array for triangls vertexes // for example {0,1,2, 0,2,3} two triangls
        int[] trianglArray = new int[(vertexCount-2)*3];

        //start point on zero
        verteces[0] = Vector3.zero;
        //add vertexes to triangl array
        for (int i = 0; i < vertexCount - 1; i++)
        {
            //full vertices array with points we have
            verteces[i + 1] = transform.InverseTransformPoint(pointsHit[i]);

            if (i < vertexCount - 2)
            {
                //full triangl array with index of points of triangls
                trianglArray[i * 3] = 0;
                trianglArray[i * 3 + 1] = i + 1;
                trianglArray[i * 3 + 2] = i + 2;
            }
        }


        // when we have all info we need (vetrices, triangles)
        // paint triangls
        fieldMesh.Clear();
        fieldMesh.vertices = verteces;
        fieldMesh.triangles = trianglArray;
        fieldMesh.RecalculateNormals();

    }

    // method for cast ray on angle from player
    // return info about raycast hit
    public ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        // raycust check colision with mask (walls)
        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obsticalsMask))
        {
            return new ViewCastInfo(true, hit.distance, hit.point, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, viewRadius, transform.position + dir * viewRadius, globalAngle);
        }
    }

    public EdgeInfo FindObsticalEdge(ViewCastInfo minCast, ViewCastInfo maxCast)
    {
        float minAngle = minCast.angle;
        float maxAngle = maxCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < obsticalCheckResolution; i++)
        {
            //find middle ray between max and min
            //get angle between
            float angleBetween = (minAngle + maxAngle) / 2;

            // new cast
            ViewCastInfo newCast = ViewCast(angleBetween);
            //paint line for test
            //Debug.DrawLine(transform.position, newCast.point, new Vector4(0, 1, 0, 1));

            bool edgeDistanceThreshHolll = Mathf.Abs(minCast.dst - newCast.dst) > edgeDistanceThresh;
            // if new cast hit, then it is new min
            // and if we not have different walls
            if (newCast.hit == minCast.hit && !edgeDistanceThreshHolll)// 
            {
                minAngle = angleBetween;
                minPoint = newCast.point;
            }
            //else if not hit then it new max
            else{
                maxAngle = angleBetween;
                maxPoint = newCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    //struct to save info about raycast hit
    public struct ViewCastInfo
    {
        //is have hit
        public bool hit;
        //distance to hit
        public float dst;
        //hit point info
        public Vector3 point;
        //angle of raycast from player
        public float angle;

        public ViewCastInfo(bool _hit, float _dst, Vector3 _point, float _angle)
        {
            hit = _hit;
            dst = _dst;
            point = _point;
            angle = _angle;
        }

    }


    //struct to save info about edges in our field of view
    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }

    }


}
