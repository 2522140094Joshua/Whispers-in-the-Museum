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
        // ── Singleton robusto — evita duplicados entre escenas ──
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // sobrevive cambios de escena
    }

    public bool AddItem(GameObject item)
    {
        // Validaciones defensivas
        if (item == null)
        {
            Debug.LogWarning("AddItem: item es null.");
            return false;
        }

        if (items.Contains(item))
        {
            Debug.LogWarning("AddItem: el item ya está en el inventario — " + item.name);
            return false;
        }

        if (items.Count >= maxSlots)
        {
            Debug.Log("Inventario lleno!");
            return false;
        }

        items.Add(item);
        item.SetActive(false);

        ActualizarUI();
        return true;
    }

    public bool RemoveItem(int index)
    {
        if (index < 0 || index >= items.Count)
        {
            Debug.LogWarning($"RemoveItem: índice {index} fuera de rango.");
            return false;
        }

        if (items[index] != null)
            items[index].SetActive(true);

        items.RemoveAt(index);
        ActualizarUI();
        return true;
    }

    public GameObject GetItem(int index)
    {
        if (index < 0 || index >= items.Count) return null;
        return items[index];
    }

    public List<GameObject> GetItems() => items;

    public bool IsFull() => items.Count >= maxSlots;

    public int Count() => items.Count;

    public void DeliverAllItems()
    {
        // Limpiamos nulos por si algún objeto fue destruido externamente
        items.RemoveAll(i => i == null);

        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (items[i] != null)
                Destroy(items[i]);
        }

        items.Clear();
        ActualizarUI();
    }

    // ── Método centralizado para actualizar UI ──
    private void ActualizarUI()
    {
        if (InventoryUI.Instance != null)
            InventoryUI.Instance.UpdateUI(items);
        else
            Debug.LogWarning("InventoryManager: InventoryUI.Instance es null.");
    }
}