using UnityEngine;
using TMPro;

public class Pickup : MonoBehaviour
{
    [Header("Indicador visual")]
    public GameObject promptUI; // Arrastra aqui un texto tipo "[E] Recoger"

    private bool isNearItem = false;
    private Collider nearbyItem = null;

    void Start()
    {
        // Ocultar prompt al inicio
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    void Update()
    {
        if (isNearItem && nearbyItem != null && Input.GetKeyDown(KeyCode.E))
        {
            TryPickup();
        }
    }

    void TryPickup()
    {
        if (InventoryManager.Instance == null) return;

        if (InventoryManager.Instance.IsFull())
        {
            Debug.Log("Inventario lleno, no puedes recoger mas items.");
            return;
        }

        GameObject obj = nearbyItem.gameObject;
        bool added = InventoryManager.Instance.AddItem(obj);

        if (added)
        {
            Debug.Log("Item recogido: " + obj.name);
            isNearItem = false;
            nearbyItem = null;

            if (promptUI != null)
                promptUI.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            isNearItem = true;
            nearbyItem = other;

            if (promptUI != null)
                promptUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            isNearItem = false;
            nearbyItem = null;

            if (promptUI != null)
                promptUI.SetActive(false);
        }
    }
}