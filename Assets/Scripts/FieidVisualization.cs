using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieidVisualization : MonoBehaviour
{



    // method for drawing area we can see
    public void DrawField(Mesh _fieldMesh, float _fieldObjectsOverlap, float _meshResolution, float _viewAngle, float _viewRadius, float _obsticalCheckResolution, float _edgeDistanceThresh, LayerMask _obsticalMask)
    {
        int stepCount = Mathf.RoundToInt(_viewAngle * _meshResolution);
        float angleStep = _viewAngle / stepCount;

        List<Vector3> pointsHit = new List<Vector3>();
        // for all steps
        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i <= stepCount; i++)
        {
            // get ray !rotate! angle
            // rotate angle + left edge of field + step
            // start from left and go right
            float rayAngle = transform.eulerAngles.y - _viewAngle / 2 + angleStep * i;

            ViewCastInfo castInfo = ViewCast(rayAngle, _viewRadius, _obsticalMask);
            //Debug.DrawLine(transform.position, castInfo.point);
            if (i > 0)
            {
                // check if our rays collide different walls
                // check by watch distance of two rays, on different walls distance will be big
                bool edgeDistanceThreshHolll = Mathf.Abs(oldViewCast.dst - castInfo.dst) > _edgeDistanceThresh;
                // check if (right hit and left not) or (right not hit and left hit) 
                if (oldViewCast.hit != castInfo.hit || (oldViewCast.hit && oldViewCast.hit && edgeDistanceThreshHolll))
                {
                    // then when we have this situation, we nead find where is obstical edge
                    EdgeInfo edge = FindObsticalEdge(oldViewCast, castInfo, _obsticalMask, _viewRadius, _obsticalCheckResolution, _edgeDistanceThresh);
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
            verteces[i + 1] = transform.InverseTransformPoint(pointsHit[i]) + Vector3.forward * _fieldObjectsOverlap;

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
        _fieldMesh.Clear();
        _fieldMesh.vertices = verteces;
        _fieldMesh.triangles = trianglArray;
        _fieldMesh.RecalculateNormals();
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

    // method for cast ray on angle from player
    // return info about raycast hit
    public ViewCastInfo ViewCast(float globalAngle, float distance, LayerMask obsticalsMask)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        // raycust check colision with mask (walls)
        if (Physics.Raycast(transform.position, dir, out hit, distance, obsticalsMask))
        {
            return new ViewCastInfo(true, hit.distance, hit.point, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, distance, transform.position + dir * distance, globalAngle);
        }
    }

    public EdgeInfo FindObsticalEdge(ViewCastInfo minCast, ViewCastInfo maxCast, LayerMask obsticalsMask, float viewRadius, float obsticalCheckResolution, float edgeDistanceThresh)
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
            ViewCastInfo newCast = ViewCast(angleBetween, viewRadius, obsticalsMask);
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
            else
            {
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
