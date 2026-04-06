using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GeneradorPass : MonoBehaviour
{
    [Header("Configuración de Clave")]
    public string claveCorrecta = "1985";

    [Header("Referencias de UI")]
    public GameObject panelUI;
    public TMP_InputField inputClave;
    public TextMeshProUGUI textoFeedback; // Arrastra el texto de error/acierto aquí

    [Header("Referencias de Escena")]
    public Light luzLed;

    private bool jugadorCerca = false;
    private bool resuelto = false;

    void Start()
    {
        // Al iniciar, nos aseguramos de que el panel esté apagado
        if (panelUI != null) panelUI.SetActive(false);
    }

    void Update()
    {
        // Abrir con E si el jugador está cerca y no está resuelto
        if (jugadorCerca && !resuelto && Input.GetKeyDown(KeyCode.E))
        {
            AbrirPanel();
        }
    }

    public void AbrirPanel()
    {
        panelUI.SetActive(true);
        textoFeedback.text = "INGRESE CÓDIGO";
        textoFeedback.color = Color.black;

        // ACTIVAR RATÓN
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Detener al jugador (Si usas el script Logica_player)
        Logica_player player = Object.FindAnyObjectByType<Logica_player>();
        if (player != null) player.puedeMoverse = false;

        Debug.Log("Panel Abierto. Ratón activado.");
    }

    // ESTA ES LA FUNCIÓN QUE VA EN EL ON CLICK DEL BOTÓN
    public void ValidarClave()
    {
        // --- ESTA LÍNEA ES EL "ESCUDO" ---
        // Si ya está resuelto, no hagas nada más (evita sumar 2/3, 3/3, etc.)
        if (resuelto) return;

        Debug.Log("Botón Validar presionado.");

        if (inputClave.text == claveCorrecta)
        {
            Debug.Log("¡CÓDIGO CORRECTO!");

            // Marcamos como resuelto INMEDIATAMENTE
            resuelto = true;

            // Feedback Visual
            textoFeedback.text = "CÓDIGO CORRECTO";
            textoFeedback.color = Color.green;
            luzLed.color = Color.green;

            // Avisar al GameManager (Solo ocurrirá UNA VEZ gracias al 'if' de arriba)
            ControladorPuzzle gestor = Object.FindAnyObjectByType<ControladorPuzzle>();
            if (gestor != null) gestor.GeneradorCompletado();

            // Esperar un poco y cerrar
            Invoke("CerrarPanel", 1.5f);
        }
        else
        {
            Debug.Log("CÓDIGO INCORRECTO");
            textoFeedback.text = "CÓDIGO INCORRECTO";
            textoFeedback.color = Color.red;
            inputClave.text = "";
        }
    }

    public void CerrarPanel()
    {
        panelUI.SetActive(false);

        // OCULTAR RATÓN
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Devolver movimiento al jugador
        Logica_player player = Object.FindAnyObjectByType<Logica_player>();
        if (player != null) player.puedeMoverse = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) jugadorCerca = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            CerrarPanel();
        }
    }
}