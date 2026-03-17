using UnityEngine;

public class Pickup : MonoBehaviour
{
    public float distance = 10f;      // Aumentamos a 10
    public GameObject heldObject;
    public Transform holdPoint;
    public Camera playerCamera;       // Arrastra TU cámara aquí manualmente

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E presionado");

            // Usa playerCamera si la asignaste, si no usa Camera.main
            Camera cam = playerCamera != null ? playerCamera : Camera.main;

            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit hit;

            Debug.DrawRay(cam.transform.position, cam.transform.forward * distance, Color.red, 2f);

            if (Physics.Raycast(ray, out hit, distance))
            {
                Debug.Log("GOLPEÓ: " + hit.collider.name + " | Tag: " + hit.collider.tag);

                if (heldObject == null)
                    TryPickup(hit);
                else
                    PlaceObject(hit);
            }
            else
            {
                Debug.LogWarning("Rayo no golpeó nada");
            }
        }

        if (heldObject != null && holdPoint != null)
        {
            heldObject.transform.position = holdPoint.position;
            heldObject.transform.rotation = holdPoint.rotation;
        }
    }

    void TryPickup(RaycastHit hit)
    {
        if (hit.collider.CompareTag("Pickup"))
        {
            heldObject = hit.collider.gameObject;
            heldObject.transform.SetParent(holdPoint);

            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;

            Debug.Log("Objeto agarrado: " + heldObject.name);
        }
    }

    void PlaceObject(RaycastHit hit)
    {
        if (hit.collider.CompareTag("Wall"))
        {
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false;

            heldObject.transform.SetParent(null);
            heldObject.transform.position = hit.point;
            heldObject.transform.rotation = Quaternion.LookRotation(hit.normal);
            heldObject = null;

            Debug.Log("Objeto colocado en la pared");
        }
    }
}