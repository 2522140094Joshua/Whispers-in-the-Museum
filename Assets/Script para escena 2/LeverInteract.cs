using UnityEngine;

public class LeverInteract : MonoBehaviour
{
    [Header("Configuracion")]
    public int leverID;
    public float rangoInteraccion = 2.5f;

    [Header("Prompt")]
    public GameObject promptUI;

    private bool activada = false;
    private bool playerCerca = false;
    private Animator anim;
    private AudioSource audioSource;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (promptUI != null) promptUI.SetActive(false);

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !activada)
        {
            playerCerca = true;
            if (promptUI != null) promptUI.SetActive(true);
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

        // Animacion — el parametro del asset se llama exactamente "LeverUp"
        if (anim != null)
            anim.SetBool("LeverUp", true);
        if (audioSource != null) audioSource.Play();
        LeverManager.Instance.ActivarPalanca(leverID);
    }

    public void Resetear()
    {
        activada = false;

        if (anim != null)
            anim.SetBool("LeverUp", false); // regresa la palanca
    }
}