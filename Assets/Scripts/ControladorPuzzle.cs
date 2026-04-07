using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorPuzzle : MonoBehaviour
{
    [Header("Configuración del Nivel")]
    public int generadoresTotales = 3;
    public int generadoresActivos = 0;

    [Header("Temporizador")]
    public float tiempoRestante = 120f;
    public float segundosExtra = 30f;
    private bool juegoTerminado = false;
    private bool juegoIniciado = false;

    [Header("Interfaz")]
    public TextMeshProUGUI textoContador;
    public TextMeshProUGUI textoTimer;
    public TextMeshProUGUI textoInstrucciones; // El texto del panel de inicio
    public GameObject panelGameOver;
    public GameObject panelInicio;

    [Header("Objetos de Escena")]
    public GameObject puertaSalida;

    void Start()
    {
        // 1. Limpieza inicial de paneles
        if (panelGameOver != null) panelGameOver.SetActive(false);

        // 2. Configurar el inicio del juego
        if (panelInicio != null)
        {
            panelInicio.SetActive(true);
            Time.timeScale = 0f; // Pausa todo el juego

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            ConfigurarTextoInstrucciones();
        }

        ActualizarInterfaz();
    }

    // Aplica el formato de colores al texto de instrucciones
    void ConfigurarTextoInstrucciones()
    {
        if (textoInstrucciones != null)
        {
            // Usamos etiquetas de color: <color=#HEX>Texto</color>
            // El texto general es negro, los detalles en blanco/gris claro para que resalten
            textoInstrucciones.text =
                "<color=black><b>OBJETIVO: REINICIO DE EMERGENCIA</b>\n" +
                "Restablece los 3 reactores antes de que el tiempo se agote.\n\n" +
                "• <b>REACTOR 1:</b> Descifra el <color=#FFFFFF>NIP de 4 dígitos</color>.\n" +
                "• <b>REACTOR 2:</b> Coloca los <color=#FFFFFF>3 cubos</color>. <color=#800000>(¡Error = Reinicio!)</color>\n" +
                "• <b>REACTOR 3:</b> Alinea las <color=#FFFFFF>palancas</color>. <color=#333333>(Tras Reactor 1 y 2)</color>\n" +
                "• <b>ESCAPE:</b> Cruza la <color=#006400>Puerta Principal</color>.\n\n" +
                "<b>CONTROLES:</b>\n" +
                "[WASD] Moverse | [E] Interactuar </color>";
        }
    }

    public void EmpezarJuego()
    {
        juegoIniciado = true;
        Time.timeScale = 1f;

        if (panelInicio != null) panelInicio.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Logica_player p = Object.FindAnyObjectByType<Logica_player>();
        if (p != null) p.puedeMoverse = true;
    }

    void Update()
    {
        if (juegoIniciado && !juegoTerminado)
        {
            if (tiempoRestante > 0)
            {
                tiempoRestante -= Time.deltaTime;
                ActualizarReloj(tiempoRestante);
            }
            else
            {
                GameOver();
            }
        }
    }

    public void GeneradorCompletado()
    {
        if (juegoTerminado) return;
        generadoresActivos++;
        tiempoRestante += segundosExtra;
        ActualizarInterfaz();

        if (generadoresActivos >= generadoresTotales) Victoria();
    }

    void ActualizarReloj(float tiempo)
    {
        float minutos = Mathf.FloorToInt(tiempo / 60);
        float segundos = Mathf.FloorToInt(tiempo % 60);
        if (textoTimer != null)
        {
            textoTimer.text = string.Format("{0:00}:{1:00}", minutos, segundos);
            if (tiempo < 15f) textoTimer.color = Color.red;
            else textoTimer.color = Color.black;
        }
    }

    public void ActualizarInterfaz()
    {
        if (textoContador != null)
        {
            textoContador.color = Color.black;
            textoContador.text = "Generadores: " + generadoresActivos + " / " + generadoresTotales;
        }
    }

    void Victoria()
    {
        juegoTerminado = true;
        if (puertaSalida != null) puertaSalida.SetActive(false);
        textoContador.text = "¡SISTEMA ONLINE - ESCAPA!";
        textoContador.color = Color.green;
    }

    void GameOver()
    {
        juegoTerminado = true;
        if (panelGameOver != null) panelGameOver.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Logica_player p = Object.FindAnyObjectByType<Logica_player>();
        if (p != null) p.puedeMoverse = false;
    }

    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}