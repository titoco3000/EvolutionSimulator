using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Vector3 MovementSpeed;
    public Transform Target;

    float minHeight = 1f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float originalY = transform.position.y;
        if (originalY < minHeight)
            originalY = minHeight;

        //go forward/backwards
        transform.position += transform.forward * Input.GetAxis("Vertical") * MovementSpeed.y * Time.deltaTime;
        //rotate around center
        transform.position += transform.right * Input.GetAxis("Horizontal") * MovementSpeed.x * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, originalY, transform.position.z);
        //up/down
        transform.position += Vector3.up * Input.GetAxis("Height") * MovementSpeed.z * Time.deltaTime;

        if (Target != null)
            transform.LookAt(Target);
    }
}
