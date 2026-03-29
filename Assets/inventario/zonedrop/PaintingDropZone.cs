using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PaintingDropZone : MonoBehaviour
{
    public float spacingBetweenPaintings = 1.2f;
    public float displayDuration = 2f;
    public Vector3 paintingRotation = new Vector3(0, 180, 0);
    public Transform rowStartPoint;
    public DoorOpener doorOpener; // arrastra el GameObject de la puerta aqui

    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        List<GameObject> items = InventoryManager.Instance.GetItems();
        if (items.Count == 0) return;

        triggered = true;
        StartCoroutine(PlaceAndDeliver(new List<GameObject>(items)));
    }

    IEnumerator PlaceAndDeliver(List<GameObject> toDeliver)
    {

            Vector3 origin = rowStartPoint != null ? rowStartPoint.position : transform.position;

            for (int i = 0; i < toDeliver.Count; i++)
            {
                GameObject painting = toDeliver[i];
                Vector3 pos = origin + Vector3.right * (i * spacingBetweenPaintings);

                painting.SetActive(true);
                painting.transform.position = pos;
                painting.transform.rotation = Quaternion.Euler(paintingRotation);

                Rigidbody rb = painting.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Corregido: Rigidbody no tiene 'linearVelocity', debe ser 'velocity'
                    rb.velocity = Vector3.zero;
                    rb.isKinematic = true;
                }

                Collider col = painting.GetComponent<Collider>();
                if (col != null) col.enabled = false;

                yield return new WaitForSeconds(0.3f);
            }

            yield return new WaitForSeconds(displayDuration);
            InventoryManager.Instance.DeliverAllItems();
            // Abrir puerta despues de entregar
            if (doorOpener != null)
            doorOpener.OpenDoor();
    }
}