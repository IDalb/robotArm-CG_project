using System;
using System.Collections;
using UnityEngine;

public class InverseKinematics : MonoBehaviour
{
    public RobotJoint[] joints;

    public Vector3 targetPosition;

    public float positionAccuracy = 0.05f;

    public int maxIterations = 15;
    public Vector3 startPoint;
    public Vector3 endPoint;
    public float drawingSpeed = 1.0f; 
    private bool targetProcessed = false;

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
        if (targetPosition == newTarget) return;
        
        targetPosition = newTarget;
        targetProcessed = false;
    }

}

