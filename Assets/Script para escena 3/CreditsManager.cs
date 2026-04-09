using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    [Header("Escena del Menú Principal")]
    public string mainMenuSceneName = "Menu";

    [Header("Créditos")]
    [TextArea(3, 10)]
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
    public float fadeInDuration = 1f;
    public float creditsDisplayTime = 6f;
    public float fadeOutDuration = 1f;

    [Header("Colores")]
    public Color backgroundColor = new Color(0f, 0f, 0f, 0.95f);
    public Color textColor = Color.white;

    private CanvasGroup canvasGroup;
    private bool creditsActive = false;

    // SIN DontDestroyOnLoad — no necesitamos que sobreviva entre escenas

    public void ShowCredits()
    {
        if (creditsActive) return;
        creditsActive = true;
        BuildCreditsUI();
        StartCoroutine(CreditsSequence());
    }

    void BuildCreditsUI()
    {
        GameObject canvasGO = new GameObject("CreditsCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        canvasGroup = canvasGO.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        // Fondo negro
        GameObject bg = new GameObject("BG");
        bg.transform.SetParent(canvasGO.transform, false);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = backgroundColor;
        RectTransform bgRT = bg.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = bgRT.offsetMax = Vector2.zero;

        // Panel del texto
        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(canvasGO.transform, false);
        RectTransform panelRT = panel.AddComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0.1f, 0.15f);
        panelRT.anchorMax = new Vector2(0.9f, 0.9f);
        panelRT.offsetMin = panelRT.offsetMax = Vector2.zero;

        // Texto de créditos
        GameObject textGO = new GameObject("CreditsText");
        textGO.transform.SetParent(panel.transform, false);
        Text txt = textGO.AddComponent<Text>();
        txt.text = creditsText;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.fontSize = 22;
        txt.color = textColor;
        txt.alignment = TextAnchor.UpperCenter;
        txt.horizontalOverflow = HorizontalWrapMode.Wrap;
        txt.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform txtRT = textGO.GetComponent<RectTransform>();
        txtRT.anchorMin = Vector2.zero;
        txtRT.anchorMax = Vector2.one;
        txtRT.offsetMin = txtRT.offsetMax = Vector2.zero;

        // Botón volver al menú
        GameObject btnGO = new GameObject("BtnMenu");
        btnGO.transform.SetParent(canvasGO.transform, false);
        Image btnImg = btnGO.AddComponent<Image>();
        btnImg.color = new Color(0.8f, 0.1f, 0.1f, 1f);
        Button btn = btnGO.AddComponent<Button>();
        btn.onClick.AddListener(GoToMainMenu);
        RectTransform btnRT = btnGO.GetComponent<RectTransform>();
        btnRT.anchorMin = new Vector2(0.35f, 0.04f);
        btnRT.anchorMax = new Vector2(0.65f, 0.12f);
        btnRT.offsetMin = btnRT.offsetMax = Vector2.zero;

        GameObject btnTxt = new GameObject("Label");
        btnTxt.transform.SetParent(btnGO.transform, false);
        Text btnLabel = btnTxt.AddComponent<Text>();
        btnLabel.text = "Volver al Menú";
        btnLabel.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        btnLabel.fontSize = 18;
        btnLabel.color = Color.white;
        btnLabel.alignment = TextAnchor.MiddleCenter;
        RectTransform btnLabelRT = btnTxt.GetComponent<RectTransform>();
        btnLabelRT.anchorMin = Vector2.zero;
        btnLabelRT.anchorMax = Vector2.one;
        btnLabelRT.offsetMin = btnLabelRT.offsetMax = Vector2.zero;
    }

    IEnumerator CreditsSequence()
    {
        // Bloquear cursor sin tocar scripts del jugador
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Fade IN
        yield return StartCoroutine(Fade(0f, 1f, fadeInDuration));

        // Esperar
        yield return new WaitForSecondsRealtime(creditsDisplayTime);

        // Fade OUT
        yield return StartCoroutine(Fade(1f, 0f, fadeOutDuration));

        GoToMainMenu();
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float e = 0f;
        canvasGroup.alpha = from;
        while (e < duration)
        {
            e += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, e / duration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }

    void GoToMainMenu()
    {
        StopAllCoroutines();

        // Restaurar todo antes de cambiar de escena
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(mainMenuSceneName);
    }
}