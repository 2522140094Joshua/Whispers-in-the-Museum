using UnityEngine;

public class PuzzleBox : MonoBehaviour
{
    [Header("Configuracion")]
    public int boxID;
    public float interactionRange = 2f;
    public GameObject promptUI;

    private bool resuelta = false;
    private bool playerCerca = false;

    void Start()
    {
        if (promptUI != null) promptUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !resuelta)
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
        if (playerCerca && !resuelta && Input.GetKeyDown(KeyCode.E))
            AbrirPuzzle();
    }

    void AbrirPuzzle()
    {
        // Aqui abres tu UI del puzzle (el panel del Activation Station)
        PuzzleManager.Instance.MostrarPuzzle(boxID, this);
    }

    public void Resolver()
    {
        resuelta = true;
        if (promptUI != null) promptUI.SetActive(false);
        gameObject.SetActive(false); // desaparece la caja
        PuzzleManager.Instance.CheckCompletado();
    }
}