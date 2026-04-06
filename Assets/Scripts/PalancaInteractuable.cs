using UnityEngine;

public class PalancaInteractuable : MonoBehaviour
{
    public int idPalanca; // 0, 1, 2, 3
    public GeneradorPalancas scriptPrincipal;
    private bool jugadorCerca = false;

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            // Mover la palanca visualmente (rotación simple)
            transform.Rotate(45, 0, 0); // Esto es un ejemplo, ajusta según tu modelo

            scriptPrincipal.CambiarPalanca(idPalanca);
        }
    }

    private void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) jugadorCerca = true; }
    private void OnTriggerExit(Collider other) { if (other.CompareTag("Player")) jugadorCerca = false; }
}