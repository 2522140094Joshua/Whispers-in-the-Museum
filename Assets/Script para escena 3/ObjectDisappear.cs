using System.Collections;
using UnityEngine;

/// <summary>
/// Coloca este script en el objeto que debe desaparecer (ej: el ATM).
/// Cuando el jugador presiona E cerca del objeto, este desaparece
/// y se muestran los créditos finales del juego.
/// </summary>
public class ObjectDisappear : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Distancia máxima para interactuar con el objeto")]
    public float interactionDistance = 3f;

    [Tooltip("Tecla para interactuar")]
    public KeyCode interactionKey = KeyCode.E;

    [Tooltip("Nombre del GameObject con CreditsManager en la escena")]
    public string creditsManagerName = "CreditsManager";

    [Header("Animación de desaparición")]
    [Tooltip("¿El objeto se desvanece gradualmente antes de desaparecer?")]
    public bool fadeOut = true;

    [Tooltip("Duración del desvanecimiento en segundos")]
    public float fadeDuration = 1.5f;

    // --- Referencias privadas ---
    private Transform player;
    private CreditsManager creditsManager;
    private bool hasDisappeared = false;
    private Renderer[] renderers;

    void Start()
    {
        // Buscar al jugador por tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("[ObjectDisappear] No se encontró un GameObject con tag 'Player'.");

        // Buscar el CreditsManager
        GameObject cm = GameObject.Find(creditsManagerName);
        if (cm != null)
            creditsManager = cm.GetComponent<CreditsManager>();
        else
            Debug.LogWarning($"[ObjectDisappear] No se encontró '{creditsManagerName}' en la escena.");

        // Guardar los renderers para el fade
        renderers = GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        if (hasDisappeared || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= interactionDistance && Input.GetKeyDown(interactionKey))
        {
            StartCoroutine(DisappearAndShowCredits());
        }
    }

    IEnumerator DisappearAndShowCredits()
    {
        hasDisappeared = true;

        if (fadeOut && renderers.Length > 0)
        {
            // Cambiar materiales a modo transparente para el fade
            foreach (Renderer r in renderers)
            {
                foreach (Material mat in r.materials)
                {
                    SetMaterialTransparent(mat);
                }
            }

            // Desvanecer gradualmente
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

                foreach (Renderer r in renderers)
                {
                    foreach (Material mat in r.materials)
                    {
                        Color c = mat.color;
                        c.a = alpha;
                        mat.color = c;
                    }
                }
                yield return null;
            }
        }

        // Desactivar el objeto
        gameObject.SetActive(false);

        // Mostrar créditos
        if (creditsManager != null)
            creditsManager.ShowCredits();
        else
            Debug.LogError("[ObjectDisappear] CreditsManager no encontrado. No se pueden mostrar los créditos.");
    }

    /// <summary>
    /// Configura un material para soportar transparencia (modo Fade).
    /// </summary>
    void SetMaterialTransparent(Material mat)
    {
        mat.SetFloat("_Mode", 2); // Fade mode en Standard Shader
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }

    // Dibuja el rango de interacción en el editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}