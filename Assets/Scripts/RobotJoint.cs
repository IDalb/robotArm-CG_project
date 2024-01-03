using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotJoint : MonoBehaviour
{
    public enum JointType { Rotoid, Prismatic }
    
    [SerializeField] private RobotJoint parent;
    public List<RobotJoint> children;

    [Header("Parameters")]
    [SerializeField] private JointType type;
    public JointType GetJointType() { return type; }

    [SerializeField] private float baseSize;
    
    private float _q;

    public void SetQ(float value)
    {
        _q = value;
        RefreshTransform();
    }
    [SerializeField] private float minValue;
    public float GetMinValue() { return minValue; }
    [SerializeField] private float maxValue;
    public float GetMaxValue() { return maxValue; }

    [Space]
    [SerializeField] private float d;
    [SerializeField] private float alpha;
    [SerializeField] private float r;
    [SerializeField] private float theta;
    
    private Matrix4x4 _localTransitionMatrix = Matrix4x4.identity;
    private Matrix4x4 _globalTransitionMatrix = Matrix4x4.identity;
    
    private void RefreshTransitionMatrix()
    {
        switch (type)
        {
            case JointType.Rotoid: theta = _q * Mathf.PI / 180; break;
            case JointType.Prismatic: r = _q; break;
            default: break;
        }
        
        Matrix4x4 newMatrix = new Matrix4x4(
            new Vector4(Mathf.Cos(theta),Mathf.Cos(alpha)*Mathf.Sin(theta),Mathf.Sin(alpha)*Mathf.Sin(theta),0),
            new Vector4(-Mathf.Sin(theta),Mathf.Cos(alpha)*Mathf.Cos(theta),Mathf.Sin(alpha)*Mathf.Cos(theta),0),
            new Vector4(0,-Mathf.Sin(alpha),Mathf.Cos(alpha),0),
            new Vector4(d,-r*Mathf.Sin(alpha),r*Mathf.Cos(alpha),1)
        );
        _localTransitionMatrix = newMatrix;

        _globalTransitionMatrix = parent is not null
            ? parent._globalTransitionMatrix * _localTransitionMatrix
            : _localTransitionMatrix;
    }
    
    private Vector3 GetPosition()
    {
        return _globalTransitionMatrix.GetPosition();
    }

    private Quaternion GetRotation()
    {
        //Returns the rotation of the joint based on its matrix
        return Quaternion.LookRotation(
            _globalTransitionMatrix.GetColumn(2),
            _globalTransitionMatrix.GetColumn(1)
        );
    }

    private void RefreshTransform()
    {
        RefreshTransitionMatrix();
        gameObject.transform.position = GetPosition();
        gameObject.transform.rotation = GetRotation();

        foreach (var child in children)
            child.RefreshTransform();
    }

    private void Awake()
    {
        if (parent != null) parent.children.Add(this);
    }
}
