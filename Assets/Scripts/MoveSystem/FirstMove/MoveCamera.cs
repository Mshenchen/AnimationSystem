using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [Header("相机放置位置")]
    public Transform cameraPosition;
    void Update()
    {
        transform.position = cameraPosition.position;
    }
    
}
