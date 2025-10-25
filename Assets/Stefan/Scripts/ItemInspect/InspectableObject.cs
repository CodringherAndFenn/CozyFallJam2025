using UnityEngine;

public class InspectableObject : MonoBehaviour
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform originalParent;
    private bool isBeingInspected = false;

    public void StartInspection(Transform inspectTarget)
    {
        if (isBeingInspected) return;

        originalParent = transform.parent;
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        transform.SetParent(null);
        transform.position = inspectTarget.position;
        transform.rotation = inspectTarget.rotation;

        isBeingInspected = true;
    }

    public void EndInspection()
    {
        if (!isBeingInspected) return;

        transform.SetParent(originalParent);
        transform.position = originalPosition;
        transform.rotation = originalRotation;

        isBeingInspected = false;
    }
}
