using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Configuracion de la puerta")]
    public float anguloAbierto = 90f;
    public float velocidad = 3f;

    [Header("Eje de rotacion")]
    public bool rotarEnY = true;
    public bool rotarEnX = false;
    public bool invertirDireccion = false;

    [Header("Rango de interaccion")]
    public float rangoInteraccion = 2.5f; // distancia maxima para abrir
    public Transform jugador;             // arrastra el Player aqui en el Inspector

    private Quaternion rotacionCerrada;
    private Quaternion rotacionAbierta;
    private bool estaAbierta = false;

    void Start()
    {
        rotacionCerrada = transform.rotation;

        Vector3 eje = rotarEnY ? Vector3.up : (rotarEnX ? Vector3.right : Vector3.forward);
        float direccion = invertirDireccion ? -1f : 1f;
        rotacionAbierta = rotacionCerrada * Quaternion.Euler(eje * anguloAbierto * direccion);

        if (jugador == null)
        {
            GameObject go = GameObject.FindWithTag("Player");
            if (go != null) jugador = go.transform;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && JugadorEstaCerca())
        {
            estaAbierta = !estaAbierta;
        }

        Quaternion objetivo = estaAbierta ? rotacionAbierta : rotacionCerrada;
        transform.rotation = Quaternion.Lerp(transform.rotation, objetivo, Time.deltaTime * velocidad);
    }

    bool JugadorEstaCerca()
    {
        if (jugador == null) return false;
        float distancia = Vector3.Distance(transform.position, jugador.position);
        return distancia <= rangoInteraccion;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoInteraccion);
    }
}