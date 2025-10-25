using UnityEngine;

public class InspectSystem : MonoBehaviour
{
    public Transform objectToInspect;
    public float rotationSpeed = 150f;

    private Vector3 currentRotation;

    void OnEnable()
    {
        if (objectToInspect != null)
            currentRotation = objectToInspect.eulerAngles;
    }

    void Update()
    {
        if (objectToInspect == null) return;

        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            currentRotation.x -= mouseY * rotationSpeed * Time.deltaTime;
            currentRotation.y += mouseX * rotationSpeed * Time.deltaTime;

            objectToInspect.rotation = Quaternion.Euler(currentRotation);
        }
    }
}
