using UnityEngine;
using System.Collections.Generic;
using StarterAssets;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public static bool IsOpen { get; private set; } = false;   

    [Header("UI")]
    public InventoryUI inventoryUI;

    [Header("References")]
    public FirstPersonController fpc;                 
    public StarterAssetsInputs starterInputs;         
    public InspectDialogueSystem dialogueSystem;     
    [Header("Data")]
    public List<InventoryItem> collected = new List<InventoryItem>();

    private bool isOpen = false;

#if ENABLE_INPUT_SYSTEM
    private PlayerInput playerInput;
#endif

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (inventoryUI)
            inventoryUI.gameObject.SetActive(false);

        if (fpc == null) fpc = FindObjectOfType<FirstPersonController>(true);
        if (starterInputs == null) starterInputs = FindObjectOfType<StarterAssetsInputs>(true);
        if (dialogueSystem == null) dialogueSystem = FindObjectOfType<InspectDialogueSystem>(true);

#if ENABLE_INPUT_SYSTEM
        if (playerInput == null) playerInput = FindObjectOfType<PlayerInput>(true);
#endif
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (InspectManager.IsInspecting)
                return;

            ToggleInventory();
        }
    }

    public void Add(InventoryItem item)
    {
        if (item == null) return;

        if (!collected.Contains(item))
        {
            collected.Add(item);
        }

        if (isOpen && inventoryUI != null)
            inventoryUI.Refresh(collected);
    }

    public void ToggleInventory()
    {
        if (inventoryUI == null)
        {
            return;
        }

        isOpen = !isOpen;
        IsOpen = isOpen;

        inventoryUI.gameObject.SetActive(isOpen);

        if (isOpen)
        {
            inventoryUI.Refresh(collected);

            if (dialogueSystem != null)
                dialogueSystem.FadeOutForInventory();

            if (fpc != null) fpc.enabled = false;
               
#if ENABLE_INPUT_SYSTEM
            if (playerInput != null) playerInput.enabled = false;
#endif
            if (starterInputs != null)
                starterInputs.cursorInputForLook = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;

            if (dialogueSystem != null)
                dialogueSystem.FadeInAfterInventory();

            if (fpc != null) fpc.enabled = true;

#if ENABLE_INPUT_SYSTEM
            if (playerInput != null) playerInput.enabled = true;
#endif
            if (starterInputs != null)
                starterInputs.cursorInputForLook = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
