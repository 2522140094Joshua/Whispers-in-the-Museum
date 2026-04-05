using UnityEngine;

public class BotonInteractuable : MonoBehaviour
{
    public int idBoton; // 1, 2 o 3
    public GeneradorSecuencia scriptPrincipal;
    private bool jugadorCerca = false;

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            scriptPrincipal.BotonPresionado(idBoton);
            // Opcional: Que el botón se hunda o brille al tocarlo
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) jugadorCerca = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) jugadorCerca = false;
    }
}