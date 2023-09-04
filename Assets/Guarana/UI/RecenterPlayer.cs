using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecenterPlayer : MonoBehaviour
{
    [SerializeField]
    private Transform head;
    [SerializeField]
    private Transform origin;
    [SerializeField]
    private Transform reference;


    public void Recenter()
    {
        // Reposition the user head in the reference position
        Vector3 offset = head.position - reference.position;
        // offset.y = 0; do not project on the horizontal axis
        origin.position = reference.position - offset;


        // Rotate the user head to reference forward
        Vector3 referenceForward = reference.forward;
        referenceForward.y = 0; // get only the horizontal axis
        Vector3 headForward = head.forward;
        headForward.y = 0; // get only the horizontal axis

        float angle = Vector3.SignedAngle(headForward, referenceForward, Vector3.up);
        origin.RotateAround(head.position, Vector3.up, angle);
    }


    void Start()
    {
        Recenter();
    }
}
