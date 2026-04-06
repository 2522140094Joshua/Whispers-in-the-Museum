using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GeneradorSecuencia : MonoBehaviour
{
    [Header("Configuración")]
    public int[] ordenCorrecto = { 1, 3, 2 };
    private int pasoActual = 0;
    private bool resuelto = false;

    [Header("Referencias de UI")]
    public GameObject panelError;
    public GameObject panelExito;

    [Header("Referencias de Escena")]
    public Light luzLed;
    public Boton3D[] todosLosBotones; // Arrastra los 3 cubos aquí

    void Start()
    {
        if (panelError != null) panelError.SetActive(false);
        if (panelExito != null) panelExito.SetActive(false);
    }

    // Nota: Ahora recibe el ID y el Script del botón físico
    public void BotonPresionado(int numeroBoton, Boton3D botonFisico)
    {
        if (resuelto) return;

        if (numeroBoton == ordenCorrecto[pasoActual])
        {
            // ACIERTO: Pintar verde
            botonFisico.PonerVerde();
            pasoActual++;
            Debug.Log("¡Correcto! Paso: " + pasoActual);

            if (pasoActual >= ordenCorrecto.Length)
            {
                FinalizarPuzzle();
            }
        }
        else
        {
            // ERROR: Pintar rojo y mostrar ventana
            botonFisico.PonerRojo();
            pasoActual = 0;
            Debug.Log("Error en la secuencia");
            Invoke("MostrarError", 0.2f);
        }
    }

    void MostrarError()
    {
        panelError.SetActive(true);
        ControlarMouseyPlayer(true);
    }

    void FinalizarPuzzle()
    {
        resuelto = true;
        if (luzLed != null) luzLed.color = Color.green;
        panelExito.SetActive(true);
        ControlarMouseyPlayer(true);

        Object.FindAnyObjectByType<ControladorPuzzle>().GeneradorCompletado();
    }

    public void CerrarVentanaUI()
    {
        panelError.SetActive(false);
        panelExito.SetActive(false);

        // Resetear todos los cubos a blanco
        foreach (Boton3D b in todosLosBotones)
        {
            b.ResetearColor();
        }

        ControlarMouseyPlayer(false);
    }

    void ControlarMouseyPlayer(bool activarUI)
    {
        Cursor.lockState = activarUI ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = activarUI;

        Logica_player p = Object.FindAnyObjectByType<Logica_player>();
        if (p != null) p.puedeMoverse = !activarUI;
    }
}