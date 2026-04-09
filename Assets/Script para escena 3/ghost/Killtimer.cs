using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// TIMER DE 60 SEGUNDOS — CIERRE FORZADO
/// Cuenta regresiva. Al llegar a 0 cierra el juego como Alt+F4.
/// El fantasma emerge a mitad del timer automáticamente.
/// </summary>
public class KillTimer : MonoBehaviour
{
    [Header("=== TIMER ===")]
    public float tiempoTotal = 60f;

    [Tooltip("TextMeshPro que muestra el tiempo restante en pantalla")]
    public TMP_Text timerUI;

    [Tooltip("Color normal del timer")]
    public Color colorNormal = new Color(1f, 0.2f, 0.2f, 1f);

    [Tooltip("Color cuando quedan pocos segundos (parpadea)")]
    public Color colorUrgente = Color.red;

    [Tooltip("Segundos restantes para empezar el parpadeo")]
    public float urgentAt = 15f;

    [Header("=== FANTASMA (opcional) ===")]
    [Tooltip("Script GhostEmerge del fantasma — emerge a mitad del timer")]
    public GhostEmerge ghostEmerge;

    [Tooltip("¿A cuántos segundos restantes emerge el fantasma?")]
    public float ghostEmergeAt = 30f;
    private bool ghostTriggered = false;

    // --- Estado ---
    private float tiempoRestante;
    private bool activo = true;
    private bool finalTriggered = false;
    private bool blinkState = false;
    private float blinkTimer = 0f;

    void Start()
    {
        tiempoRestante = tiempoTotal;

        if (timerUI != null)
        {
            timerUI.color = colorNormal;
            timerUI.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (!activo || finalTriggered) return;

        tiempoRestante -= Time.deltaTime;

        // Actualizar UI
        if (timerUI != null)
        {
            int secs = Mathf.CeilToInt(Mathf.Max(tiempoRestante, 0f));
            timerUI.text = secs.ToString();

            // Parpadeo urgente
            if (tiempoRestante <= urgentAt)
            {
                blinkTimer += Time.deltaTime;
                if (blinkTimer >= 0.4f)
                {
                    blinkState = !blinkState;
                    timerUI.color = blinkState ? colorUrgente : Color.white;
                    blinkTimer = 0f;
                }
            }
        }

        // Emerger fantasma
        if (!ghostTriggered && ghostEmerge != null && tiempoRestante <= ghostEmergeAt)
        {
            ghostTriggered = true;
            ghostEmerge.EmergeGhost();
        }

        // Timer llegó a 0
        if (tiempoRestante <= 0f)
        {
            finalTriggered = true;
            activo = false;
            if (timerUI != null) timerUI.gameObject.SetActive(false);
            ForceQuit();
        }
    }

    void ForceQuit()
    {
        Debug.Log("[KillTimer] Tiempo agotado — cerrando juego.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
    }

    // --- Métodos públicos ---
    public void PausarTimer() { activo = false; }
    public void ReanudarTimer() { if (!finalTriggered) activo = true; }
    public void AgregarTiempo(float segundos)
    {
        tiempoRestante = Mathf.Min(tiempoRestante + segundos, tiempoTotal);
    }
}