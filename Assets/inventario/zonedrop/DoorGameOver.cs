using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(DoorOpener))]
public class DoorGameOver : MonoBehaviour
{
    [Header("=== GAME OVER ===")]
    public float delayBeforeGameOver = 1.8f;
    public float gameOverDisplayTime = 4f;
    public float fadeDuration = 0.8f;

    [Header("=== TEXTOS ===")]
    public string gameOverTitle = "GAME OVER";
    public string subtitleText = "¡Se acabó el tiempo!";

    [Header("=== COLORES ===")]
    public Color bgColor = new Color(0.04f, 0f, 0f, 0.97f);
    public Color titleColor = new Color(0.88f, 0.1f, 0.1f, 1f);
    public Color subtitleColor = new Color(0.82f, 0.82f, 0.82f, 1f);
    public Color timerColor = new Color(1f, 0.55f, 0f, 1f);

    private CanvasGroup canvasGroup;
    private Text timerLabel;
    private bool triggered = false;

    public void TriggerGameOver()
    {
        if (triggered) return;
        triggered = true;
        StartCoroutine(GameOverSequence());
    }

    IEnumerator GameOverSequence()
    {
        // Bloquear cursor sin tocar scripts del jugador
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Esperar en tiempo real para que la puerta cierre
        yield return new WaitForSecondsRealtime(delayBeforeGameOver);

        BuildGameOverUI();

        // Fade IN usando tiempo real (no afecta si TimeScale cambia)
        yield return StartCoroutine(FadeRealtime(0f, 1f, fadeDuration));

        // Cuenta regresiva
        float elapsed = 0f;
        int lastSec = -1;
        while (elapsed < gameOverDisplayTime)
        {
            elapsed += Time.unscaledDeltaTime;
            int remaining = Mathf.CeilToInt(gameOverDisplayTime - elapsed);
            if (remaining != lastSec && timerLabel != null)
            {
                timerLabel.text = $"Reiniciando en {remaining}...";
                lastSec = remaining;
            }
            yield return null;
        }

        DoRestart();
    }

    void DoRestart()
    {
        StopAllCoroutines();
        // Restaurar estado antes de recargar
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void BuildGameOverUI()
    {
        // Sin DontDestroyOnLoad — se destruye solo al recargar
        GameObject canvasGO = new GameObject("GameOverCanvas");

        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasGO.AddComponent<GraphicRaycaster>();
        canvasGroup = canvasGO.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        MakeImage(canvasGO.transform, "BG", bgColor, Vector2.zero, Vector2.one);
        MakeImage(canvasGO.transform, "LineTop", titleColor,
            new Vector2(0.15f, 0.672f), new Vector2(0.85f, 0.675f));
        MakeText(canvasGO.transform, "Title", gameOverTitle,
            new Vector2(0.1f, 0.55f), new Vector2(0.9f, 0.74f),
            titleColor, 90, FontStyle.Bold);
        MakeImage(canvasGO.transform, "LineBot", titleColor,
            new Vector2(0.15f, 0.548f), new Vector2(0.85f, 0.551f));
        MakeText(canvasGO.transform, "Subtitle", subtitleText,
            new Vector2(0.15f, 0.43f), new Vector2(0.85f, 0.54f),
            subtitleColor, 30, FontStyle.Italic);
        timerLabel = MakeText(canvasGO.transform, "Timer", $"Reiniciando en {(int)gameOverDisplayTime}...",
            new Vector2(0.25f, 0.30f), new Vector2(0.75f, 0.40f),
            timerColor, 26, FontStyle.Normal);
        MakeButton(canvasGO.transform,
            new Vector2(0.35f, 0.16f), new Vector2(0.65f, 0.25f),
            "Reiniciar ahora",
            new Color(0.65f, 0.08f, 0.08f, 1f),
            () => DoRestart());
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

    Text MakeText(Transform parent, string name, string content,
        Vector2 aMin, Vector2 aMax, Color color, int size, FontStyle style)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Text t = go.AddComponent<Text>();
        t.text = content;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = size; t.fontStyle = style;
        t.color = color; t.alignment = TextAnchor.MiddleCenter;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = aMin; rt.anchorMax = aMax;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        return t;
    }

    void MakeButton(Transform parent, Vector2 aMin, Vector2 aMax,
        string label, Color bg, UnityEngine.Events.UnityAction onClick)
    {
        GameObject go = new GameObject("Btn");
        go.transform.SetParent(parent, false);
        go.AddComponent<Image>().color = bg;
        Button btn = go.AddComponent<Button>();
        btn.onClick.AddListener(onClick);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = aMin; rt.anchorMax = aMax;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        MakeText(go.transform, "Lbl", label,
            Vector2.zero, Vector2.one, Color.white, 22, FontStyle.Normal);
    }

    IEnumerator FadeRealtime(float from, float to, float duration)
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
}