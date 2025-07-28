using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]float smoothSpeed;
    public Transform target;
    [SerializeField] Vector3 offset;
    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + offset, smoothSpeed * Time.deltaTime);
    }
}
