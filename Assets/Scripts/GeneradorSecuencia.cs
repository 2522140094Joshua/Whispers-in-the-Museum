using UnityEngine;

public class GeneradorSecuencia : MonoBehaviour
{
    [Header("Configuración")]
    public int[] ordenCorrecto = { 1, 3, 2 }; // El orden de los botones
    private int pasoActual = 0;

    public Light luzLed;
    private bool resuelto = false;

    // Esta función la llamarán los botones individuales
    public void BotonPresionado(int numeroBoton)
    {
        if (resuelto) return;

        if (numeroBoton == ordenCorrecto[pasoActual])
        {
            pasoActual++;
            Debug.Log("¡Boton correcto! Paso: " + pasoActual);

            if (pasoActual >= ordenCorrecto.Length)
            {
                FinalizarPuzzle();
            }
        }
        else
        {
            Debug.Log("Orden incorrecto. Reiniciando...");
            pasoActual = 0;
            // Aquí podrías poner un sonido de error
        }
    }

    void FinalizarPuzzle()
    {
        resuelto = true;
        luzLed.color = Color.green;
        Object.FindAnyObjectByType<ControladorPuzzle>().GeneradorCompletado();
    }
}