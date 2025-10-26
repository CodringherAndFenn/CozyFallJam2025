using UnityEngine;

public class CameraFreeze : MonoBehaviour
{
    public KeyCode lockKey = KeyCode.L;
    private bool isLocked = false;
    private Vector3 frozenPosition;
    private Quaternion frozenRotation;

    void Update()
    {
        if (Input.GetKeyDown(lockKey))
        {
            isLocked = !isLocked;

            if (isLocked)
            {
                frozenPosition = transform.position;
                frozenRotation = transform.rotation;
            }
        }

        if (isLocked)
        {
            transform.position = frozenPosition;
            transform.rotation = frozenRotation;
        }
    }
}
