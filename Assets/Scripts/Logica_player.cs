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
    private float yRotation = 0f;
    private float inputX = 0f;  // ← guardamos input crudo
    private float inputY = 0f;
    private Rigidbody rb;
    private Animator animator;
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
        // Solo guardamos el input, NO calculamos dirección aquí
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(inputX) < 0.1f) inputX = 0f;
        if (Mathf.Abs(inputY) < 0.1f) inputY = 0f;

        corriendo = Input.GetKey(teclaSprint) && inputY > 0f;

        // Animator se actualiza con input crudo
        float animY = corriendo ? inputY * 1f : inputY * 0.5f;
        float animX = corriendo ? inputX * 1f : inputX * 0.5f;

        animator.SetFloat("SpeedX", animX, 0.1f, Time.deltaTime);
        animator.SetFloat("SpeedY", animY, 0.1f, Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            return;
        }

        // 1. Primero aplicamos la rotación
        rb.MoveRotation(Quaternion.Euler(0f, yRotation, 0f));

        // 2. DESPUÉS calculamos inputDir con el transform ya rotado
        Vector3 inputDir = transform.right * inputX + transform.forward * inputY;
        if (inputDir.magnitude > 1f)
            inputDir = inputDir.normalized;

        // 3. Aplicamos velocidad
        float vel = corriendo ? velocidadSprint : speed;
        Vector3 newVel = inputDir * vel;
        rb.velocity = new Vector3(newVel.x, rb.velocity.y, newVel.z);
    }
}