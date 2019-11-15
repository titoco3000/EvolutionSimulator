using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorRotation : MonoBehaviour
{
    public Transform mirroredObject;

    
    private void Update()
    {
        Vector3 currentRot = mirroredObject.rotation.eulerAngles;
        
        transform.rotation = Quaternion.Euler(transform.rotation.x, currentRot.z * 0.5f -90 , transform.rotation.z);
    }
}
