using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTo : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    private void Awake() {
        if (target == null) enabled = false;
    }

    void LateUpdate()
    {
        transform.LookAt(target);
    }
}
