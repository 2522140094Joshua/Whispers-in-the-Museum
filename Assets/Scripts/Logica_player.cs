using UnityEngine;

public class Logica_player : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 5f;
    public float rotationSpeed = 10f; // Ajustado para suavidad
    public bool puedeMoverse = true;

    [Header("Sprint")]
    public float velocidadSprint = 10f;
    public KeyCode teclaSprint = KeyCode.LeftShift;
    private bool corriendo = false;

    private Rigidbody rb;
    private Animator animator;
    private float x, y;
    private Vector3 direccionMovimiento;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // Evita que el muñeco se tropiece y caiga de cara
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        if (puedeMoverse)
        {
            x = Input.GetAxis("Horizontal");
            y = Input.GetAxis("Vertical");
            corriendo = Input.GetKey(teclaSprint) && y > 0;

            // Animaciones
            animator.SetFloat("SpeedX", x);
            animator.SetFloat("SpeedY", corriendo ? y * 2f : y);
        }
        else
        {
            x = 0;
            y = 0;
            animator.SetFloat("SpeedX", 0);
            animator.SetFloat("SpeedY", 0);
        }
    }

    // La física SIEMPRE va en FixedUpdate
    void FixedUpdate()
    {
        if (!puedeMoverse) return;

        float velocidadActual = corriendo ? velocidadSprint : speed;

        // Calculamos el movimiento relativo a la orientación del personaje
        Vector3 movimiento = transform.forward * y * velocidadActual * Time.fixedDeltaTime;

        // Movemos manteniendo la velocidad actual de caída (Y) del Rigidbody
        rb.MovePosition(rb.position + movimiento);

        // Rotación simple con las teclas A y D
        if (x != 0)
        {
            Quaternion deltaRotation = Quaternion.Euler(Vector3.up * x * rotationSpeed);
            rb.MoveRotation(rb.rotation * deltaRotation);
        }
    }

    private void Awake()
    {
        // Asegúrate de que no haya dos reproductores al volver a una escena
        if (FindObjectsOfType<Logica_player>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
}