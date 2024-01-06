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

    void SolveInverseKinematics(Vector3 target)
    {   
        if(targetProcessed) return;

        for (int iter = 0; iter < maxIterations; iter++)
        {
            for (int j = joints.Length - 1; j >= 0; j--)
            {
                Debug.Log($"Processing joint {j}: {joints[j].gameObject.name}");
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
        // Calculate the rotation needed for the joint
        Vector3 toEndEffector = GetEndEffectorPosition() - joint.transform.position;
        Vector3 toTarget = target - joint.transform.position;

        if (Vector3.Cross(toEndEffector, toTarget).magnitude > float.Epsilon)
        {
            Quaternion rotation = CalculateRotation(toEndEffector, toTarget);
            Debug.Log($"{joint.gameObject.name} Rotate: {rotation.eulerAngles.z}");
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
        Debug.Log($"{joint.gameObject.name} Translate: {translation}");
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
    public IEnumerator DrawLetterE()
    {
        float radius = 1.0f;
        Vector3 center = new Vector3(0, 0, 0);
        List<Vector3> points = new List<Vector3>();

        Vector3 leftmostPoint = new Vector3(center.x - radius, center.y, center.z);
        points.Add(leftmostPoint); 
        points.Add(center);   

        int totalPoints = 20;

        for (int i = 0; i <= totalPoints; i++)
        {
            float angle = (Mathf.PI * 1.5f) * i / totalPoints;
            float x = center.x + radius * Mathf.Cos(angle);
            float y = center.y + radius * Mathf.Sin(angle);
            points.Add(new Vector3(x, y, center.z));
        }

        for (int i = 0; i < points.Count - 1; i++)
        {
            startPoint = points[i];
            endPoint = points[i + 1];
            yield return StartCoroutine(MoveTo(endPoint));
        }

        currentEndPosition = points[points.Count - 1];
    }

    public IEnumerator DrawLetterN()
    {
        float height = 2.0f;
        float width = 1.0f; 
        Vector3 start = currentEndPosition;

        List<Vector3> points = new List<Vector3>
        {
            start,
            start + Vector3.up * height,
            start + Vector3.up * height + Vector3.right * width,
            start + Vector3.right * width
        };

        for (int i = 0; i < points.Count - 1; i++)
        {
            startPoint = points[i];
            endPoint = points[i + 1];
            yield return StartCoroutine(MoveTo(endPoint));
        }

        currentEndPosition = points[points.Count - 1];
    }// TODO: Top of n must be a half circle

    public IEnumerator DrawLetterS()
    {
        float radius = 0.5f; 
        Vector3 start = currentEndPosition;
        Vector3 centerTop = start + new Vector3(0, 1.5f, 0); 
        Vector3 centerBottom = start + new Vector3(0, 0.5f, 0);

        List<Vector3> points = new List<Vector3>();

        for (int i = 0; i <= 10; i++)
        {
            float angle = Mathf.PI * i / 10;
            float x = centerTop.x + radius * Mathf.Cos(angle);
            float y = centerTop.y + radius * Mathf.Sin(angle);
            points.Add(new Vector3(x, y, 0));
        }

        for (int i = 0; i <= 10; i++)
        {
            float angle = Mathf.PI + (Mathf.PI / 2) * (i / 10.0f);
            float x = centerBottom.x - radius * Mathf.Cos(angle);
            float y = centerBottom.y + radius * Mathf.Sin(angle);
            points.Add(new Vector3(x, y, 0));
        }

        for (int i = 0; i < points.Count - 1; i++)
        {
            startPoint = points[i];
            endPoint = points[i + 1];
            yield return StartCoroutine(MoveTo(endPoint));
        }

        currentEndPosition = points[points.Count - 1];
    }

    public IEnumerator DrawLetterI()
    {
        float height = 2.0f; 
        Vector3 start = currentEndPosition; 

        List<Vector3> points = new List<Vector3>
        {
            start,
            start + Vector3.up * height
        };

        for (int i = 0; i < points.Count - 1; i++)
        {
            startPoint = points[i];
            endPoint = points[i + 1];
            yield return StartCoroutine(MoveTo(endPoint));
        }

        currentEndPosition = points[points.Count - 1];
    }
    /*
    public IEnumerator DrawLetterA()
    {
        Vector3 start = currentEndPosition;

        currentEndPosition = points[points.Count - 1];
    }//TODO: a minuscule = half circle + straight horizontal line + full circle
    */

    public void Space()
    {
        currentEndPosition.x += letterSpacing;
    }

   
    IEnumerator DrawWordENSISA()
    {
        yield return StartCoroutine(DrawLetterE());
        Space();
        yield return StartCoroutine(DrawLetterN());
        Space();
        yield return StartCoroutine(DrawLetterS());
        Space();
        yield return StartCoroutine(DrawLetterI());
        Space();
        yield return StartCoroutine(DrawLetterS()); // TODO: This one must be the other way
        Space();
        //yield return StartCoroutine(DrawLetterA());

    }

    public void StartDrawing()
    {
        StartCoroutine(DrawWordENSISA());
    }


}

// TODO: Fix letters drawing
