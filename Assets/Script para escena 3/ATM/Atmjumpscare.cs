using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ATM FALSO - Nivel 3
/// Jugador presiona E → ATM desaparece → risa → suspenso → imagen del jumpscare.
/// Sin sonido de grito.
/// </summary>
public class ATMJumpscare : MonoBehaviour
{
    [Header("=== INTERACCIÓN ===")]
    public KeyCode interactionKey = KeyCode.E;
    public float interactionRange = 2.5f;

    [Header("=== DESAPARICIÓN DEL ATM ===")]
    public float atmFadeDuration = 0.4f;

    [Header("=== SONIDO ===")]
    [Tooltip("Risa que suena cuando el ATM desaparece")]
    public AudioClip laughClip;
    [Range(0f, 1f)]
    public float laughVolume = 1f;

    [Header("=== JUMPSCARE ===")]
    public Sprite[] jumpscarSprites;
    public float jumpscareDuration = 1.2f;

    [Tooltip("Delay base antes del primer jumpscare")]
    public float baseJumpscareDelay = 1.0f;

    [Tooltip("Segundos extra de suspenso por cada ATM falso tocado")]
    public float delayIncreasePerFail = 0.8f;

    [Tooltip("Máximo delay de suspenso")]
    public float maxJumpscareDelay = 6f;

    [Header("=== PROMPT UI (opcional) ===")]
    public GameObject promptUI;

    private static int globalFailCount = 0;
    public static void ResetFailCount() { globalFailCount = 0; }

    private Transform player;
    private bool used = false;
    private bool playerNearby = false;

    private GameObject jumpscarCanvas;
    private Image jumpscarImage;
    private CanvasGroup jumpscarCG;
    private Renderer[] atmRenderers;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        else Debug.LogWarning("[ATMJumpscare] No se encontró Player.");

        atmRenderers = GetComponentsInChildren<Renderer>();
        if (promptUI != null) promptUI.SetActive(false);
        BuildJumpscarUI();
    }

    void Update()
    {
        if (player == null || used) return;

        float dist = Vector3.Distance(transform.position, player.position);
        bool inRange = dist <= interactionRange;

        if (promptUI != null && inRange != playerNearby)
            promptUI.SetActive(inRange);

        playerNearby = inRange;

        if (inRange && Input.GetKeyDown(interactionKey))
        {
            used = true;
            globalFailCount++;
            if (promptUI != null) promptUI.SetActive(false);
            StartCoroutine(FalseATMSequence());
        }
    }

    IEnumerator FalseATMSequence()
    {
        // 1. ATM desaparece
        yield return StartCoroutine(FadeOutATM());
        gameObject.SetActive(false);

        // 2. Risa
        if (laughClip != null)
            AudioSource.PlayClipAtPoint(laughClip, transform.position, laughVolume);

        // 3. Suspenso (crece con cada fallo)
        float delay = Mathf.Min(
            baseJumpscareDelay + (globalFailCount - 1) * delayIncreasePerFail,
            maxJumpscareDelay);

        Debug.Log($"[ATMJumpscare] Fallo #{globalFailCount} — Jumpscare en {delay:F1}s");
        yield return new WaitForSeconds(delay);

        // 4. Solo imagen
        yield return StartCoroutine(ShowJumpscare());
    }

    IEnumerator ShowJumpscare()
    {
        if (jumpscarSprites == null || jumpscarSprites.Length == 0)
        {
            Debug.LogWarning("[ATMJumpscare] No hay sprites asignados.");
            yield break;
        }

        jumpscarImage.sprite = jumpscarSprites[Random.Range(0, jumpscarSprites.Length)];
        jumpscarCanvas.SetActive(true);

        yield return StartCoroutine(FadeCanvas(0f, 1f, 0.04f));
        yield return new WaitForSeconds(jumpscareDuration);
        yield return StartCoroutine(FadeCanvas(1f, 0f, 0.15f));

        jumpscarCanvas.SetActive(false);
    }

    IEnumerator FadeOutATM()
    {
        foreach (Renderer r in atmRenderers)
            foreach (Material mat in r.materials)
                SetTransparent(mat);

        float elapsed = 0f;
        while (elapsed < atmFadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / atmFadeDuration);
            foreach (Renderer r in atmRenderers)
                foreach (Material mat in r.materials)
                {
                    Color c = mat.color; c.a = alpha; mat.color = c;
                }
            yield return null;
        }
    }

    void SetTransparent(Material mat)
    {
        mat.SetFloat("_Mode", 2);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }

    IEnumerator FadeCanvas(float from, float to, float duration)
    {
        float e = 0f; jumpscarCG.alpha = from;
        while (e < duration)
        {
            e += Time.deltaTime;
            jumpscarCG.alpha = Mathf.Lerp(from, to, e / duration);
            yield return null;
        }
        jumpscarCG.alpha = to;
    }

    void BuildJumpscarUI()
    {
        jumpscarCanvas = new GameObject("JumpscarCanvas");
        Canvas c = jumpscarCanvas.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        c.sortingOrder = 9998;
        jumpscarCanvas.AddComponent<CanvasScaler>();
        jumpscarCanvas.AddComponent<GraphicRaycaster>();

        jumpscarCG = jumpscarCanvas.AddComponent<CanvasGroup>();
        jumpscarCG.alpha = 0f;
        jumpscarCG.interactable = false;
        jumpscarCG.blocksRaycasts = false;

        GameObject bg = new GameObject("BG");
        bg.transform.SetParent(jumpscarCanvas.transform, false);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = Color.black;
        RectTransform bgRT = bg.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero; bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = bgRT.offsetMax = Vector2.zero;

        GameObject imgGO = new GameObject("JumpscarImg");
        imgGO.transform.SetParent(jumpscarCanvas.transform, false);
        jumpscarImage = imgGO.AddComponent<Image>();
        jumpscarImage.color = Color.white;
        jumpscarImage.preserveAspect = true;
        RectTransform imgRT = imgGO.GetComponent<RectTransform>();
        imgRT.anchorMin = new Vector2(0.05f, 0.05f);
        imgRT.anchorMax = new Vector2(0.95f, 0.95f);
        imgRT.offsetMin = imgRT.offsetMax = Vector2.zero;

        jumpscarCanvas.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.2f, 0f, 0.8f);
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}