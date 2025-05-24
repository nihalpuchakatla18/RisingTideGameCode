using UnityEngine;

public class PresurePlate : MonoBehaviour
{
    public GameObject platformPrefab;
    private GameObject spawnedPlatform;

    private int objectCount = 0;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Clone"))
        {
            objectCount++;

            if (spawnedPlatform == null)
            {
                spawnedPlatform = Instantiate(platformPrefab, new Vector2(11f, 1f), Quaternion.identity);
                Debug.Log("SKIBIDI SPAWNED");
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Clone"))
        {
            objectCount--;

            if (objectCount <= 0 && spawnedPlatform != null)
            {
                Destroy(spawnedPlatform);
                spawnedPlatform = null;
                Debug.Log("SKIBIDI DESTROYED");
            }
        }
    }
}
