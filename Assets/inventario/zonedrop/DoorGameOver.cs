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
    private GameObject gameOverCanvas;
    private Vector3 playerStartPos;
    private GameObject player;



    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            playerStartPos = player.transform.position;
    }
    public void TriggerGameOver()
    {
        StartCoroutine(ResetLevel());
    }

    IEnumerator ResetLevel()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            player.SetActive(false);

        yield return new WaitForSeconds(1f);

        // Reset inventario (recomendado)
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.ResetInventory();

        // Reset zona (MUY IMPORTANTE)
        PaintingDropZone zone = FindObjectOfType<PaintingDropZone>();
        if (zone != null)
            zone.ResetZone();

        // Reset puerta
        DoorOpener door = GetComponent<DoorOpener>();
        if (door != null)
            door.CloseDoor();

        // Regresar jugador
        if (player != null)
        {
            player.transform.position = playerStartPos;
            player.SetActive(true);
        }
        ResettableObject[] objetos = FindObjectsOfType<ResettableObject>(true);

        foreach (var obj in objetos)
        {
            obj.ResetObject();
        }
    }
    IEnumerator GameOverSequence()
    {
        // Bloquear cursor sin tocar scripts del jugador
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Esperar en tiempo real para que la puerta cierre
        yield return new WaitForSecondsRealtime(delayBeforeGameOver);

       

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
        if (canvasGO != null)
        {
            Destroy(canvasGO);
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ResetInventory();
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Destroy(player);
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private GameObject canvasGO;
   
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