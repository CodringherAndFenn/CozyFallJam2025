using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;

public class InspectManager : MonoBehaviour
{
    [Header("References")]
    public Transform inspectTarget;
    public Camera inspectRenderCamera;
    public FirstPersonController fpc;
    public InspectSystem inspectSystem;
    public GameObject blurVolume;
    public TMP_Text interactText;

    [Header("Interaction Settings")]
    public LayerMask interactLayer;
    public float outlineDistance = 5f;
    public float interactDistance = 3f;
    public string outlineLayerName = "OutlinedObjects";

    [Header("Stability Settings")]
    public float outlineLossDelay = 0.05f;

    private Camera cam;
    private InspectableObject currentObject;
    private InspectableObject outlinedObject;
    private bool isInspecting = false;
    private float outlineLossTimer = 0f;

    private Volume blurVolumeComponent;
    private DepthOfField dof;
    private readonly Dictionary<Transform, int> outlinedOriginalLayers = new Dictionary<Transform, int>();

    [Header("Dialogue System")]
    public InspectDialogueSystem dialogueSystem;

    void Start()
    {
        cam = Camera.main;

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

        if (inspectRenderCamera != null)
            inspectRenderCamera.enabled = false;
    }

    void Update()
    {
        if (!isInspecting)
        {
            HandleOutlineAndPrompt();
            HandleInteractionInput();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))
        {
            EndInspect();
        }
    }

    void HandleOutlineAndPrompt()
    {
        int outlineLayerIndex = LayerMask.NameToLayer(outlineLayerName);
        int combinedLayerMask = interactLayer | (1 << outlineLayerIndex);

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
                    CacheAndSetOutlineLayers(outlinedObject.transform, outlineLayerIndex);
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
                // 👇 Remove ClearOutline() here
                StartInspect(outlinedObject);
                interactText.gameObject.SetActive(false);
            }
        }
    }

    bool HasLineOfSight(RaycastHit hit)
    {
        Vector3 direction = (hit.point - cam.transform.position).normalized;
        float distance = Vector3.Distance(cam.transform.position, hit.point);
        if (Physics.Raycast(cam.transform.position, direction, out RaycastHit obstacle, distance, ~0))
            return obstacle.collider == hit.collider;
        return false;
    }

    void StartInspect(InspectableObject obj)
    {
        // First, restore its normal layer (Default) before inspection
        ClearOutline();

        isInspecting = true;
        currentObject = obj;
        obj.StartInspection(inspectTarget); // This sets Inspect layer

        if (dialogueSystem != null && !string.IsNullOrEmpty(obj.inspectDescription))
        {
            dialogueSystem.StartDialogue(obj.inspectDescription);
        }

        fpc.enabled = false;
        inspectSystem.objectToInspect = obj.transform;
        inspectSystem.enabled = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (dof != null)
        {
            float distance = Vector3.Distance(cam.transform.position, obj.transform.position);
            dof.focusDistance.value = distance;
        }

        StartCoroutine(FadeBlur(true));

        if (inspectRenderCamera != null)
            inspectRenderCamera.enabled = true;
    }

    void EndInspect()
    {
        if (currentObject != null)
    {
        // --- NEW: add to inventory if it has a pickup ---
        var pickup = currentObject.GetComponent<PickupWhenInspected>();
        if (pickup != null && pickup.item != null)
        {
            InventoryManager.Instance.Add(pickup.item);
            // Hide the world object after collecting
            currentObject.gameObject.SetActive(false);
        }
        // -------------------------------------------------

        currentObject.EndInspection();
        currentObject = null;
        }

        fpc.enabled = true;
        inspectSystem.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isInspecting = false;

        if (dof != null)
            dof.focusDistance.value = 10f;

        StartCoroutine(FadeBlur(false));

        if (inspectRenderCamera != null)
            inspectRenderCamera.enabled = false;
    }

    // --- Outline Layer Handling ---
    void CacheAndSetOutlineLayers(Transform target, int outlineLayer)
    {
        outlinedOriginalLayers.Clear();
        CacheLayersRecursive(target);
        SetLayerRecursive(target, outlineLayer);
    }

    void ClearOutline()
    {
        foreach (var kvp in outlinedOriginalLayers)
        {
            if (kvp.Key)
                kvp.Key.gameObject.layer = kvp.Value;
        }
        outlinedOriginalLayers.Clear();
        outlinedObject = null;
    }

    void CacheLayersRecursive(Transform t)
    {
        outlinedOriginalLayers[t] = t.gameObject.layer;
        for (int i = 0; i < t.childCount; i++)
            CacheLayersRecursive(t.GetChild(i));
    }

    void SetLayerRecursive(Transform t, int layer)
    {
        t.gameObject.layer = layer;
        for (int i = 0; i < t.childCount; i++)
            SetLayerRecursive(t.GetChild(i), layer);
    }

    // --- Blur ---
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
