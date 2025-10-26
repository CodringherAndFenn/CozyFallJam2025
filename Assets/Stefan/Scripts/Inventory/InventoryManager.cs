using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public InventoryUI inventoryUI;
    public List<InventoryItem> collected = new List<InventoryItem>();

    private bool isOpen = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ✅ keeps the inventory persistent
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (inventoryUI)
            inventoryUI.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            ToggleInventory();
    }

    public void Add(InventoryItem item)
    {
        if (item == null) return;

        // Avoid duplicates
        if (!collected.Contains(item))
        {
            collected.Add(item);
            Debug.Log($"✅ Added item: {item.itemName} (Total: {collected.Count})");
        }

        // If inventory is open, refresh it
        if (isOpen)
            inventoryUI.Refresh(collected);
    }

    public void ToggleInventory()
    {
        if (inventoryUI == null)
        {
            Debug.LogWarning("InventoryUI not assigned in InventoryManager!");
            return;
        }

        isOpen = !isOpen;
        inventoryUI.gameObject.SetActive(isOpen);

        if (isOpen)
        {
            // Refresh UI
            inventoryUI.Refresh(collected);

            // ✅ Unlock and show cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // ✅ Optional: Pause the game
            Time.timeScale = 0f;
        }
        else
        {
            // ✅ Lock and hide cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // ✅ Resume game
            Time.timeScale = 1f;
        }
    }
}
