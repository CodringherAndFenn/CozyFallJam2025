using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("Grid References")]
    public Transform gridParent;       
    public GameObject gridItemPrefab;   

    [Header("Details Panel")]
    public GameObject detailsPanel;
    public Image detailsIcon;
    public TMP_Text detailsTitle;
    public TMP_Text detailsBody;

    private readonly List<GameObject> spawnedButtons = new List<GameObject>();

    public void Refresh(List<InventoryItem> items)
    {
        foreach (GameObject go in spawnedButtons)
        {
            if (go != null)
                Destroy(go);
        }
        spawnedButtons.Clear();

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
