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
    public int pinturaRequeridas = 3;

    [Header("Contador")]
    public float tiempoLimite = 60f;
    public TMPro.TextMeshProUGUI timerUI;

    [Header("Game Over")]
    [Tooltip("Arrastra aquí el GameObject PuertaPivote2 (el que tiene DoorGameOver)")]
    public DoorGameOver doorGameOver;

    [Header("Prompt UI")]
    public GameObject promptUI;
    public GameObject promptFaltanUI;

    private bool playerDentro = false;
    private bool triggered = false;
    private bool contadorActivo = false;
    private float tiempoRestante;

    void Start()
    {
        OcultarPrompt();
        if (promptFaltanUI != null) promptFaltanUI.SetActive(false);
        if (timerUI != null) timerUI.gameObject.SetActive(false);
    }

    void Update()
    {
        // ── Lógica del contador ──────────────────────────────────────────
        if (contadorActivo)
        {
            tiempoRestante -= Time.deltaTime;

            if (timerUI != null)
                timerUI.text = Mathf.CeilToInt(tiempoRestante).ToString();

            if (tiempoRestante <= 0f)
            {
                contadorActivo = false;
                if (timerUI != null) timerUI.gameObject.SetActive(false);

                // Cerrar la puerta
                if (doorOpener != null) doorOpener.CloseDoor();

                // ★ Activar Game Over ★
                if (doorGameOver != null)
                    doorGameOver.TriggerGameOver();
                else
                    Debug.LogWarning("[PaintingDropZone] doorGameOver no asignado en el Inspector.");
            }
        }

        // ── Lógica de entrega ────────────────────────────────────────────
        if (!playerDentro || triggered) return;

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

    public void CancelarContador()
    {
        contadorActivo = false;
        if (timerUI != null) timerUI.gameObject.SetActive(false);
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

        if (InventoryManager.Instance.GetItems().Count >= pinturaRequeridas)
            MostrarPrompt();
        else
        {
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

        // Inicia el contador después de abrir la puerta
        tiempoRestante = tiempoLimite;
        contadorActivo = true;
        if (timerUI != null) timerUI.gameObject.SetActive(true);
    }
}