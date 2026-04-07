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
            // BUSCAMOS EL CONTADOR GLOBAL
            ControladorPuzzle gestor = Object.FindAnyObjectByType<ControladorPuzzle>();

            // EL BLOQUEO: Si el contador es menor a 2, no hace nada
            if (gestor != null && gestor.generadoresActivos < 2)
            {
                Debug.Log("Reactor 3 Bloqueado: Faltan generadores.");

                // Feedback visual en el texto de arriba
                gestor.textoContador.text = "ERROR: ACTIVA 2 GENERADORES";
                gestor.textoContador.color = Color.red;

                // Restaurar texto normal después de 1.5 segundos
                Invoke("ResetTexto", 1.5f);
                return; // AQUÍ SE CORTA: La palanca no se mueve ni avisa al script principal
            }

            // SI PASA EL BLOQUEO (ya hay 2 listos):
            transform.Rotate(45, 0, 0);
            scriptPrincipal.CambiarPalanca(idPalanca);
        }
    }

    void ResetTexto()
    {
        ControladorPuzzle gestor = Object.FindAnyObjectByType<ControladorPuzzle>();
        if (gestor != null)
        {
            gestor.textoContador.color = Color.white;
            gestor.ActualizarInterfaz();
        }
    }

    private void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) jugadorCerca = true; }
    private void OnTriggerExit(Collider other) { if (other.CompareTag("Player")) jugadorCerca = false; }
}