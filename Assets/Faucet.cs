using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Faucet : MonoBehaviour
{
    public GameObject item;
    public float rate = 1.0f;
    public float spawnSize = 5.0f;

    private float countdown;

    // Start is called before the first frame update
    void Start()
    {
        countdown = 1.0f/rate;
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown < 0) {
            countdown = 1.0f/rate;

            GameObject go = Instantiate(item, transform.position + 0.5f * new Vector3(Random.Range(-spawnSize, spawnSize), 0, Random.Range(-spawnSize, spawnSize)), Quaternion.identity);
        }
    }

    IEnumerable ScheduleDestruction(GameObject go) {
        yield return new WaitForSeconds(5.0f);
        Destroy(go);
    }
}
