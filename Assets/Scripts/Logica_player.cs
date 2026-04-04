using UnityEngine;

public class Logica_player : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 5f;
    public float jumpForce = 5f;

    [Header("Sprint")]
    public float velocidadSprint = 10f;
    public KeyCode teclaSprint = KeyCode.LeftShift;

    [Header("Cámara Primera Persona")]
    public Transform camaraJugador;
    public float mouseSensitivity = 100f;

    private float xRotation = 0f;
    private float yRotation = 0f;  // ← acumulamos rotación Y total
    private Rigidbody rb;
    private Animator animator;
    private Vector3 inputDir;
    private bool corriendo;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.freezeRotation = true;

        // Sincronizamos yRotation con la rotación actual del personaje
        yRotation = transform.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            animator.SetFloat("SpeedY", 0f, 0.1f, Time.deltaTime);
            return;
        }

        MoverCamara();
        ManejarMovimiento();
    }

    void MoverCamara()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Acumulamos Y para aplicarlo en FixedUpdate
        yRotation += mouseX;

        // Cámara solo maneja eje vertical
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        camaraJugador.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void ManejarMovimiento()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(x) < 0.1f) x = 0f;
        if (Mathf.Abs(y) < 0.1f) y = 0f;

        corriendo = Input.GetKey(teclaSprint) && y > 0f;

        inputDir = transform.right * x + transform.forward * y;
        if (inputDir.magnitude > 1f)
            inputDir = inputDir.normalized;

        float animY = corriendo ? y * 2f : y * 0.5f;
        animator.SetFloat("SpeedY", animY, 0.1f, Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            return;
        }

        // ── Rotación horizontal via Rigidbody ──────────────────
        // Usamos MoveRotation con el valor acumulado — compatible con freezeRotation
        Quaternion targetRot = Quaternion.Euler(0f, yRotation, 0f);
        rb.MoveRotation(targetRot);

        // ── Movimiento ─────────────────────────────────────────
        float vel = corriendo ? velocidadSprint : speed;
        Vector3 newVel = inputDir * vel;
        rb.velocity = new Vector3(newVel.x, rb.velocity.y, newVel.z);
    }
}