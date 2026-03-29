using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    public Transform door;
    public Vector3 openRotation = new Vector3(0, 90, 0);
    public float speed = 2f;

    private bool isOpening = false;
    private Quaternion closedRot;
    private Quaternion openRot;

    void Start()
    {
        closedRot = door.rotation;
        openRot = Quaternion.Euler(openRotation);
    }

    void Update()
    {
        if (isOpening)
            door.rotation = Quaternion.Lerp(door.rotation, openRot, Time.deltaTime * speed);
    }

    public void OpenDoor()
    {
        isOpening = true;
    }
}