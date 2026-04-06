using UnityEngine;

public class Boton3D : MonoBehaviour
{
    [Header("Configuración")]
    public int idBoton;
    public GeneradorSecuencia scriptCentral;

    private MeshRenderer miRenderer;
    private Color colorOriginal;
    private bool jugadorCerca = false;

    void Start()
    {
        miRenderer = GetComponent<MeshRenderer>();
        colorOriginal = miRenderer.material.color;
    }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            if (scriptCentral != null)
            {
                // Enviamos el ID y "this" (este script) al jefe
                scriptCentral.BotonPresionado(idBoton, this);
            }
        }
    }

    public void PonerVerde() { miRenderer.material.color = Color.green; }
    public void PonerRojo() { miRenderer.material.color = Color.red; }
    public void ResetearColor() { miRenderer.material.color = colorOriginal; }

    private void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) jugadorCerca = true; }
    private void OnTriggerExit(Collider other) { if (other.CompareTag("Player")) jugadorCerca = false; }
}