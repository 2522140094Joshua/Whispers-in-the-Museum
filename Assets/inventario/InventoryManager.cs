using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Configuracion")]
    public int maxSlots = 3;

    private List<GameObject> items = new List<GameObject>();

    void Awake()
    {
        Instance = this;
    }

    public bool AddItem(GameObject item)
    {
        if (items.Count >= maxSlots)
        {
            Debug.Log("Inventario lleno!");
            return false;
        }

        items.Add(item);
        item.SetActive(false); // desaparece del mundo

        // ARREGLADO: ahora si actualiza la UI al recoger un item
        if (InventoryUI.Instance != null)
            InventoryUI.Instance.UpdateUI(items);

        return true;
    }

    public bool RemoveItem(int index)
    {
        if (index < 0 || index >= items.Count) return false;

        items[index].SetActive(true); // regresa al mundo
        items.RemoveAt(index);

        if (InventoryUI.Instance != null)
            InventoryUI.Instance.UpdateUI(items);

        return true;
    }

    public GameObject GetItem(int index)
    {
        if (index < items.Count)
            return items[index];
        return null;
    }

    public List<GameObject> GetItems() => items;

    public bool IsFull() => items.Count >= maxSlots;

    // Agrega esto al final de tu InventoryManager existente, antes del ultimo }
    public void DeliverAllItems()
    {
        for (int i = items.Count - 1; i >= 0; i--)
            Destroy(items[i]);

        items.Clear();

        if (InventoryUI.Instance != null)
            InventoryUI.Instance.UpdateUI(items);
    }
}