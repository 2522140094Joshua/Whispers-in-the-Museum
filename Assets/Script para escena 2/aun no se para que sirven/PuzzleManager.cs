using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    [Header("Cajas")]
    public PuzzleBox[] cajas;

    [Header("Al completar todo")]
    public DoorOpener doorOpener;

    private int cajasResueltas = 0;

    void Awake()
    {
        Instance = this;
    }

    public void MostrarPuzzle(int id, PuzzleBox caja)
    {
        caja.Resolver();
    }

    public void CheckCompletado()
    {
        cajasResueltas++;
        Debug.Log($"Cajas resueltas: {cajasResueltas}/{cajas.Length}");

        if (cajasResueltas >= cajas.Length)
        {
            if (doorOpener != null) doorOpener.OpenDoor();
        }
    }
}