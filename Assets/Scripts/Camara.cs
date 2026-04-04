using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camara : MonoBehaviour
{
    public float Sensibilidad = 100f;
    public Transform Player;

    float RotacionHorizontal = 0f;
    float RotacionVertical = 0f;
    // Start is called before the first frame update
    void Start()
    {
        //Bloquea el cursor en el centro de la pantalla y lo hace invisible
        Cursor.lockState = CursorLockMode.Locked;
        //Ocultar el cursor mientras se juega
        Cursor.visible = false;

    }

    // Update is called once per frame
    void Update()
    {
        //Nos da el valor del movimiento del mouse en el eje X y Y, lo multiplica por la sensibilidad y por el tiempo entre frames para que el movimiento sea suave e independiente de la velocidad de fotogramas
        float ValorX = Input.GetAxis("Mouse X") * Sensibilidad * Time.deltaTime;
        float ValorY = Input.GetAxis("Mouse Y") * Sensibilidad * Time.deltaTime;

        //guarda el valor de la rotacion horizontal y vertical, la rotacion horizontal se suma con el valor del mouse en el eje X, mientras que la rotacion vertical se resta con el valor del mouse en el eje Y para que al mover el mouse hacia arriba se mire hacia abajo y viceversa
        RotacionHorizontal += ValorX;
        RotacionVertical -= ValorY;

        //Limita la rotacion vertical para evitar que la camara gire completamente hacia arriba o hacia abajo, lo que podria causar problemas de vision
        RotacionVertical = Mathf.Clamp(RotacionVertical, -80f, 80f);
        //Aplica la rotacion vertical a la camara, utilizando Quaternion.Euler para convertir los grados de rotacion en una rotacion en 3D
        transform.localRotation = Quaternion.Euler(RotacionVertical, 0f, 0f);
        //Aplica la rotacion horizontal al jugador, utilizando el metodo Rotate para rotar el jugador alrededor del eje Y (Vector3.up) por el valor de la rotacion horizontal
        Player.Rotate(Vector3.up * ValorX);

    }
}
