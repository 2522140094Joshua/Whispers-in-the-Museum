using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [Header("UI References")]
    public GameObject inventoryPanel;
    public Image[] slots;
    public TextMeshProUGUI[] slotNames;
    public Sprite emptySlotSprite;

    private bool isOpen = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        // Inicializar slots vacios
        UpdateUI(new List<GameObject>());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen;
    }

    public void UpdateUI(List<GameObject> items)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Count)
            {
                ItemData data = items[i].GetComponent<ItemData>();

                if (data != null && data.icon != null)
                {
                    slots[i].sprite = data.icon;
                    slots[i].color = Color.white;

                    if (slotNames[i] != null)
                        slotNames[i].text = data.itemName;
                }
                else
                {
                    slots[i].color = Color.white;
                    if (slotNames[i] != null)
                        slotNames[i].text = items[i].name;
                }
            }
            else
            {
                // Slot vacio
                if (emptySlotSprite != null)
                    slots[i].sprite = emptySlotSprite;

                slots[i].color = new Color(1f, 1f, 1f, 0.15f);

                if (slotNames[i] != null)
                    slotNames[i].text = "Vacio";
            }
        }
    }
}