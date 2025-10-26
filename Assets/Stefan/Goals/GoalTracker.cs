using UnityEngine;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class GoalTracker : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text itemsText;
    public TMP_Text cleanText;

    [Header("Goal Settings")]
    public int targetItems = 6;
    public float targetCleanLevel = 100f;

    [Header("References")]
    public InventoryManager inventoryManager;
    public MonoBehaviour cleanLevelScript;
    public string cleanValueFieldName = "cleanLevel";

    [Header("Goal Status")]
    public bool allGoalsComplete = false;

    private float currentCleanLevel;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void Update()
    {
        // --- Check if we should hide ---
        bool shouldHide = InventoryManager.IsOpen || InspectManager.IsInspecting;

        // --- Smooth fade (optional) ---
        float targetAlpha = shouldHide ? 0f : 1f;
        canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, Time.unscaledDeltaTime * 8f);

        // --- If hidden, don't update content ---
        if (shouldHide) return;

        // --- Get item count ---
        int collectedCount = inventoryManager ? inventoryManager.collected.Count : 0;

        // --- Get clean level from external script ---
        if (cleanLevelScript != null && !string.IsNullOrEmpty(cleanValueFieldName))
        {
            var type = cleanLevelScript.GetType();
            var field = type.GetField(cleanValueFieldName);
            if (field != null && field.FieldType == typeof(float))
                currentCleanLevel = (float)field.GetValue(cleanLevelScript);
        }

        // --- Update UI text ---
        if (itemsText)
            itemsText.text = $"Items Collected: {collectedCount}/{targetItems}";

        if (cleanText)
            cleanText.text = $"Clean Level: {Mathf.FloorToInt(currentCleanLevel)}/{Mathf.FloorToInt(targetCleanLevel)}";

        // --- Completion check ---
        allGoalsComplete = collectedCount >= targetItems && currentCleanLevel >= targetCleanLevel;
    }
}
