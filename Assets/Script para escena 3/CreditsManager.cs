using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Crea un GameObject vacío llamado "CreditsManager" en tu escena
/// y adjunta este script. También necesitas un Canvas en la escena.
/// El script crea la UI de créditos automáticamente en tiempo de ejecución.
/// </summary>
public class CreditsManager : MonoBehaviour
{
    [Header("Escena del Menú Principal")]
    [Tooltip("Nombre exacto de la escena del menú principal (como aparece en Build Settings)")]
    public string mainMenuSceneName = "MainMenu";

    [Header("Créditos")]
    [TextArea(3, 10)]
    [Tooltip("Texto de los créditos. Usa \\n para saltos de línea.")]
    public string creditsText =
        "FIN DEL JUEGO\n\n" +
        "Gracias por jugar\n\n" +
        "CRÉDITOS\n\n" +
        "Diseño y Programación\n[Tu Nombre]\n\n" +
        "Arte y Modelos\n[Nombre del Artista]\n\n" +
        "Música y Sonido\n[Nombre del Compositor]\n\n" +
        "Desarrollado con Unity\n\n" +
        "© 2024 [Tu Estudio]";

    [Header("Tiempos")]
    [Tooltip("Segundos que tarda en aparecer el panel de créditos")]
    public float fadeInDuration = 1f;

    [Tooltip("Segundos que permanecen visibles los créditos antes de volver al menú")]
    public float creditsDisplayTime = 6f;

    [Tooltip("Segundos que tarda en desaparecer el panel antes de cambiar de escena")]
    public float fadeOutDuration = 1f;

    [Header("Colores")]
    public Color backgroundColor = new Color(0f, 0f, 0f, 0.95f);
    public Color titleColor = new Color(1f, 0.84f, 0f, 1f);   // Dorado
    public Color textColor = Color.white;

    // --- UI generada en runtime ---
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private bool creditsActive = false;

    void Awake()
    {
        // Asegurarse de que no se destruya entre escenas (opcional)
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Llamado por ObjectDisappear cuando el objeto desaparece.
    /// </summary>
    public void ShowCredits()
    {
        if (creditsActive) return;
        creditsActive = true;

        BuildCreditsUI();
        StartCoroutine(CreditsSequence());
    }

    // -------------------------------------------------------
    // Construcción de la UI de créditos en runtime
    // -------------------------------------------------------
    void BuildCreditsUI()
    {
        // Canvas raíz
        GameObject canvasGO = new GameObject("CreditsCanvas");
        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        canvasGroup = canvasGO.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        // Fondo negro
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(canvasGO.transform, false);
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = backgroundColor;
        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // Panel central con scroll (para textos largos)
        GameObject panel = new GameObject("CreditsPanel");
        panel.transform.SetParent(canvasGO.transform, false);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.1f, 0.1f);
        panelRect.anchorMax = new Vector2(0.9f, 0.9f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // Texto de créditos
        GameObject textGO = new GameObject("CreditsText");
        textGO.transform.SetParent(panel.transform, false);
        Text creditsTextComponent = textGO.AddComponent<Text>();
        creditsTextComponent.text = creditsText;
        creditsTextComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        creditsTextComponent.fontSize = 22;
        creditsTextComponent.color = textColor;
        creditsTextComponent.alignment = TextAnchor.UpperCenter;
        creditsTextComponent.horizontalOverflow = HorizontalWrapMode.Wrap;
        creditsTextComponent.verticalOverflow = VerticalWrapMode.Overflow;

        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        // Botón para volver al menú manualmente
        GameObject btnGO = new GameObject("BackToMenuButton");
        btnGO.transform.SetParent(canvasGO.transform, false);
        Button btn = btnGO.AddComponent<Button>();
        Image btnImage = btnGO.AddComponent<Image>();
        btnImage.color = new Color(0.8f, 0.1f, 0.1f, 1f);

        RectTransform btnRect = btnGO.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.35f, 0.04f);
        btnRect.anchorMax = new Vector2(0.65f, 0.12f);
        btnRect.offsetMin = Vector2.zero;
        btnRect.offsetMax = Vector2.zero;

        // Texto del botón
        GameObject btnText = new GameObject("Text");
        btnText.transform.SetParent(btnGO.transform, false);
        Text btnLabel = btnText.AddComponent<Text>();
        btnLabel.text = "Volver al Menú";
        btnLabel.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        btnLabel.fontSize = 18;
        btnLabel.color = Color.white;
        btnLabel.alignment = TextAnchor.MiddleCenter;
        RectTransform btnLabelRect = btnText.GetComponent<RectTransform>();
        btnLabelRect.anchorMin = Vector2.zero;
        btnLabelRect.anchorMax = Vector2.one;
        btnLabelRect.offsetMin = Vector2.zero;
        btnLabelRect.offsetMax = Vector2.zero;

        btn.onClick.AddListener(GoToMainMenu);
    }

    // -------------------------------------------------------
    // Secuencia: fade in → esperar → fade out → cambiar escena
    // -------------------------------------------------------
    IEnumerator CreditsSequence()
    {
        // Desactivar control del jugador (opcional)
        SetPlayerControl(false);

        // Fade IN
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, fadeInDuration));

        // Esperar mientras se muestran los créditos
        yield return new WaitForSeconds(creditsDisplayTime);

        // Fade OUT y cambiar escena
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0f, fadeOutDuration));

        GoToMainMenu();
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        float elapsed = 0f;
        cg.alpha = from;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        cg.alpha = to;
    }

    void GoToMainMenu()
    {
        StopAllCoroutines();
        SetPlayerControl(true);
        SceneManager.LoadScene(mainMenuSceneName);
    }

    /// <summary>
    /// Activa o desactiva el control del jugador durante los créditos.
    /// </summary>
    void SetPlayerControl(bool enabled)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        // Intentar desactivar MonoBehaviours de movimiento comunes
        MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            // No desactivar este mismo script
            if (script is CreditsManager) continue;
            script.enabled = enabled;
        }

        // Cursor
        Cursor.lockState = enabled ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !enabled;
    }
}