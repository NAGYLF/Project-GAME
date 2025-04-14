using System.Collections;
using UnityEngine;

public class target_dummy : MonoBehaviour
{
    private static bool hasSpawned = false;

    void Start()
    {
        if (!hasSpawned)
        {
            hasSpawned = true;

            for (int i = 0; i < 5; i++)
            {
                SpawnNewDummy();
            }

            Destroy(gameObject);
        }
    }

    public static void SpawnNewDummy()
    {
        float randomX = Random.Range(300f, 600f);
        float randomY = Random.Range(-200f, 100f);
        Vector3 spawnPosition = new Vector3(randomX, randomY, 0f);

        GameObject newDummy = Instantiate(Resources.Load<GameObject>("MapElements/Env/celtabla"));
        newDummy.transform.position = spawnPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Bullet>() != null)
        {
            StartCoroutine(RespawnAfterDelay(2f));
            Destroy(gameObject);
        }
    }

    private IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnNewDummy();
    }
}
