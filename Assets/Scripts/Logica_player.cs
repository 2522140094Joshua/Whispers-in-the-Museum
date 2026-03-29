using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Logica_player : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 5f;
    public float rotationSpeed = 200f;
    public float jumpForce = 5f;

    [Header("Sprint")]
    public float velocidadSprint = 10f;  // velocidad al correr
    public KeyCode teclaSprint = KeyCode.LeftShift; // tecla para correr
    private bool corriendo = false;

    [Header("Audio")]
    public AudioClip sonidoPuntos;
    public AudioClip sonidoNegativo;

    private Rigidbody rb;
    private Animator animator;
    public float x, y;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");

        // Sprint: true si mantiene Shift y se esta moviendo
        corriendo = Input.GetKey(teclaSprint) && y != 0;

        float velocidadActual = corriendo ? velocidadSprint : speed;

        // Rotacion y movimiento
        //transform.Rotate(0, x * rotationSpeed * Time.deltaTime, 0);
        transform.Translate(0, 0, y * velocidadActual * Time.deltaTime);

        // Rigidbody movimiento
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 movimiento = new Vector3(horizontal, 0f, vertical) * velocidadActual * Time.deltaTime;
        rb.MovePosition(rb.position + movimiento);

        // Animaciones
        animator.SetFloat("SpeedX", x);
        animator.SetFloat("SpeedY", corriendo ? y * 2f : y); // anima mas rapido al correr
    }
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}