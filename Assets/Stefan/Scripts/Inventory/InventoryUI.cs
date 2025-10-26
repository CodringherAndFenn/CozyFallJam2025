using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("Grid References")]
    public Transform gridParent;        // Content under ScrollView
    public GameObject gridItemPrefab;   // Prefab for each inventory slot

    [Header("Details Panel")]
    public GameObject detailsPanel;
    public Image detailsIcon;
    public TMP_Text detailsTitle;
    public TMP_Text detailsBody;

    // Keep track of currently displayed buttons (so we can rebuild cleanly)
    private readonly List<GameObject> spawnedButtons = new List<GameObject>();

    public void Refresh(List<InventoryItem> items)
    {
        // Clear old entries safely
        foreach (GameObject go in spawnedButtons)
        {
            if (go != null)
                Destroy(go);
        }
        spawnedButtons.Clear();

        // Build new buttons for each collected item
        foreach (var item in items)
        {
            if (item == null) continue;

            GameObject go = Instantiate(gridItemPrefab, gridParent);
            go.name = $"ItemButton_{item.itemName}";
            spawnedButtons.Add(go);

            // Find icon & label (make sure prefab children are named correctly)
            Image icon = go.transform.Find("Icon")?.GetComponent<Image>();
            TMP_Text label = go.transform.Find("Name")?.GetComponent<TMP_Text>();

            if (icon != null) icon.sprite = item.itemIcon;
            if (label != null) label.text = item.itemName;

            Button btn = go.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => ShowDetails(item));
            }
        }

        detailsPanel.SetActive(false);
    }

    public void ShowDetails(InventoryItem item)
    {
        if (item == null) return;

        detailsPanel.SetActive(true);

        if (detailsIcon) detailsIcon.sprite = item.itemIcon;
        if (detailsTitle) detailsTitle.text = item.itemName;
        if (detailsBody) detailsBody.text = item.itemDescription;
    }

    public void CloseDetails()
    {
        detailsPanel.SetActive(false);
    }
}
