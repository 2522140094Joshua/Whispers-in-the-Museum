using System.Collections;
using UnityEngine;

/// <summary>
/// FANTASMA QUE EMERGE DEL SUELO
/// 
/// Coloca este script en el GameObject del fantasma.
/// El fantasma empieza bajo el suelo (oculto) y cuando se activa:
///   1. Sube lentamente del suelo (emerge)
///   2. Gira para mirar al jugador
///   3. Se queda mirándolo (sigue su posición)
///   4. Opcionalmente vuelve a hundirse después de un tiempo
/// 
/// SETUP:
///  - Coloca el fantasma enterrado bajo el suelo en la escena
///  - Asigna este script
///  - Llama a EmergeGhost() desde otro script cuando quieras activarlo
///    (ej: cuando el timer llegue a cierto punto, o al entrar a un área)
/// </summary>
public class GhostEmerge : MonoBehaviour
{
    [Header("=== EMERGE ===")]
    [Tooltip("Posición Y donde el fantasma está oculto bajo el suelo")]
    public float hiddenY = -3f;

    [Tooltip("Posición Y donde el fantasma queda visible (nivel del suelo o encima)")]
    public float visibleY = 0.5f;

    [Tooltip("Velocidad a la que sube del suelo")]
    public float emergeSpeed = 1.2f;

    [Tooltip("Segundos que tarda en empezar a subir después de ser activado")]
    public float emergeDelay = 0f;

    [Header("=== SEGUIR AL JUGADOR ===")]
    [Tooltip("¿El fantasma sigue mirando al jugador después de emerger?")]
    public bool trackPlayer = true;

    [Tooltip("Qué tan rápido gira para mirar al jugador")]
    public float rotationSpeed = 2f;

    [Header("=== HUNDIRSE (opcional) ===")]
    [Tooltip("¿El fantasma vuelve a hundirse después de un tiempo?")]
    public bool sinkAfterTime = false;

    [Tooltip("Segundos que permanece visible antes de hundirse")]
    public float visibleDuration = 5f;

    [Tooltip("Velocidad a la que se hunde")]
    public float sinkSpeed = 1.5f;

    [Header("=== SONIDO (opcional) ===")]
    public AudioClip emergeSound;
    [Range(0f, 1f)]
    public float soundVolume = 0.8f;

    // --- Privadas ---
    private Transform player;
    private bool isEmerging = false;
    private bool hasEmerged = false;
    private bool isSinking = false;

    void Start()
    {
        // Comenzar oculto bajo el suelo
        Vector3 pos = transform.position;
        pos.y = hiddenY;
        transform.position = pos;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        else Debug.LogWarning("[GhostEmerge] No se encontró Player.");
    }

    void Update()
    {
        // ── Subir del suelo ───────────────────────────────────────────────
        if (isEmerging && !hasEmerged)
        {
            Vector3 pos = transform.position;
            pos.y = Mathf.MoveTowards(pos.y, visibleY, emergeSpeed * Time.deltaTime);
            transform.position = pos;

            if (Mathf.Abs(pos.y - visibleY) < 0.01f)
            {
                pos.y = visibleY;
                transform.position = pos;
                isEmerging = false;
                hasEmerged = true;

                if (sinkAfterTime)
                    StartCoroutine(SinkAfterDelay());
            }
        }

        // ── Mirar al jugador ──────────────────────────────────────────────
        if (hasEmerged && !isSinking && trackPlayer && player != null)
        {
            Vector3 dir = player.position - transform.position;
            dir.y = 0f; // solo girar en Y (horizontal)
            if (dir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }

        // ── Hundirse ──────────────────────────────────────────────────────
        if (isSinking)
        {
            Vector3 pos = transform.position;
            pos.y = Mathf.MoveTowards(pos.y, hiddenY, sinkSpeed * Time.deltaTime);
            transform.position = pos;

            if (Mathf.Abs(pos.y - hiddenY) < 0.01f)
            {
                pos.y = hiddenY;
                transform.position = pos;
                isSinking = false;
                hasEmerged = false;
                gameObject.SetActive(false);
            }
        }
    }

    // -------------------------------------------------------
    // LLAMAR ESTO PARA ACTIVAR EL FANTASMA
    // -------------------------------------------------------
    public void EmergeGhost()
    {
        if (isEmerging || hasEmerged) return;
        gameObject.SetActive(true);
        StartCoroutine(EmergeWithDelay());
    }

    IEnumerator EmergeWithDelay()
    {
        yield return new WaitForSeconds(emergeDelay);

        if (emergeSound != null)
            AudioSource.PlayClipAtPoint(emergeSound, transform.position, soundVolume);

        isEmerging = true;
    }

    IEnumerator SinkAfterDelay()
    {
        yield return new WaitForSeconds(visibleDuration);
        isSinking = true;
        hasEmerged = false;
    }

    // Para llamar desde otro script si quieres hundirlo manualmente
    public void SinkGhost()
    {
        if (!hasEmerged) return;
        StopAllCoroutines();
        isSinking = true;
        hasEmerged = false;
    }

    void OnDrawGizmosSelected()
    {
        // Línea mostrando recorrido de emerge
        Gizmos.color = new Color(0.5f, 0f, 1f, 0.8f);
        Vector3 base3 = transform.position; base3.y = hiddenY;
        Vector3 top = transform.position; top.y = visibleY;
        Gizmos.DrawLine(base3, top);
        Gizmos.DrawWireSphere(top, 0.3f);
    }
}