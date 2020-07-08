using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCases : MonoBehaviour
{
    public GameObject plane;

    public Transform cameraYAxis;

    public float timeBetweenSpawn = 2.0f;
    float timeElapsed = 0f;

    public int maxPlanes = 3;

    [HideInInspector] public float minDist = 45f;
    [HideInInspector] public float maxDist = 200f;

    public float heightDiff = 0f;

    float cameraFOV = 60f; //FOV horizontal, comprobado a mano (en realidad es 63.7 pero se desprecia la diferencia)

    public bool invertCommons = false;
    

    [Range(0.0f, 1.0f)]
    public float commonCase;
    [Range(0.0f, 1.0f)]
    public float inLeftCase;
    [Range(0.0f, 1.0f)]
    public float inRightCase;
    [Range(0.0f, 1.0f)]
    public float consecutiveCase;

    float lastCommon, lastLeft, lastRight, lastConsecutive;

    private void OnValidate()
    {
        if (lastCommon != commonCase)
        {
            RecalculateValues(ref commonCase, ref inLeftCase, ref inRightCase, ref consecutiveCase);
        }
        else if (lastLeft != inLeftCase)
        {
            RecalculateValues(ref inLeftCase, ref commonCase, ref inRightCase, ref consecutiveCase);
        }
        else if (lastRight != inRightCase)
        {
            RecalculateValues(ref inRightCase, ref commonCase, ref inLeftCase, ref consecutiveCase);
        }
        else if (lastConsecutive != consecutiveCase)
        {
            RecalculateValues(ref consecutiveCase, ref inRightCase, ref commonCase, ref inLeftCase);
        }

        lastCommon = commonCase;
        lastLeft = inLeftCase;
        lastRight = inRightCase;
        lastConsecutive = consecutiveCase;

        /*
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
        }*/
    }

    private void RecalculateValues(ref float fixedValue, ref float v1, ref float v2, ref float v3)
    {
        float total = v1 + v2 + v3;

        if (total == 0f && fixedValue == 0)
        {
            v1 = 1 / 3f;
            v2 = 1 / 3f;
            v3 = 1 / 3f;
        }
        else if (fixedValue == 1f)
        {
            v1 = v2 = v3 = 0f;
        }
        else
        {
            float rest = 1f - fixedValue;
            if (total == 0f)
            {
                v1 = 1f / 3f * rest;
                v2 = 1f / 3f * rest;
                v3 = 1f / 3f * rest;
            }
            else
            {
                v1 = v1 / total * rest;
                v2 = v2 / total * rest;
                v3 = v3 / total * rest;
            }
           
        }
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed > timeBetweenSpawn && transform.childCount < maxPlanes)
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
        float currentAngle = cameraYAxis.GetChild(0).rotation.eulerAngles.x;
        if ((currentAngle > 20f) && (360f - currentAngle > 20f))
        {
            //Debug.Log(currentAngle);
            return; // No spawnea si esta mirando arriba
        }

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

            if (invertCommons)
                angle = Random.Range(right + 10f, right + 30f); //por fuera
            else
                angle = Random.Range(left - 30f, left - 10f);

            GameObject newPlaneExtra = Instantiate(plane, new Vector3(0f, Random.Range(-heightDiff, heightDiff), dist), transform.rotation, transform);
            newPlaneExtra.transform.RotateAround(transform.position, Vector3.up, angle);

            if (invertCommons)
                angle = angle + Random.Range(0f, cameraFOV / 2f); //Vuelve a spawnear consecutivo
            else
                angle = angle - Random.Range(0f, cameraFOV / 2f);

            dist = Random.Range(minDist, maxDist);

            if (typeSpawn == ESpawnCase.OUTLEFT)
                return;
        }
        


        GameObject newPlane = Instantiate(plane, new Vector3(0f, Random.Range(-heightDiff, heightDiff), dist), transform.rotation, transform);
        newPlane.transform.RotateAround(transform.position, Vector3.up, angle);

    }

    public void ResetSpawner(float height, bool invert, Transform newCamera)
    {
        timeElapsed = 0f;

        heightDiff = height;

        invertCommons = invert;

        cameraYAxis = newCamera;
    }
}

public enum ESpawnCase
{
    OUTLEFT, INLEFT, INRIGHT, CONSECUTIVE
}