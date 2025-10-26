using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("Grid References")]
    public GameObject gridContainer;      // 👈 reference to your Scroll View or GridParent
    public Transform gridParent;          // where the item buttons are spawned
    public GameObject gridItemPrefab;     

    [Header("Details Panel")]
    public GameObject detailsPanel;
    public Image detailsIcon;
    public TMP_Text detailsTitle;
    public TMP_Text detailsBody;
    public Button closeButton;            // 👈 optional close button reference

    private readonly List<GameObject> spawnedButtons = new List<GameObject>();

    private void Start()
    {
        // Make sure only grid is visible on start
        if (gridContainer) gridContainer.SetActive(true);
        if (detailsPanel) detailsPanel.SetActive(false);

        if (closeButton)
            closeButton.onClick.AddListener(CloseDetails);
    }

    public void Refresh(List<InventoryItem> items)
    {
        // Clean up previous buttons
        foreach (GameObject go in spawnedButtons)
        {
            if (go != null)
                Destroy(go);
        }
        spawnedButtons.Clear();

        // Spawn buttons for collected items
        foreach (var item in items)
        {
            if (item == null) continue;

            GameObject go = Instantiate(gridItemPrefab, gridParent);
            go.name = $"ItemButton_{item.itemName}";
            spawnedButtons.Add(go);

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

        // Show grid and hide details when refreshed
        if (gridContainer) gridContainer.SetActive(true);
        if (detailsPanel) detailsPanel.SetActive(false);
    }

    public void ShowDetails(InventoryItem item)
    {
        if (item == null) return;

        // Hide the grid, show the details
        if (gridContainer) gridContainer.SetActive(false);
        if (detailsPanel) detailsPanel.SetActive(true);

        if (detailsIcon) detailsIcon.sprite = item.itemIcon;
        if (detailsTitle) detailsTitle.text = item.itemName;
        if (detailsBody) detailsBody.text = item.itemDescription;
    }

    public void CloseDetails()
    {
        // Hide details, show grid again
        if (detailsPanel) detailsPanel.SetActive(false);
        if (gridContainer) gridContainer.SetActive(true);
    }
}
