using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Coloca este script en un GameObject vacío llamado "UISetup" en tu escena.
/// Al darle Play crea automáticamente el Canvas del Prompt ("Presiona E")
/// y lo asigna al ATMJumpscare más cercano.
/// 
/// ÚSALO UNA VEZ para generar la UI, luego puedes borrar este script.
/// </summary>
public class JumpscarePromptSetup : MonoBehaviour
{
    void Awake()
    {
        GameObject prompt = BuildPromptUI();

        // Buscar ATMJumpscare en la escena y asignarle el prompt
        ATMJumpscare atm = FindObjectOfType<ATMJumpscare>();
        if (atm != null)
        {
            atm.promptUI = prompt;
            Debug.Log("[JumpscarePromptSetup] Prompt asignado al ATMJumpscare.");
        }
        else
        {
            Debug.LogWarning("[JumpscarePromptSetup] No se encontró ATMJumpscare en la escena.");
        }

        // Auto-destruir este script después de configurar
        Destroy(this);
    }

    GameObject BuildPromptUI()
    {
        // ── Canvas ────────────────────────────────────────────────────────
        GameObject canvasGO = new GameObject("PromptCanvas_ATM");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        // ── Panel fondo semitransparente ──────────────────────────────────
        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(canvasGO.transform, false);

        Image panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0f, 0f, 0f, 0.55f);

        RectTransform panelRT = panel.GetComponent<RectTransform>();
        // Centrado abajo de la pantalla
        panelRT.anchorMin = new Vector2(0.5f, 0.08f);
        panelRT.anchorMax = new Vector2(0.5f, 0.08f);
        panelRT.pivot = new Vector2(0.5f, 0.5f);
        panelRT.sizeDelta = new Vector2(340f, 60f);

        // ── Tecla [E] ────────────────────────────────────────────────────
        GameObject keyBox = new GameObject("KeyBox");
        keyBox.transform.SetParent(panel.transform, false);

        Image keyImg = keyBox.AddComponent<Image>();
        keyImg.color = new Color(0.15f, 0.15f, 0.15f, 1f);

        RectTransform keyRT = keyBox.GetComponent<RectTransform>();
        keyRT.anchorMin = new Vector2(0f, 0.5f);
        keyRT.anchorMax = new Vector2(0f, 0.5f);
        keyRT.pivot = new Vector2(0f, 0.5f);
        keyRT.anchoredPosition = new Vector2(12f, 0f);
        keyRT.sizeDelta = new Vector2(44f, 44f);

        // Borde de la tecla
        Outline keyOutline = keyBox.AddComponent<Outline>();
        keyOutline.effectColor = new Color(1f, 1f, 1f, 0.6f);
        keyOutline.effectDistance = new Vector2(1.5f, -1.5f);

        // Letra E
        GameObject keyText = new GameObject("KeyLabel");
        keyText.transform.SetParent(keyBox.transform, false);
        TMP_Text keyLabel = keyText.AddComponent<TextMeshProUGUI>();
        keyLabel.text = "E";
        keyLabel.fontSize = 26;
        keyLabel.fontStyle = FontStyles.Bold;
        keyLabel.color = Color.white;
        keyLabel.alignment = TextAlignmentOptions.Center;
        RectTransform keyLabelRT = keyText.GetComponent<RectTransform>();
        keyLabelRT.anchorMin = Vector2.zero;
        keyLabelRT.anchorMax = Vector2.one;
        keyLabelRT.offsetMin = keyLabelRT.offsetMax = Vector2.zero;

        // ── Texto "Interactuar" ───────────────────────────────────────────
        GameObject textGO = new GameObject("ActionLabel");
        textGO.transform.SetParent(panel.transform, false);
        TMP_Text actionLabel = textGO.AddComponent<TextMeshProUGUI>();
        actionLabel.text = "Interactuar";
        actionLabel.fontSize = 22;
        actionLabel.color = new Color(0.95f, 0.95f, 0.95f, 1f);
        actionLabel.alignment = TextAlignmentOptions.MidlineLeft;

        RectTransform actionRT = textGO.GetComponent<RectTransform>();
        actionRT.anchorMin = new Vector2(0f, 0.5f);
        actionRT.anchorMax = new Vector2(1f, 0.5f);
        actionRT.pivot = new Vector2(0f, 0.5f);
        actionRT.anchoredPosition = new Vector2(68f, 0f);
        actionRT.sizeDelta = new Vector2(-80f, 40f);

        // Empieza oculto
        canvasGO.SetActive(false);

        return canvasGO;
    }
}