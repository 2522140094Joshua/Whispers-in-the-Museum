using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform spawnPoint; // Arrastra aquí el SpawnPoint del siguiente nivel

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                other.transform.position = spawnPoint.position;
                cc.enabled = true;
            }
            else
            {
                other.transform.position = spawnPoint.position;
            }
        }
    }
}