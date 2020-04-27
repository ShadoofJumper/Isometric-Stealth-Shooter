using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieidVisualization : MonoBehaviour
{
    // ---------------------------- Fields ----------------------------
    [Header("Settings for field")]
    public float viewRadius = 12.5f;
    [Range(0, 360)]
    public float viewAngle = 45.0f;
    // ray count resolution
    public float meshResolution = 0.5f;
    // resolution of how many time check edge of obstical, for smooth look
    public int obsticalCheckResolution = 4;
    // for check different walls whene search for edge
    public float edgeDistanceThresh = 0.5f;
    // for file of view mesh can leat bit overlap objects
    public float fieldObjectsOverlap = 0.15f;

    [Header("Mask for obsticals and targets")]
    // mask for obsticals and targets
    public LayerMask obsticalsMask;
    public LayerMask targetsMask;
    public LayerMask interactMask;

    // mesh of field of view visualization
    [SerializeField] private protected MeshFilter fieldMeshFilter;
    private protected Mesh fieldMesh;

    // ---------------------------- Logic ----------------------------
    // method for drawing area we can see
    public void DrawField(Transform parent)
    {
        int stepCount   = Mathf.RoundToInt(viewAngle * meshResolution);
        float angleStep = viewAngle / stepCount;

        List<Vector3> pointsHit = new List<Vector3>();
        // for all steps
        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i <= stepCount; i++)
        {
            // get ray !rotate! angle
            // rotate angle + left edge of field + step
            // start from left and go right
            float rayAngle = parent.transform.eulerAngles.y - viewAngle / 2 + angleStep * i;

            ViewCastInfo castInfo = ViewCast(rayAngle, viewRadius, parent);
            //Debug.DrawLine(parent.transform.position, castInfo.point);
            if (i > 0)
            {
                // check if our rays collide different walls
                // check by watch distance of two rays, on different walls distance will be big
                bool edgeDistanceThreshHolll = Mathf.Abs(oldViewCast.dst - castInfo.dst) > edgeDistanceThresh;
                // check if (right hit and left not) or (right not hit and left hit) 
                if (oldViewCast.hit != castInfo.hit || (oldViewCast.hit && oldViewCast.hit && edgeDistanceThreshHolll))
                {
                    // then when we have this situation, we nead find where is obstical edge
                    EdgeInfo edge = FindObsticalEdge(oldViewCast, castInfo, parent);
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
        //count of vertex
        int vertexCount = pointsHit.Count + 1;
        // array for all vertexes // for example {0,1,2,3}
        Vector3[] verteces = new Vector3[vertexCount];
        //array for triangls vertexes // for example {0,1,2, 0,2,3} two triangls
        int[] trianglArray = new int[(vertexCount - 2) * 3];

        //start point on zero
        verteces[0] = Vector3.zero;
        //add vertexes to triangle array
        for (int i = 0; i < vertexCount - 1; i++)
        {
            //full vertices array with points we have
            verteces[i + 1] = parent.transform.InverseTransformPoint(pointsHit[i]) + Vector3.forward * fieldObjectsOverlap;

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
    public ViewCastInfo ViewCast(float globalAngle, float distance, Transform parent)
    {
        Vector3 dir = Helpers.DirFromAngle(globalAngle, true, parent);
        RaycastHit hit;

        // raycust check colision with mask (walls)
        if (Physics.Raycast(parent.transform.position, dir, out hit, distance, obsticalsMask))
        {
            return new ViewCastInfo(true, hit.distance, hit.point, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, distance, parent.transform.position + dir * distance, globalAngle);
        }
    }

    public EdgeInfo FindObsticalEdge(ViewCastInfo minCast, ViewCastInfo maxCast, Transform parent)
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
            ViewCastInfo newCast = ViewCast(angleBetween, viewRadius, parent);
            //paint line for test
            //Debug.DrawLine(parent.transform.position, newCast.point, new Vector4(0, 1, 0, 1));

            bool edgeDistanceThreshHolll = Mathf.Abs(minCast.dst - newCast.dst) > edgeDistanceThresh;
            // if new cast hit, then it is new min
            // and if we not have different walls
            if (newCast.hit == minCast.hit && !edgeDistanceThreshHolll)// 
            {
                minAngle = angleBetween;
                minPoint = newCast.point;
            }
            //else if not hit then it new max
            else
            {
                maxAngle = angleBetween;
                maxPoint = newCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    // ---------------------------- Structs ----------------------------
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
