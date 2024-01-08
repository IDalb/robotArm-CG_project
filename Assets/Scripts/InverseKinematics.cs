using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseKinematics : MonoBehaviour
{
    public RobotJoint[] joints;

    public Vector3 targetPosition;

    public float positionAccuracy = 0.01f;

    public int maxIterations = 50;
    public Vector3 startPoint;
    public Vector3 endPoint;
    public float duration = 1.0f;
    private bool targetProcessed = false;
    public float letterSpacing = 2.0f; 
    private Vector3 currentEndPosition = Vector3.zero; 

    [Space]
    public TrailManager trailManager;

    void SolveInverseKinematics(Vector3 target)
    {   
        if(targetProcessed) return;

        for (int iter = 0; iter < maxIterations; iter++)
        {
            for (int j = joints.Length - 1; j >= 0; j--)
            {
                switch (joints[j].GetJointType())
                {
                    case RobotJoint.JointType.Rotoid: RotateTowardsTarget(joints[j], target); break;
                    case RobotJoint.JointType.Prismatic: TranslateTowardsTarget(joints[j], target); break;
                }
            }

            // Check if the target position is reached within a given tolerance
            if (IsTargetReached(target))
            {
                targetProcessed = true; 
                break;
            }
        }
    }

    void RotateTowardsTarget(RobotJoint joint, Vector3 target)
    {
        Vector3 toEndEffector = GetEndEffectorPosition() - joint.transform.position;
        Vector3 toTarget = target - joint.transform.position;

        if (Vector3.Cross(toEndEffector, toTarget).magnitude > float.Epsilon)
        {
            Quaternion rotation = CalculateRotation(toEndEffector, toTarget);
            joint.SetQ(joint.GetQ() + rotation.eulerAngles.z); 
        }
    }

    void TranslateTowardsTarget(RobotJoint joint, Vector3 target)
    {
        Vector3 jointAxis = joint.transform.forward;

        Vector3 toEndEffector = GetEndEffectorPosition() - joint.transform.position;
        Vector3 toTarget = target - joint.transform.position;

        float endEffectorProjection = Vector3.Dot(toEndEffector, jointAxis);
        float targetProjection = Vector3.Dot(toTarget, jointAxis);

        float translation = targetProjection - endEffectorProjection;
        joint.SetQ(joint.GetQ() + translation);
    }

    Quaternion CalculateRotation(Vector3 vectorA, Vector3 vectorB)
    {
        Vector3 axis = Vector3.Cross(vectorA, vectorB);
        float angle = Vector3.Angle(vectorA, vectorB);
        return Quaternion.AngleAxis(angle, axis.normalized);
    }

    private Vector3 GetEndEffectorPosition()
    {
        return joints[joints.Length - 1].transform.position;
    }


    bool IsTargetReached(Vector3 target)
    {
        Vector3 endEffectorPos = GetEndEffectorPosition();
        float distanceToTarget = (target - endEffectorPos).magnitude;
        Debug.Log($"Distance to Target: {distanceToTarget}");
        return distanceToTarget < positionAccuracy;
    }

    public IEnumerator MoveTo(Vector3 target, float duration = 2f)
    {
        Vector3 startPosition = targetPosition;
        float step = 0.0f;

        while (step < 1.0f)
        {
            step += Time.deltaTime / duration;
            SetTargetPosition(Vector3.Lerp(startPosition, target, step));
            SolveInverseKinematics(targetPosition);
            yield return null;
        }
    }

    public void SetTargetPosition(Vector3 newTarget)
    {
        if (targetPosition != newTarget)
        {
            targetPosition = newTarget;
            targetProcessed = false;
        }
    }

    private IEnumerator Draw(List<Vector3> points)
    {
        for (int i = 0; i < points.Count - 1; i++)
        {
            startPoint = points[i];
            endPoint = points[i + 1];
            yield return StartCoroutine(MoveTo(endPoint, 0.1f));
            if (i == 0) trailManager.SetDrawState(true);
        }

        trailManager.SetDrawState(false);
    }

    public IEnumerator DrawLetterE()
    {
        float radius = 1.0f;
        Vector3 center = new(-4, 0, 0);
        List<Vector3> points = new();
        Vector3 leftmostPoint = new(center.x - 2.1f, center.y, center.z);
        Vector3 rightmostPoint = new(center.x-radius, center.y, center.z);
        points.Add(leftmostPoint); 
        points.Add(rightmostPoint);   

        int totalPoints = 20;

        for (int i = 0; i <= totalPoints; i++)
        {
            float angle = (Mathf.PI * 1.5f) * i / totalPoints;
            float x = center.x + radius * Mathf.Cos(angle);
            float y = center.y + radius * Mathf.Sin(angle);
            points.Add(new Vector3(x, y, center.z));
        }

        yield return StartCoroutine(Draw(points));
        
        currentEndPosition = points[^1];
    }

    public IEnumerator DrawLetterN()
    {
        List<Vector3> points = new();
        float verticalLineHeight = 1.5f;
        float radius = 0.5f;
        Vector3 start = currentEndPosition;
        Vector3 topVerticalPoint = start + new Vector3(0, verticalLineHeight, 0);
        Vector3 centerCircle = topVerticalPoint + new Vector3(radius, 0, 0); 
        Vector3 bottomVerticalPoint = topVerticalPoint + new Vector3(2 * radius, -verticalLineHeight, 0); 

        points.Add(start);
        points.Add(start);
        points.Add(topVerticalPoint);

        for (int i = 10; i >= 0; i--)
        {
            float angle = Mathf.PI * i / 10; 
            float x = centerCircle.x + radius * Mathf.Cos(angle);
            float y = centerCircle.y + radius * Mathf.Sin(angle);
            points.Add(new Vector3(x, y, 0));
        }

        points.Add(bottomVerticalPoint);

        yield return StartCoroutine(Draw(points));

        currentEndPosition = points[^1]; 
    }


    public IEnumerator DrawLetterS()
    {
        float radius = 0.5f; 
        Vector3 start = currentEndPosition;
        Vector3 centerTop = start + new Vector3(0, 1.5f, 0); 
        Vector3 centerBottom = start + new Vector3(0, 0.5f, 0);

        List<Vector3> points = new();

        for (int i = 0; i <= 10; i++)
        {
            float angle = Mathf.PI * i / 10;
            float x = centerTop.x + radius * Mathf.Cos(angle);
            float y = centerTop.y + radius * Mathf.Sin(angle);
            points.Add(new Vector3(x, y, 0));
        }

        for (int i = 0; i <= 10; i++)
        {
            float angle = Mathf.PI + (Mathf.PI / 1.2f) * (i / 10.0f);
            float x = centerBottom.x - radius * Mathf.Cos(angle);
            float y = centerBottom.y + radius * Mathf.Sin(angle);
            points.Add(new Vector3(x, y, 0));
        }

        yield return StartCoroutine(Draw(points));

        currentEndPosition = new Vector3(points[points.Count - 1].x,-1,0);
    }

    public IEnumerator DrawSymmetricLetterS()
    {
        float radius = 0.5f;
        Vector3 start = currentEndPosition;
        Vector3 centerTop = start + new Vector3(0, 1.5f, 0);
        Vector3 centerBottom = start + new Vector3(0, 0.5f, 0);

        List<Vector3> points = new List<Vector3>();

        for (int i = 0; i <= 10; i++)
        {
            float angle = Mathf.PI * i / 10;
            float x = centerTop.x - radius * Mathf.Cos(angle);
            float y = centerTop.y + radius * Mathf.Sin(angle);
            points.Add(new Vector3(x, y, 0));
        }

        for (int i = 0; i <= 10; i++)
        {
            float angle = Mathf.PI + (Mathf.PI / 1.2f) * (i / 10.0f); 
            float x = centerBottom.x + radius * Mathf.Cos(angle); 
            float y = centerBottom.y + radius * Mathf.Sin(angle); 
            points.Add(new Vector3(x, y, 0));
        }

        yield return StartCoroutine(Draw(points));

        currentEndPosition = new Vector3(points[points.Count - 1].x-1,-1,0);
    }


    public IEnumerator DrawLetterI()
    {
        float height = 2; 
        Vector3 start = currentEndPosition; 


        List<Vector3> points = new()
        {
            start,
            start,
            start + Vector3.up * height/4,
            start + Vector3.up * height/3,
            start + Vector3.up * height/2, 
            start + Vector3.up * height
        };

        yield return StartCoroutine(Draw(points));

        currentEndPosition = new Vector3(points[points.Count - 1].x,-1,0);
    }
    
    public IEnumerator DrawLetterA()
    {
        List<Vector3> points = new();
        Vector3 start = currentEndPosition;
        float verticalLineHeight = 1.5f;
        float radius = 0.5f;
        Vector3 topVerticalPoint = start + new Vector3(0, verticalLineHeight, 0);
        Vector3 centerCircle = topVerticalPoint + new Vector3(radius, 0, 0); 
        Vector3 bottomVerticalPoint = topVerticalPoint + new Vector3(2 * radius + 0.5f, -verticalLineHeight, 0);

        points.Add(topVerticalPoint);
        for (int i = 10; i >= 0; i--)
        {
            float angle = Mathf.PI * i / 10; 
            float x = centerCircle.x + radius * Mathf.Cos(angle);
            float y = centerCircle.y + radius * Mathf.Sin(angle);
            points.Add(new Vector3(x, y, 0));
        }

        points.Add(bottomVerticalPoint);

        Vector3 bellyCenter = new(bottomVerticalPoint.x-(radius+0.15f), start.y + radius, 0); 
        for (int i = 0; i <= 20; i++) 
        {
            float angle = 2 * Mathf.PI * i / 20;
            float x = bellyCenter.x + radius * Mathf.Cos(angle);
            float y = bellyCenter.y + radius * Mathf.Sin(angle);
            points.Add(new Vector3(x, y, 0));
        }

        yield return StartCoroutine(Draw(points));

        currentEndPosition = points[^1];
    }

    public IEnumerator DrawCircleInN()
    {
        Vector3 nCenter = currentEndPosition + new Vector3(-8.85f, 0.4f, 0);
        float radius = 0.15f; 
        int numberOfPoints = 40; 

        List<Vector3> circlePoints = new();
        for (int i = 0; i < numberOfPoints; i++)
        {   
            float angle = (i * 360f / numberOfPoints) * Mathf.Deg2Rad; 
            float x = nCenter.x + radius * Mathf.Cos(angle);
            float y = nCenter.y + radius * Mathf.Sin(angle);
            circlePoints.Add(new Vector3(x, y, 0));
        }
        circlePoints.Add(circlePoints[0]);
        circlePoints.Add(circlePoints[1]);
        
        yield return StartCoroutine(Draw(circlePoints));
        currentEndPosition = new(0,-2,0);
        yield return StartCoroutine(MoveTo(currentEndPosition));
    }


    public void Space()
    {
        currentEndPosition.x += letterSpacing;
    }

   
    IEnumerator DrawWordENSISA()
    {
        trailManager.SetDrawState(false);
        yield return StartCoroutine(DrawLetterE());
        Space();
        yield return StartCoroutine(DrawLetterN());
        Space();
        yield return StartCoroutine(DrawLetterS());
        Space();
        yield return StartCoroutine(DrawLetterI());
        Space();
        yield return StartCoroutine(DrawSymmetricLetterS());
        Space();
        yield return StartCoroutine(DrawLetterA());
        yield return StartCoroutine(DrawCircleInN());

    }

    public void StartDrawing()
    {
        StartCoroutine(DrawWordENSISA());
    }


}
