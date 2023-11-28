using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotJoint : MonoBehaviour
{
    public enum JointType { Rotoid, Prismatic }
    
    [SerializeField] private RobotJoint parent;

    [Header("Parameters")]
    [SerializeField] private JointType type;
    public JointType GetJointType() { return type; }

    [SerializeField] private float baseSize;
    
    private float _q;
    
    public void SetQ(float value) { _q = value; }
    [SerializeField] private float minValue;
    public float GetMinValue() { return minValue; }
    [SerializeField] private float maxValue;
    public float GetMaxValue() { return maxValue; }

    [Space]
    [SerializeField] private float d;
    [SerializeField] private float alpha;
    [SerializeField] private float r;
    [SerializeField] private float theta;
    
    private Matrix4x4 _transitionMatrix = new Matrix4x4(
        new Vector4(1,0,0,0),
        new Vector4(0,1,0,0),
        new Vector4(0,0,1,0),
        new Vector4(0,0,0,1)
    );
    
    private void RefreshTransitionMatrix()
    {
        if (type == JointType.Rotoid) theta = _q * Mathf.PI / 180;
        else if (type == JointType.Prismatic) r = _q;
        
        Matrix4x4 newMatrix = new Matrix4x4(
            new Vector4(Mathf.Cos(theta),Mathf.Cos(alpha)*Mathf.Sin(theta),Mathf.Sin(alpha)*Mathf.Sin(theta),0),
            new Vector4(-Mathf.Sin(theta),Mathf.Cos(alpha)*Mathf.Cos(theta),Mathf.Sin(alpha)*Mathf.Cos(theta),0),
            new Vector4(0,-Mathf.Sin(alpha),Mathf.Cos(alpha),0),
            new Vector4(d,-r*Mathf.Sin(alpha),r*Mathf.Cos(alpha),1)
        );
        this._transitionMatrix = newMatrix;
    }
    
    private Vector3 GetPosition()
    {
        return _transitionMatrix.GetPosition();
    }

    private Quaternion GetRotation()
    {
        //Returns the rotation of the joint based on its matrix
        return Quaternion.LookRotation(
            _transitionMatrix.GetColumn(2),
            _transitionMatrix.GetColumn(1)
        );
    }

    public void RefreshTransform() {
        RefreshTransitionMatrix();
        gameObject.transform.position = GetPosition();
        gameObject.transform.rotation = GetRotation();
    }


    private void FixedUpdate() {
        RefreshTransform();
    }
}
