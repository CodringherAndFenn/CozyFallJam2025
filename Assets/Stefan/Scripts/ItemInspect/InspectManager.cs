using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using StarterAssets;
using System.Collections;

public class InspectManager : MonoBehaviour
{
    [Header("References")]
    public Transform inspectTarget;
    public FirstPersonController fpc;
    public InspectSystem inspectSystem;
    public GameObject blurVolume;
    public TMP_Text interactText;

    [Header("Interaction Settings")]
    public LayerMask interactLayer;
    public float outlineDistance = 5f;
    public float interactDistance = 3f;
    [Tooltip("Select the layer used by your outline shader")]
    public int outlineLayer;

    [Header("Stability Settings")]
    [Tooltip("Delay before outline disappears after losing sight")]
    public float outlineLossDelay = 0.05f;

    private Camera cam;
    private InspectableObject currentObject;
    private InspectableObject outlinedObject;
    private int defaultLayer;
    private bool isInspecting = false;
    private float outlineLossTimer = 0f;

    private Volume blurVolumeComponent;
    private DepthOfField dof;

    void Start()
    {
        cam = Camera.main;

        // Cache DoF override
        if (blurVolume != null)
        {
            blurVolumeComponent = blurVolume.GetComponent<Volume>();
            if (blurVolumeComponent != null)
                blurVolumeComponent.profile.TryGet(out dof);
        }

        if (blurVolume != null)
            blurVolume.SetActive(false);

        inspectSystem.enabled = false;
        interactText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isInspecting)
        {
            HandleOutlineAndPrompt();
            HandleInteractionInput();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))
            {
                EndInspect();
            }
        }
    }

    void HandleOutlineAndPrompt()
    {
        // Combine interact layer and outline layer so raycast keeps hitting outlined objects
        int combinedLayerMask = interactLayer | (1 << outlineLayer);

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, outlineDistance, combinedLayerMask))
        {
            var inspectable = hit.collider.GetComponent<InspectableObject>();
            if (inspectable != null && HasLineOfSight(hit))
            {
                outlineLossTimer = 0f;

                if (outlinedObject != inspectable)
                {
                    ClearOutline();
                    outlinedObject = inspectable;
                    defaultLayer = outlinedObject.gameObject.layer;
                    outlinedObject.gameObject.layer = outlineLayer;
                }

                float distance = Vector3.Distance(cam.transform.position, hit.point);
                interactText.gameObject.SetActive(distance <= interactDistance);
                return;
            }
        }

        outlineLossTimer += Time.deltaTime;
        if (outlineLossTimer >= outlineLossDelay)
        {
            ClearOutline();
            interactText.gameObject.SetActive(false);
        }
    }

    void HandleInteractionInput()
    {
        if ((Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)) && outlinedObject != null)
        {
            float distance = Vector3.Distance(cam.transform.position, outlinedObject.transform.position);
            if (distance <= interactDistance)
            {
                StartInspect(outlinedObject);
                ClearOutline();
                interactText.gameObject.SetActive(false);
            }
        }
    }

    bool HasLineOfSight(RaycastHit hit)
    {
        Vector3 direction = (hit.point - cam.transform.position).normalized;
        float distance = Vector3.Distance(cam.transform.position, hit.point);
        if (Physics.Raycast(cam.transform.position, direction, out RaycastHit obstacle, distance, ~0))
        {
            return obstacle.collider == hit.collider;
        }
        return false;
    }

    void StartInspect(InspectableObject obj)
    {
        isInspecting = true;
        currentObject = obj;
        obj.StartInspection(inspectTarget);

        fpc.enabled = false;
        inspectSystem.objectToInspect = obj.transform;
        inspectSystem.enabled = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Focus camera on the inspected object distance
        if (dof != null)
        {
            float distance = Vector3.Distance(cam.transform.position, obj.transform.position);
            dof.focusDistance.value = distance;
        }

        StartCoroutine(FadeBlur(true));
    }

    void EndInspect()
    {
        if (currentObject != null)
        {
            currentObject.EndInspection();
            currentObject = null;
        }

        fpc.enabled = true;
        inspectSystem.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isInspecting = false;

        // Reset focus distance to far (clear blur)
        if (dof != null)
            dof.focusDistance.value = 10f;

        StartCoroutine(FadeBlur(false));
    }

    void ClearOutline()
    {
        if (outlinedObject != null)
        {
            outlinedObject.gameObject.layer = defaultLayer;
            outlinedObject = null;
        }
    }

    // Smooth blur fade
    IEnumerator FadeBlur(bool enable)
    {
        if (blurVolumeComponent == null) yield break;
        if (enable) blurVolume.SetActive(true);

        float startWeight = blurVolumeComponent.weight;
        float targetWeight = enable ? 1f : 0f;
        float duration = 0.3f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            blurVolumeComponent.weight = Mathf.Lerp(startWeight, targetWeight, t / duration);
            yield return null;
        }

        blurVolumeComponent.weight = targetWeight;
        if (!enable) blurVolume.SetActive(false);
    }
}
