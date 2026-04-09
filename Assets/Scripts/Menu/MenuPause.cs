using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;

    public GameObject panelPausa;
    private bool pausado = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Escuchar cuando cualquier escena cargue para reconectar referencias
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Se ejecuta automáticamente cada vez que carga una escena nueva.
    /// Aquí reconectamos TODO lo que pudo haberse perdido.
    /// </summary>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Resetear estado de pausa
        pausado = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Si estamos en el menú no necesitamos el panel
        if (scene.name == "Menu")
        {
            if (panelPausa != null) panelPausa.SetActive(false);
            return;
        }

        // Reconectar panelPausa buscándolo por nombre en la nueva escena
        if (panelPausa == null)
        {
            GameObject found = GameObject.Find("PanelPausa");
            if (found != null)
                panelPausa = found;
            else
                Debug.LogWarning("[PauseMenu] No se encontró 'PanelPausa' en la escena.");
        }

        if (panelPausa != null) panelPausa.SetActive(false);
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Menu") return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausado) Continuar();
            else Pausar();
        }
    }

    public void Pausar()
    {
        if (panelPausa != null) panelPausa.SetActive(true);
        Time.timeScale = 0f;
        pausado = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Continuar()
    {
        if (panelPausa != null) panelPausa.SetActive(false);
        Time.timeScale = 1f;
        pausado = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Reiniciar()
    {
        // Limpiar estado — OnSceneLoaded se encarga del resto
        Time.timeScale = 1f;
        pausado = false;
        panelPausa = null; // forzar reconexión al recargar
        if (panelPausa != null) panelPausa.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void IrMenu()
    {
        Time.timeScale = 1f;
        pausado = false;
        panelPausa = null; // forzar reconexión
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("Menu");
    }
}