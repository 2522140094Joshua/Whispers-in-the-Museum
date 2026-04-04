using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Botones que aparecen después")]
    public GameObject botonJugar;
    public GameObject botonNiveles;

    [Header("Paneles")]
    public GameObject menuOpciones;
    public GameObject menuNiveles;
    public GameObject menuPrincipal;

    private bool submenuActivo = false;

    void Start()
    {
        // Ocultar al iniciar
        botonJugar.SetActive(false);
        botonNiveles.SetActive(false);

        menuOpciones.SetActive(false);
        menuNiveles.SetActive(false);

    }

    // BOTON EMPEZAR
    public void Empezar()
    {
        submenuActivo = !submenuActivo;

        botonJugar.SetActive(submenuActivo);
        botonNiveles.SetActive(submenuActivo);
    }

    // BOTON JUGAR
    public void Jugar()
    {
        SceneManager.LoadScene("Escena 1"); // nombre de tu escena
    }

    public void JugarNivel1()
    {
        SceneManager.LoadScene("Escena 1");
    }
    public void JugarNivel2()
    {
        SceneManager.LoadScene("Escena 2");
    }
    public void JugarNivel3()
    {
        SceneManager.LoadScene("Escena 3");
    }

    // BOTON NIVELES
    public void AbrirNiveles()
    {
        menuNiveles.SetActive(true);
        menuPrincipal.SetActive(false);
    }

    // BOTON AJUSTES
    public void AbrirOpciones()
    {
        menuOpciones.SetActive(true);
        menuPrincipal.SetActive(false);
    }

    // BOTON VOLVER
    public void Volver()
    {
        menuOpciones.SetActive(false);
        menuNiveles.SetActive(false);
        menuPrincipal.SetActive(true);

        botonJugar.SetActive(false);
        botonNiveles.SetActive(false);

        submenuActivo = false;
    }

    // BOTON SALIR
    public void Salir()
    {
        Application.Quit();
        Debug.Log("Salir del juego");
    }
}
