using UnityEngine;

public class Pickup : MonoBehaviour
{
    [Header("Indicador visual")]
    public GameObject promptUI;

    [Header("Deteccion")]
    public float pickupRange = 2f;          // distancia maxima para recoger
    public LayerMask pickupLayer;           // opcional, filtra objetos

    private bool isNearItem = false;
    private Collider nearbyItem = null;

    void Start()
    {
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    void Update()
    {
        if (isNearItem && nearbyItem != null && Input.GetKeyDown(KeyCode.E))
            TryPickup();
    }

    void TryPickup()
    {
        if (InventoryManager.Instance == null) return;

        if (InventoryManager.Instance.IsFull())
        {
            Debug.Log("Inventario lleno.");
            return;
        }

        GameObject obj = nearbyItem.gameObject;
        bool added = InventoryManager.Instance.AddItem(obj);

        if (added)
        {
            Debug.Log("Recogido: " + obj.name);
            isNearItem = false;
            nearbyItem = null;
            if (promptUI != null)
                promptUI.SetActive(false);
        }
    }

    // ── Detección por Trigger (el collider de la pintura es Trigger) ──
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            isNearItem = true;
            nearbyItem = other;
            if (promptUI != null) promptUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            isNearItem = false;
            nearbyItem = null;
            if (promptUI != null) promptUI.SetActive(false);
        }
    }

    // ── Detección por Collider normal (si la pintura NO es Trigger) ──
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pickup"))
        {
            isNearItem = true;
            nearbyItem = collision.collider;
            if (promptUI != null) promptUI.SetActive(true);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pickup"))
        {
            isNearItem = false;
            nearbyItem = null;
            if (promptUI != null) promptUI.SetActive(false);
        }
    }
}