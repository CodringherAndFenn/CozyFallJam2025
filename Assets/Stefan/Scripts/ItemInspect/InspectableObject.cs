using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InspectableObject : MonoBehaviour
{

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;
    private Transform originalParent;
    private bool isBeingInspected = false;
    private Collider col;

    [Header("Inspect Text")]
    [TextArea(3, 6)]
    public string inspectDescription;

    [Header("Inventory Link")]
    public PickupWhenInspected pickup; // optional reference if present

    // Store all original layers (for objects with children)
    private readonly Dictionary<Transform, int> originalLayers = new Dictionary<Transform, int>();

    [Header("Inspect Settings")]
    public float moveSpeed = 8f;
    public float scaleFactor = 0.8f;
    public float viewDistance = 0.2f;
    void Awake()
    {
        col = GetComponent<Collider>();
    }

    public void StartInspection(Transform inspectTarget)
    {
        if (isBeingInspected) return;

        originalParent = transform.parent;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;

        if (col != null) col.enabled = false;

        // Save & switch all layers to Inspect
        originalLayers.Clear();
        CacheLayersRecursive(transform);
        SetLayerRecursive(transform, LayerMask.NameToLayer("Inspect"));

        transform.SetParent(inspectTarget, worldPositionStays: false);
        StartCoroutine(MoveAndScale(new Vector3(0, 0, viewDistance), originalScale * scaleFactor));

        transform.localRotation = Quaternion.identity;
        isBeingInspected = true;
    }

    public void EndInspection()
    {
        if (!isBeingInspected) return;

        if (col != null) col.enabled = true;

        transform.SetParent(originalParent);
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        transform.localScale = originalScale;

        // Restore all child layers
        foreach (var kvp in originalLayers)
        {
            if (kvp.Key)
                kvp.Key.gameObject.layer = kvp.Value;
        }

        originalLayers.Clear();
        isBeingInspected = false;
    }

    IEnumerator MoveAndScale(Vector3 targetLocalPos, Vector3 targetScale)
    {
        Vector3 startPos = transform.localPosition;
        Vector3 startScale = transform.localScale;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            transform.localPosition = Vector3.Lerp(startPos, targetLocalPos, t);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        transform.localPosition = targetLocalPos;
        transform.localScale = targetScale;
    }

    void CacheLayersRecursive(Transform t)
    {
        originalLayers[t] = t.gameObject.layer;
        for (int i = 0; i < t.childCount; i++)
            CacheLayersRecursive(t.GetChild(i));
    }

    void SetLayerRecursive(Transform t, int layer)
    {
        t.gameObject.layer = layer;
        for (int i = 0; i < t.childCount; i++)
            SetLayerRecursive(t.GetChild(i), layer);
    }
}
