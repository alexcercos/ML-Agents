using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCases : MonoBehaviour
{
    public GameObject plane;

    public Transform cameraYAxis;

    public float timeBetweenSpawn = 2.0f;
    float timeElapsed = 0f;

    public float minDist = 30f;
    public float maxDist = 400f;

    public float heightDiff = 20f;

    float cameraFOV = 60f; //FOV horizontal, comprobado a mano (en realidad es 63.7 pero se desprecia la diferencia)
    

    [Range(0.0f, 1.0f)]
    public float commonCase;
    [Range(0.0f, 1.0f)]
    public float inLeftCase;
    [Range(0.0f, 1.0f)]
    public float inRightCase;
    [Range(0.0f, 1.0f)]
    public float consecutiveCase;


    private void OnValidate()
    {
        
        float total = commonCase + inLeftCase + inRightCase + consecutiveCase;
        
        if (total == 0f)
        {
            commonCase = inLeftCase = inRightCase = consecutiveCase = 1 / 4f;
        }
        else
        {
            commonCase /= total;
            inLeftCase /= total;
            inRightCase /= total;
            consecutiveCase /= total;
        }
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed > timeBetweenSpawn)
        {
            float rand = Random.Range(0f, 1f);

            //Debug.Log(rand);
            if (rand < commonCase)
                SpawnPlane(ESpawnCase.OUTLEFT);
            rand -= commonCase;

            if (rand>=0f  && rand < inLeftCase)
                SpawnPlane(ESpawnCase.INLEFT);
            
            rand -= inLeftCase;

            if (rand >= 0f && rand < inRightCase)
                SpawnPlane(ESpawnCase.INRIGHT);

            rand -= inLeftCase;

            if (rand >= 0f && rand < consecutiveCase)
                SpawnPlane(ESpawnCase.CONSECUTIVE);

            //rand -= consecutiveCase;
        }
    }

    void SpawnPlane(ESpawnCase typeSpawn)
    {
        timeElapsed = 0f;

        //Debug.Log(typeSpawn);

        float dist = Random.Range(minDist, maxDist);
        float angle = 0f;

        float left = (cameraYAxis.rotation.eulerAngles.y - cameraFOV / 2f);
        float right = (cameraYAxis.rotation.eulerAngles.y + cameraFOV / 2f);

        if (typeSpawn == ESpawnCase.INLEFT)
        {
            angle = Random.Range(left, cameraYAxis.rotation.eulerAngles.y); //por dentro
        }
        else if (typeSpawn == ESpawnCase.INRIGHT)
        {
            angle = Random.Range(cameraYAxis.rotation.eulerAngles.y, right); //por dentro
        }
        else
        {
            //if (typeSpawn == ESpawnCase.CONSECUTIVE)
            angle = Random.Range(left - 10f, left - 30f); //por fuera

            GameObject newPlaneExtra = Instantiate(plane, new Vector3(0f, Random.Range(-heightDiff, heightDiff), dist), transform.rotation, transform);
            newPlaneExtra.transform.RotateAround(transform.position, Vector3.up, angle);

            angle = angle - Random.Range(0f, cameraFOV / 2f); //Vuelve a spawnear consecutivo
            dist = Random.Range(minDist, maxDist);

            if (typeSpawn == ESpawnCase.OUTLEFT)
                return;
        }
        


        GameObject newPlane = Instantiate(plane, new Vector3(0f, Random.Range(-heightDiff, heightDiff), dist), transform.rotation, transform);
        newPlane.transform.RotateAround(transform.position, Vector3.up, angle);

    }
}

public enum ESpawnCase
{
    OUTLEFT, INLEFT, INRIGHT, CONSECUTIVE
}