using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Sensibilidad")]
    public float sensibilidadX = 2f;
    public float sensibilidadY = 2f;

    [Header("Limite vertical")]
    public float limiteArriba = 60f;
    public float limiteAbajo = -60f;

    [Header("Referencias")]
    public Transform cuerpoJugador;  // raiz del Player
    public Transform huesoCabeza;    // head bone (solo para posicion)

    private float rotacionVertical = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        // Ignorar si el inventario esta abierto
        if (Cursor.lockState != CursorLockMode.Locked) return;

        float mouseX = Input.GetAxis("Mouse X") * sensibilidadX;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadY;

        // Rotacion vertical con limite
        rotacionVertical -= mouseY;
        rotacionVertical = Mathf.Clamp(rotacionVertical, limiteAbajo, limiteArriba);

        // Cuerpo rota horizontalmente con el mouse
        if (cuerpoJugador != null)
            cuerpoJugador.Rotate(Vector3.up * mouseX);

        // Camara sigue la POSICION del hueso pero ignora su rotacion de animacion
        if (huesoCabeza != null)
            transform.position = huesoCabeza.position;

        // Solo aplica la rotacion del mouse, sin el bamboleo
        transform.rotation = cuerpoJugador.rotation * Quaternion.Euler(rotacionVertical, 0f, 0f);
    }
}