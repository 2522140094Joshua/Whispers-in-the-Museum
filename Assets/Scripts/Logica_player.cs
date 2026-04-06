using UnityEngine;

public class Logica_player : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 5f;

    [Header("Sprint")]
    public float velocidadSprint = 10f;
    public KeyCode teclaSprint = KeyCode.LeftShift;

    [Header("Cámara Primera Persona")]
    public Transform camaraJugador;
    public float mouseSensitivity = 100f;

    private float xRotation = 0f;
    private float yRotation = 0f;
    private float inputX = 0f;
    private float inputY = 0f;
    private bool corriendo = false;

    private Rigidbody rb;
    private Animator animator;

    // ── Singleton para evitar duplicados con DontDestroyOnLoad ────────────────
    private static Logica_player instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.freezeRotation = true;
        rb.useGravity = true;

        yRotation = transform.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            animator.SetFloat("SpeedX", 0f, 0.1f, Time.deltaTime);
            animator.SetFloat("SpeedY", 0f, 0.1f, Time.deltaTime);
            return;
        }

        MoverCamara();
        ManejarInput();
    }

    void MoverCamara()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        camaraJugador.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void ManejarInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(inputX) < 0.1f) inputX = 0f;
        if (Mathf.Abs(inputY) < 0.1f) inputY = 0f;

        corriendo = Input.GetKey(teclaSprint) && inputY > 0f;

        float animY = corriendo ? inputY * 1f : inputY * 0.5f;
        float animX = corriendo ? inputX * 1f : inputX * 0.5f;
        animator.SetFloat("SpeedX", animX, 0.1f, Time.deltaTime);
        animator.SetFloat("SpeedY", animY, 0.1f, Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            return;
        }

        // 1. Rotar cuerpo
        rb.MoveRotation(Quaternion.Euler(0f, yRotation, 0f));

        // 2. Dirección de movimiento
        Vector3 inputDir = transform.right * inputX + transform.forward * inputY;
        if (inputDir.magnitude > 1f)
            inputDir = inputDir.normalized;

        // 3. Aplicar velocidad horizontal, conservar Y para la gravedad
        float vel = corriendo ? velocidadSprint : speed;
        Vector3 newVel = inputDir * vel;
        rb.velocity = new Vector3(newVel.x, rb.velocity.y, newVel.z);
    }
}