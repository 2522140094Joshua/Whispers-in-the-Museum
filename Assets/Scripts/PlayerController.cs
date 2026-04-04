using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class playerController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform camara;
    private CharacterController controlador;
    private Animator animator;


    [Header("Movimiento")]
    [SerializeField] private float velocidadMovimiento = 5f;
    [SerializeField] private float velocidadCorrer = 8f;

    [SerializeField] private float staminaMax = 100f;
    [SerializeField] private float consumoPorSegundo = 20f;
    [SerializeField] private float recuperacionPorSegundo = 15f;
    [SerializeField] private float delayRecuperacion = 0.5f;
    [SerializeField] private Image barraEstaminaFill;
    private float staminaActual;
    private float tiempoSinCorrer = 0f;


    [Header("Gravedad")]
    [SerializeField] private float gravedad = -9.81f;
    private Vector3 velocidadVertical;

    void Start()
    {

    }
    private void Awake()
    {
        animator = GetComponent<Animator>();
        controlador = GetComponent<CharacterController>();

        if (camara == null && Camera.main != null)
        {
            camara = Camera.main.transform;
        }

        staminaActual = staminaMax;
    }

    void Update()
    {
        MoverJugadorEnPlano();
        AplicarGravedad();
    }

    private void MoverJugadorEnPlano()
    {
        //Mecanicas de Movimiento
        float ValorHorizontal = Input.GetAxisRaw("Horizontal");
        float ValorVertical = Input.GetAxisRaw("Vertical");

        animator.SetFloat("SpeedX", ValorHorizontal);
        animator.SetFloat("SpeedY", ValorVertical);

        Vector3 adelanteCamara = camara.forward;
        Vector3 derechaCamara = camara.right;

        adelanteCamara.y = 0;
        derechaCamara.y = 0;
        adelanteCamara.Normalize();
        derechaCamara.Normalize();

        Vector3 direccionMovimiento = adelanteCamara * ValorVertical + derechaCamara * ValorHorizontal;

        if (direccionMovimiento.sqrMagnitude > 0.0001f)
        {
            direccionMovimiento.Normalize();
        }
        // mecanicas de Correr
        bool seEstaMoviendo = direccionMovimiento.sqrMagnitude > 0.0001f;
        bool botonCorrer = Input.GetKey(KeyCode.LeftShift);
        bool puedoCorrer = staminaActual > 0.01f;
        bool corriendo = botonCorrer && seEstaMoviendo && puedoCorrer;

        if (corriendo)
        {
            staminaActual -= consumoPorSegundo * Time.deltaTime;
            tiempoSinCorrer = 0f;
        }
        else
        {
            tiempoSinCorrer += Time.deltaTime;
            if (tiempoSinCorrer >= delayRecuperacion)
            {
                staminaActual += recuperacionPorSegundo * Time.deltaTime;
            }
        }

        staminaActual = Mathf.Clamp(staminaActual, 0f, staminaMax);

        if (barraEstaminaFill != null)
        {
            barraEstaminaFill.fillAmount = staminaActual / staminaMax;
        }

        float velocidadActual = corriendo ? velocidadCorrer : velocidadMovimiento;

        Vector3 desplazamientoXZ = direccionMovimiento * (velocidadActual * Time.deltaTime);
        controlador.Move(desplazamientoXZ);

    }

    private void AplicarGravedad()
    {
        if (controlador.isGrounded && velocidadVertical.y < 0)
        {
            velocidadVertical.y = -2f;
        }

        velocidadVertical.y += gravedad * Time.deltaTime;
        controlador.Move(velocidadVertical * Time.deltaTime);
    }
}
