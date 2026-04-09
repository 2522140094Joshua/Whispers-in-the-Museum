using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneTimer : MonoBehaviour
{
    [Header("=== UI EXTERNA ===")]
    [Tooltip("Arrastra aqui tu TimerText (TextMeshPro) del Canvas existente")]
    public TMP_Text timerUI;

    [Header("=== TIMER ===")]
    public float tiempoTotal = 120f;

    [Tooltip("Nombre exacto de la escena del menu principal en Build Settings")]
    public string mainMenuSceneName = "MainMenu";

    [Tooltip("Segundos restantes para empezar parpadeo de urgencia")]
    public float urgentAt = 30f;

    [Header("=== GAME OVER ===")]
    public string gameOverTitle = "GAME OVER";
    public string gameOverSubtitle = "Se acabo el tiempo...";
    public float gameOverDisplayTime = 3f;
    public float fadeDuration = 0.8f;

    // --- Privadas ---
    private TMP_Text timerLabel;
    private GameObject gameOverCanvas;
    private CanvasGroup gameOverCG;

    private float tiempoRestante;
    private bool activo = true;
    private bool finalTriggered = false;
    private bool blinkState = false;
    private float blinkTimer = 0f;

    void Start()
    {
        tiempoRestante = tiempoTotal;

        // Si se asignó un TMP externo, usarlo. Si no, crear uno nuevo.
        if (timerUI != null)
            timerLabel = timerUI;
        else
            BuildTimerUI();

        BuildGameOverUI();
    }

    void Update()
    {
        if (!activo || finalTriggered) return;

        tiempoRestante -= Time.deltaTime;
        tiempoRestante = Mathf.Max(tiempoRestante, 0f);

        if (timerLabel != null)
        {
            int mins = Mathf.FloorToInt(tiempoRestante / 60f);
            int secs = Mathf.FloorToInt(tiempoRestante % 60f);
            timerLabel.text = $"{mins:00}:{secs:00}";

            if (tiempoRestante <= urgentAt)
            {
                blinkTimer += Time.deltaTime;
                if (blinkTimer >= 0.4f)
                {
                    blinkState = !blinkState;
                    timerLabel.color = blinkState
                        ? new Color(1f, 0.15f, 0.15f, 1f)
                        : Color.white;
                    blinkTimer = 0f;
                }
            }
        }

        if (tiempoRestante <= 0f)
        {
            finalTriggered = true;
            activo = false;
            StartCoroutine(GameOverSequence());
        }
    }

    IEnumerator GameOverSequence()
    {
        if (timerLabel != null) timerLabel.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        gameOverCanvas.SetActive(true);
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        float elapsed = 0f;
        while (elapsed < gameOverDisplayTime)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));

        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float e = 0f;
        gameOverCG.alpha = from;
        while (e < duration)
        {
            e += Time.unscaledDeltaTime;
            gameOverCG.alpha = Mathf.Lerp(from, to, e / duration);
            yield return null;
        }
        gameOverCG.alpha = to;
    }

    public void PausarTimer() { activo = false; }
    public void ReanudarTimer() { if (!finalTriggered) activo = true; }
    public void AgregarTiempo(float segundos)
    {
        tiempoRestante = Mathf.Min(tiempoRestante + segundos, tiempoTotal);
    }

    void BuildTimerUI()
    {
        GameObject canvasGO = new GameObject("TimerCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject bg = new GameObject("TimerBG");
        bg.transform.SetParent(canvasGO.transform, false);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0f, 0f, 0f, 0.45f);
        RectTransform bgRT = bg.GetComponent<RectTransform>();
        bgRT.anchorMin = bgRT.anchorMax = new Vector2(0.5f, 0.95f);
        bgRT.pivot = new Vector2(0.5f, 0.5f);
        bgRT.sizeDelta = new Vector2(160f, 60f);

        GameObject textGO = new GameObject("TimerText");
        textGO.transform.SetParent(canvasGO.transform, false);
        timerLabel = textGO.AddComponent<TextMeshProUGUI>();
        timerLabel.text = "02:00";
        timerLabel.fontSize = 42;
        timerLabel.fontStyle = FontStyles.Bold;
        timerLabel.color = Color.white;
        timerLabel.alignment = TextAlignmentOptions.Center;
        RectTransform textRT = textGO.GetComponent<RectTransform>();
        textRT.anchorMin = textRT.anchorMax = new Vector2(0.5f, 0.95f);
        textRT.pivot = new Vector2(0.5f, 0.5f);
        textRT.sizeDelta = new Vector2(160f, 60f);
    }

    void BuildGameOverUI()
    {
        gameOverCanvas = new GameObject("GameOverCanvas");
        Canvas canvas = gameOverCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;
        CanvasScaler scaler = gameOverCanvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        gameOverCanvas.AddComponent<GraphicRaycaster>();

        gameOverCG = gameOverCanvas.AddComponent<CanvasGroup>();
        gameOverCG.alpha = 0f;

        MakeImage(gameOverCanvas.transform, "BG",
            new Color(0.04f, 0f, 0f, 0.97f), Vector2.zero, Vector2.one);
        MakeImage(gameOverCanvas.transform, "LineTop",
            new Color(0.88f, 0.1f, 0.1f, 1f),
            new Vector2(0.15f, 0.672f), new Vector2(0.85f, 0.675f));
        MakeTMP(gameOverCanvas.transform, "Title", gameOverTitle,
            new Vector2(0.1f, 0.55f), new Vector2(0.9f, 0.74f),
            new Color(0.88f, 0.1f, 0.1f, 1f), 90, FontStyles.Bold);
        MakeImage(gameOverCanvas.transform, "LineBot",
            new Color(0.88f, 0.1f, 0.1f, 1f),
            new Vector2(0.15f, 0.548f), new Vector2(0.85f, 0.551f));
        MakeTMP(gameOverCanvas.transform, "Subtitle", gameOverSubtitle,
            new Vector2(0.15f, 0.43f), new Vector2(0.85f, 0.54f),
            new Color(0.82f, 0.82f, 0.82f, 1f), 30, FontStyles.Italic);
        MakeTMP(gameOverCanvas.transform, "Returning", "Volviendo al menu...",
            new Vector2(0.25f, 0.30f), new Vector2(0.75f, 0.40f),
            new Color(1f, 0.55f, 0f, 1f), 24, FontStyles.Normal);

        gameOverCanvas.SetActive(false);
    }

    void MakeImage(Transform parent, string name, Color color, Vector2 aMin, Vector2 aMax)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<Image>().color = color;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = aMin; rt.anchorMax = aMax;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    TMP_Text MakeTMP(Transform parent, string name, string content,
        Vector2 aMin, Vector2 aMax, Color color, int size, FontStyles style)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        TMP_Text t = go.AddComponent<TextMeshProUGUI>();
        t.text = content; t.fontSize = size;
        t.fontStyle = style; t.color = color;
        t.alignment = TextAlignmentOptions.Center;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = aMin; rt.anchorMax = aMax;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        return t;
    }
}