using UnityEngine;

public class ResettableObject : MonoBehaviour
{
    private Vector3 startPos;
    private Quaternion startRot;
    private Collider col;
    private Rigidbody rb;

    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;

        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }

    public void ResetObject()
    {
        transform.position = startPos;
        transform.rotation = startRot;

        // Reactivar objeto
        gameObject.SetActive(true);

        // 🔥 Reactivar collider
        if (col != null)
            col.enabled = true;

        // 🔥 Reactivar física
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
        }
    }
}