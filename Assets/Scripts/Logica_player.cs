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
    public bool puedeMoverse = true;

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
        {
            if (puedeMoverse) // <--- Solo se mueve si esto es verdadero
            {
                x = Input.GetAxis("Horizontal");
                y = Input.GetAxis("Vertical");

                corriendo = Input.GetKey(teclaSprint) && y != 0;
                float velocidadActual = corriendo ? velocidadSprint : speed;

                // Movimiento Rigidbody
                Vector3 movimiento = new Vector3(x, 0f, y) * velocidadActual * Time.deltaTime;
                rb.MovePosition(rb.position + movimiento);

                // Animaciones
                animator.SetFloat("SpeedX", x);
                animator.SetFloat("SpeedY", corriendo ? y * 2f : y);
            }
            else // Si no puede moverse, reseteamos las animaciones a 0
            {
                animator.SetFloat("SpeedX", 0);
                animator.SetFloat("SpeedY", 0);
            }
        }
    }
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}