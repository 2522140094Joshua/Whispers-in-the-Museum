using UnityEngine;
using TMPro;

public class GeneradorPalancas : MonoBehaviour
{
    [Header("Combinación Correcta (0=Abajo, 1=Arriba)")]
    public int[] combinacionObjetivo = { 1, 0, 1, 1 };
    private int[] estadoActual = { 0, 0, 0, 0 };

    [Header("Referencias UI")]
    public GameObject panelExito;
    public Light luzLed;

    private bool resuelto = false;

    // Esta función la llama cada palanca al darle a la 'E'
    public void CambiarPalanca(int indice)
    {
        if (resuelto) return;

        // Cambia de 0 a 1, o de 1 a 0
        estadoActual[indice] = (estadoActual[indice] == 0) ? 1 : 0;

        Debug.Log("Palanca " + indice + " ahora está en: " + estadoActual[indice]);

        VerificarCombinacion();
    }

    void VerificarCombinacion()
    {
        for (int i = 0; i < combinacionObjetivo.Length; i++)
        {
            if (estadoActual[i] != combinacionObjetivo[i]) return;
            // Si una no coincide, sale de la función y no pasa nada
        }

        // Si el ciclo termina, es que todas coinciden
        Victoria();
    }

    void Victoria()
    {
        resuelto = true;
        luzLed.color = Color.green;
        panelExito.SetActive(true);

        // Bloquear mouse y player para el mensaje final
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Object.FindAnyObjectByType<Logica_player>().puedeMoverse = false;

        // ¡EL MOMENTO FINAL! 3/3
        Object.FindAnyObjectByType<ControladorPuzzle>().GeneradorCompletado();
    }

    public void CerrarMensajeFinal()
    {
        panelExito.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Object.FindAnyObjectByType<Logica_player>().puedeMoverse = true;
    }

    public void CerrarVentanaFinal()
    {
        panelExito.SetActive(false); // Apaga el panel verde

        // Regresamos el mouse y el movimiento a la normalidad
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Logica_player p = Object.FindAnyObjectByType<Logica_player>();
        if (p != null) p.puedeMoverse = true;

        Debug.Log("Ventana final cerrada. ¡Corre a la salida!");
    }

}