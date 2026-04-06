using UnityEngine;

public class ControlLinterna : MonoBehaviour
{
    public Light luzLinterna; // Arrastra aquí tu Spotlight
    public bool encendida = true;

    void Update()
    {
        // Si presionas la tecla F, se PRENDE
        if (Input.GetKeyDown(KeyCode.F))
        {
            Encender(true);
        }

        // Si presionas la tecla G, se APAGA
        if (Input.GetKeyDown(KeyCode.G))
        {
            Encender(false);
        }

        
    }

    void Encender(bool estado)
    {
        encendida = estado;
        luzLinterna.enabled = encendida;
        Debug.Log("Linterna: " + (encendida ? "Encendida" : "Apagada"));
    }
}