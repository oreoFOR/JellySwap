using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Windmill : MonoBehaviour
{
    [SerializeField] float torque;
    private void Update()
    {
        transform.Rotate(Vector3.forward * torque * Time.deltaTime);
    }
}
