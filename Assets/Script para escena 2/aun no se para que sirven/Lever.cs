using UnityEngine;
/*
public class Lever : MonoBehaviour
{
    [Header("Configuracion")]
    public int leverID; // numero unico: 0, 1, 2, 3...
    public float interactionRange = 2f;

    [Header("Visual")]
    public GameObject promptUI;
    public Animator leverAnimator; // opcional, si tiene animacion
    public string animacionActivar = "Activar";

    private bool activada = false;
    private bool playerCerca = false;

    void Start()
    {
        LeverManager.Instance.RegistrarPalanca(this);
        if (promptUI != null) promptUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCerca = true;
            if (!activada && promptUI != null)
                promptUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCerca = false;
            if (promptUI != null) promptUI.SetActive(false);
        }
    }

    void Update()
    {
        if (playerCerca && !activada && Input.GetKeyDown(KeyCode.E))
            Activar();
    }

    void Activar()
    {
        activada = true;
        if (promptUI != null) promptUI.SetActive(false);

        if (leverAnimator != null)
            leverAnimator.Play(animacionActivar);

        LeverManager.Instance.ActivarPalanca(leverID);
    }

    public void Resetear()
    {
        activada = false;
        // si quieres animacion de reset puedes agregarla aqui
    }
}
*/