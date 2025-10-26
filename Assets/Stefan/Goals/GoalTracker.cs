using UnityEngine;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class GoalTracker : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text itemsText;                 
    public TMP_Text goalCompleteText;          
    [Header("Goal Settings")]
    public int targetItems = 8;                

    [Header("References")]
    public InventoryManager inventoryManager;

    [Header("Goal Status")]
    public bool allGoalsComplete = false;

    private CanvasGroup canvasGroup;
    private bool goalMessageShown = false;
    private float messageTimer = 0f;
    private const float messageDuration = 4f;  

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        if (goalCompleteText)
            goalCompleteText.gameObject.SetActive(false);
    }

    void Update()
    {

        bool shouldHide = InventoryManager.IsOpen || InspectManager.IsInspecting;

        float targetAlpha = shouldHide ? 0f : 1f;
        canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, Time.unscaledDeltaTime * 8f);

        if (shouldHide) return;


        int collectedCount = inventoryManager ? inventoryManager.collected.Count : 0;

        if (itemsText)
            itemsText.text = $"Items Collected: {collectedCount}/{targetItems}";


        if (!allGoalsComplete && collectedCount >= targetItems)
        {
            allGoalsComplete = true;
            ShowGoalCompleteMessage();
        }


        if (goalMessageShown)
        {
            messageTimer += Time.unscaledDeltaTime;
            if (messageTimer >= messageDuration)
            {
                goalMessageShown = false;
                if (goalCompleteText)
                    goalCompleteText.gameObject.SetActive(false);
            }
        }
    }

    void ShowGoalCompleteMessage()
    {
        if (goalCompleteText)
        {
            goalCompleteText.gameObject.SetActive(true);
            goalCompleteText.text = "All Items Collected!"; 
        }

        goalMessageShown = true;
        messageTimer = 0f;
    }
}
