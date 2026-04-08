using UnityEngine;
using System.Collections;

public class LeverManager : MonoBehaviour
{
    public static LeverManager Instance;

    [Header("Orden correcto (IDs de las palancas)")]
    public int[] ordenCorrecto = { 0, 2, 4, 1, 3 };

    [Header("Al completar")]
    public GameObject chest;       // el chest que se destruye
    public DoorOpener doorOpener;  // la puerta que se abre
    public string siguienteNivel = "Escena 3";

    private int pasoActual = 0;
    private LeverInteract[] todasLasPalancas;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        todasLasPalancas = FindObjectsOfType<LeverInteract>();
    }

    public void ActivarPalanca(int id)
    {
        if (ordenCorrecto[pasoActual] == id)
        {
            pasoActual++;
            Debug.Log($"✅ Correcto! {pasoActual}/{ordenCorrecto.Length}");

            if (pasoActual >= ordenCorrecto.Length)
                StartCoroutine(Completado());
        }
        else
        {
            Debug.Log("❌ Orden incorrecto, reseteando...");
            StartCoroutine(ResetearTodo());
        }
    }

    IEnumerator Completado()
    {
        yield return new WaitForSeconds(0.5f);

        // Rompe/destruye el chest
        if (chest != null)
            Destroy(chest);

        yield return new WaitForSeconds(0.3f);

        // Abre la puerta
        if (doorOpener != null)
            doorOpener.OpenDoor();
    }

    IEnumerator ResetearTodo()
    {
        yield return new WaitForSeconds(0.3f);
        pasoActual = 0;
        foreach (LeverInteract p in todasLasPalancas)
            p.Resetear();
    }
}