using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject plane;

    public int maxPlanes = 6;
    public static int currentPlanes = 0;

    public float timeBetweenSpawn = 2.0f;
    float timeElapsed = 0f;

    public float minDist = 30f;
    public float maxDist = 400f;

    // Start is called before the first frame update
    void Start()
    {
        SpawnPlane();
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed > timeBetweenSpawn && currentPlanes<maxPlanes)
        {
            SpawnPlane();
        }
    }

    void SpawnPlane()
    {
        float dist = Random.Range(minDist, maxDist);
        float angle = Random.Range(0f, 360f);

        GameObject newPlane = Instantiate(plane, new Vector3(0f, 0f, dist), transform.rotation, transform);
        newPlane.transform.RotateAround(transform.position, Vector3.up, angle);

        timeElapsed = 0f;
        currentPlanes++;
    }
}
