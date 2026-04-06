using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    public Transform door;
    public Vector3 openRotation = new Vector3(0, -90, 0);
    public Vector3 closeRotation = new Vector3(0, 0, 0);
    public float speed = 2f;

    private bool isOpening = false;
    private bool isClosing = false;
    private Quaternion closedRot;
    private Quaternion openRot;

    void Start()
    {
        closedRot = Quaternion.Euler(closeRotation);
        openRot = Quaternion.Euler(openRotation);
    }

    void Update()
    {
        if (isOpening)
        {
            door.rotation = Quaternion.Lerp(door.rotation, openRot, Time.deltaTime * speed);

            // Snap final cuando está muy cerca
            if (Quaternion.Angle(door.rotation, openRot) < 0.1f)
            {
                door.rotation = openRot;
                isOpening = false;
            }
        }

        if (isClosing)
        {
            door.rotation = Quaternion.Lerp(door.rotation, closedRot, Time.deltaTime * speed);

            // Snap final cuando está muy cerca
            if (Quaternion.Angle(door.rotation, closedRot) < 0.1f)
            {
                door.rotation = closedRot;
                isClosing = false;
            }
        }
    }

    public void OpenDoor()
    {
        isClosing = false; // cancela cierre si estaba cerrando
        isOpening = true;
    }

    public void CloseDoor()
    {
        isOpening = false; // cancela apertura si estaba abriendo
        isClosing = true;
    }
}