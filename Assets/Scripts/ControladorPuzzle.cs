using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class ControladorPuzzle : MonoBehaviour
{
    [Header("Configuración del Nivel")]
    public int generadoresTotales = 3;
    private int generadoresActivos = 0;
    
    [Header("Interfaz")]
    public TextMeshProUGUI textoContador;


    [Tooltip("Arrastra aquí el objeto que bloquea la salida")]
    public GameObject puertaSalida;

    // Esta función la llaman los generadores cuando alguien acierta
    public void GeneradorCompletado()
    {
        generadoresActivos++;
        ActualizarInterfaz();
        Debug.Log("Generadores listos: " + generadoresActivos + "/" + generadoresTotales);

        if (generadoresActivos >= generadoresTotales)
        {
            AbrirPuerta();
            textoContador.text = "¡SISTEMA REINICIADO - ESCAPA!";
            textoContador.color = Color.green;
        }
    }

    void ActualizarInterfaz()
    {
        if (textoContador != null)
        {
            textoContador.text = "Generadores: " + generadoresActivos + " / " + generadoresTotales;
        }
    }

    void AbrirPuerta()
    {
        if (puertaSalida != null)
        {
            Debug.Log("¡Puerta de seguridad abierta!");
            puertaSalida.SetActive(false); // La puerta desaparece
        }
        else
        {
            Debug.LogWarning("No asignaste el objeto de la puerta en el Inspector.");
        }
    }
}