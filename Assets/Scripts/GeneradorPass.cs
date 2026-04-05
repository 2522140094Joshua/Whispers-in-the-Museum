using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GeneradorPass : MonoBehaviour
{
    [Header("Configuración")]
    public string claveCorrecta = "1985"; // Cambia la clave aquí
    public GameObject panelUI;
    public TMP_InputField inputClave;
    public Light luzLed;
    

    private bool jugadorCerca = false;
    private bool resuelto = false;

    void Update()
    {
        // Si el jugador está cerca, no se ha resuelto y presiona E
        if (jugadorCerca && !resuelto && Input.GetKeyDown(KeyCode.E))
        {
            AbrirPanel();
        }
    }

    void AbrirPanel()
    {
        panelUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None; // Libera el mouse para escribir
        Cursor.visible = true;

        Logica_player player = Object.FindAnyObjectByType<Logica_player>();
        if (player != null) player.puedeMoverse = false;
        // Si tu personaje tiene un script de movimiento, deberías pausarlo aquí
    }

    // Esta función la conectaremos al BOTÓN de la UI
    public void ValidarClave()
    {
        if (inputClave.text == claveCorrecta)
        {
            Debug.Log("¡Acceso Concedido!");
            resuelto = true;
            CerrarPanel();
            luzLed.color = Color.green; // Cambia a verde

            // Avisar al GameManager que este ya quedó
            Object.FindAnyObjectByType<ControladorPuzzle>().GeneradorCompletado();
        }
        else
        {
            Debug.Log("Clave Incorrecta");
            inputClave.text = ""; // Limpiar para reintentar
        }
    }

    public void CerrarPanel()
    {
        panelUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked; // Bloquea el mouse para seguir jugando
        Cursor.visible = false;
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