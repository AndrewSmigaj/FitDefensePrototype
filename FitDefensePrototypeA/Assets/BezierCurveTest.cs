using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurveTest : MonoBehaviour
{
    public Transform[] controlPoints;

    public GameObject movingObjectPrefab;
    GameObject movingObject;

    private int num_segments = 30;

    public LineRenderer lineRenderer;


    // Start is called before the first frame update
    void Start()
    {
        if (!lineRenderer)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        if (!movingObject)
        {
            movingObject = Instantiate(movingObjectPrefab, controlPoints[0].position, movingObjectPrefab.transform.rotation);
        }
        StartCoroutine(MoveAlongControlPoints());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator MoveAlongControlPoints()
    {
        //while (Vector3.Distance(controlPoints[2].position, movingObjectPrefab.transform.position) < 0.1)
        //
//yield return new WaitForSeconds(1);

        for(int i = 1; i <= num_segments; i++)
        {
            yield return new WaitForSeconds(0.05f);

            float t = i / (float)num_segments;
            Vector3 nextPos = CalculateQuadraticBezierPoint(t, controlPoints[0].position, controlPoints[1].position, controlPoints[2].position);
            movingObject.transform.position = nextPos;

            Vector3 lookAtPosition = CalculateQuadraticBezierPoint((i+1)/(float)num_segments, controlPoints[0].position, controlPoints[1].position, controlPoints[2].position);
            movingObject.transform.LookAt(lookAtPosition);
        }

            //move along curve
        //}
    }


    Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        //float uuu = uu * u;
       // float ttt = tt * t;

        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
   //     p += ttt * p3;

        return p;
    }


}
