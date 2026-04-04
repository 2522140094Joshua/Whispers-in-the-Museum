using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PaintingDropZone : MonoBehaviour
{
    public float spacingBetweenPaintings = 1.2f;
    public float displayDuration = 2f;
    public Vector3 paintingRotation = new Vector3(0, 180, 0);
    public Transform rowStartPoint;
    public DoorOpener doorOpener;

    [Header("Validacion")]
    public int pinturaRequeridas = 3;       // ← cuantas pinturas necesita

    [Header("Prompt UI")]
    public GameObject promptUI;
    public GameObject promptFaltanUI;       // ← texto "Te faltan pinturas" opcional

    private bool playerDentro = false;
    private bool triggered = false;

    void Start()
    {
        OcultarPrompt();
        if (promptFaltanUI != null) promptFaltanUI.SetActive(false);
    }

    void Update()
    {
        if (!playerDentro || triggered) return;

        // Actualiza el prompt según cuántas pinturas tiene
        if (InventoryManager.Instance.GetItems().Count >= pinturaRequeridas)
            MostrarPrompt();
        else
            OcultarPrompt();

        if (Input.GetKeyDown(KeyCode.E))
        {
            List<GameObject> items = InventoryManager.Instance.GetItems();

            if (items.Count < pinturaRequeridas)
            {
                if (promptFaltanUI != null)
                    StartCoroutine(MostrarMensajeTemporal());
                return;
            }

            triggered = true;
            OcultarPrompt();
            StartCoroutine(PlaceAndDeliver(new List<GameObject>(items)));
        }
    }

    IEnumerator MostrarMensajeTemporal()
    {
        if (promptFaltanUI != null)
        {
            promptFaltanUI.SetActive(true);
            yield return new WaitForSeconds(2f);
            promptFaltanUI.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerDentro = true;

        // Solo muestra "[E] Colocar" si tiene suficientes pinturas
        if (InventoryManager.Instance.GetItems().Count >= pinturaRequeridas)
            MostrarPrompt();
        else
        {
            // Si no tiene suficientes, muestra el mensaje de faltan
            if (promptFaltanUI != null)
                StartCoroutine(MostrarMensajeTemporal());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerDentro = false;
        OcultarPrompt();
        if (promptFaltanUI != null) promptFaltanUI.SetActive(false);
    }

    void MostrarPrompt() { if (promptUI != null) promptUI.SetActive(true); }
    void OcultarPrompt() { if (promptUI != null) promptUI.SetActive(false); }

    IEnumerator PlaceAndDeliver(List<GameObject> toDeliver)
    {
        Vector3 origin = rowStartPoint != null ? rowStartPoint.position : transform.position;

        for (int i = 0; i < toDeliver.Count; i++)
        {
            GameObject painting = toDeliver[i];
            Vector3 pos = origin + Vector3.right * (i * spacingBetweenPaintings);

            painting.SetActive(true);
            painting.transform.position = pos;
            painting.transform.rotation = Quaternion.Euler(paintingRotation);

            Rigidbody rb = painting.GetComponent<Rigidbody>();
            if (rb != null) { rb.velocity = Vector3.zero; rb.isKinematic = true; }

            Collider col = painting.GetComponent<Collider>();
            if (col != null) col.enabled = false;

            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(displayDuration);
        InventoryManager.Instance.DeliverAllItems();
        if (doorOpener != null) doorOpener.OpenDoor();
    }
}