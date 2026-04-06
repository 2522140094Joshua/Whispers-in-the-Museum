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
    public Transform huesoCabeza;    // mixamorig:Head (no HeadTop_End)

    private float rotacionVertical = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Si no se asigno en el Inspector, busca el Player automaticamente
        if (cuerpoJugador == null)
            cuerpoJugador = GameObject.FindWithTag("Player").transform;
    }

    void LateUpdate()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;
        if (cuerpoJugador == null) return; // seguridad extra

        float mouseX = Input.GetAxis("Mouse X") * sensibilidadX;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadY;

        // Rotacion vertical con limite
        rotacionVertical -= mouseY;
        rotacionVertical = Mathf.Clamp(rotacionVertical, limiteAbajo, limiteArriba);

        // Cuerpo rota horizontalmente
        cuerpoJugador.Rotate(Vector3.up * mouseX);

        // Camara sigue la posicion del hueso de la cabeza
        if (huesoCabeza != null)
            transform.position = huesoCabeza.position;

        // Rotacion solo del mouse, sin heredar bamboleo de animacion
        float yawMundo = cuerpoJugador.eulerAngles.y;
        transform.rotation = Quaternion.Euler(rotacionVertical, yawMundo, 0f);
    }
}