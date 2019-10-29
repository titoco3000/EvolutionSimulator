using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Vector3 MovementSpeed;
    public Transform Target;

    private Transform OriginalTarget;

    float minHeight = 1f;
    void Start()
    {
        OriginalTarget = Target;
    }

    // Update is called once per frame
    void Update()
    {
        float originalY = transform.position.y;
        if (originalY < minHeight)
            originalY = minHeight;

        //go forward/backwards
        transform.position += transform.forward * Input.GetAxis("Vertical") * MovementSpeed.y * Time.deltaTime/Time.timeScale;
        //rotate around center
        transform.position += transform.right * Input.GetAxis("Horizontal") * MovementSpeed.x * Time.deltaTime / Time.timeScale;
        transform.position = new Vector3(transform.position.x, originalY, transform.position.z);
        //up/down
        transform.position += Vector3.up * Input.GetAxis("Height") * MovementSpeed.z * Time.deltaTime / Time.timeScale;

        if (Target != null)
        {
            //transform.LookAt(Target);
            Vector3 relativePos = Target.position - transform.position;
            Quaternion toRotation = Quaternion.LookRotation(relativePos);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 40 * Time.deltaTime);

        }
        //se o target é null
        else
            ResetTarget();
    }

    public void DefinirTarget(Transform newTarget)
    {
        Target = newTarget;
    }
    public void ResetTarget()
    {
        Target = OriginalTarget;
    }
}
