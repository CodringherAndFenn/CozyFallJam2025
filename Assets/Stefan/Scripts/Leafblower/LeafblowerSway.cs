using UnityEngine;
using StarterAssets;

public class LeafblowerSway : MonoBehaviour
{
    [Header("Rotation Sway")]
    [SerializeField] private float rotationSwayAmount = 2f;
    [SerializeField] private float rotationSwaySmooth = 6f;

    [Header("Position Sway")]
    [SerializeField] private float positionSwayAmount = 0.02f;
    [SerializeField] private float positionSwaySmooth = 8f;

    [Header("Idle / Walk Bob")]
    [SerializeField] private float idleBobSpeed = 1.5f;
    [SerializeField] private float idleBobAmount = 0.02f;
    [SerializeField] private float walkBobSpeed = 8f;
    [SerializeField] private float walkBobAmount = 0.05f;

    [Header("References")]
    [Tooltip("Usually PlayerCameraRoot (the camera pivot point).")]
    public Transform cameraTarget;

    private Vector3 initialLocalPos;
    private Quaternion initialLocalRot;

    private Vector2 lookInput;
    private StarterAssetsInputs input;
    private CharacterController controller;

    private float bobTimer;

    void Start()
    {
        initialLocalPos = transform.localPosition;
        initialLocalRot = transform.localRotation;

        input = FindObjectOfType<StarterAssetsInputs>();
        controller = FindObjectOfType<CharacterController>();

        if (input == null)
            Debug.LogWarning("[LeafblowerSway] No StarterAssetsInputs found — using default Input.GetAxis fallback.");
    }

    void Update()
    {
        HandleSway();
        HandleBob();
    }

    private void HandleSway()
    {
        // --- ROTATION SWAY ---
        if (input != null)
            lookInput = input.look;
        else
            lookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        float moveX = -lookInput.x * rotationSwayAmount;
        float moveY = lookInput.y * rotationSwayAmount;

        Quaternion targetRot = Quaternion.Euler(moveY, moveX, 0f);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, initialLocalRot * targetRot, Time.deltaTime * rotationSwaySmooth);

        // --- POSITION SWAY ---
        Vector3 targetPos = initialLocalPos + new Vector3(-lookInput.x, -lookInput.y, 0f) * positionSwayAmount;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * positionSwaySmooth);
    }

    private void HandleBob()
    {
        if (controller == null) return;

        bool isMoving = controller.velocity.magnitude > 0.1f;
        float speed = isMoving ? walkBobSpeed : idleBobSpeed;
        float amount = isMoving ? walkBobAmount : idleBobAmount;

        bobTimer += Time.deltaTime * speed;
        float offsetY = Mathf.Sin(bobTimer) * amount;
        float offsetX = Mathf.Cos(bobTimer / 2f) * amount * 0.5f; // smaller sideways motion

        Vector3 bobOffset = new Vector3(offsetX, offsetY, 0f);
        transform.localPosition += bobOffset * Time.deltaTime * 60f; // scaled to be framerate independent
    }
}
