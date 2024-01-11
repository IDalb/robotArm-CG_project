using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RobotJoint))]

public class PistonScale : MonoBehaviour
{
    // ===== Small script that resizes the piston rod size ===== //

    public Transform pistonRod;

    private void Update() {
        pistonRod.localScale = new Vector3(pistonRod.localScale.x, transform.localPosition.y / 2, pistonRod.localScale.z);
        pistonRod.localPosition = new Vector3(pistonRod.localPosition.x, pistonRod.localPosition.y, -0.5f * transform.localPosition.y);
    }
    
}
