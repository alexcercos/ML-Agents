using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotDelayUniform : IBotMovement
{
    Transform scene;

    GameObject gameObjective;
    Quaternion objective;
    bool hasObjective = false;

    public bool debug = false;

    float rot = -0.3f;

    public float reactTime = 0.2f;
    public float noise = 0.0f;
    public float tempNoise = 0.0f; //se suma

    float nextReact = 0.0f;

    float timerReact = 0f;

    float closest = 0.5f;

    public override bool Click()
    {
        return true;
    }

    public override float MouseX()
    {
        if (timerReact > nextReact)
        {
            float angle = objective.eulerAngles.y - transform.rotation.eulerAngles.y;
            if (angle > 180f) angle -= 360f;
            if (angle < -180f) angle += 360f;


            //Debug.Log(angle);
            if (Mathf.Abs(angle) < 1f)
            {
                hasObjective = false;
                closest = 0.5f;
                //rot = -rot;
                timerReact = 0f;
                nextReact = reactTime + Random.Range(0f, tempNoise);
            }

            return Mathf.Clamp(angle, -15f, 15f) * CameraMovement.width / (CameraMovement.camAngleHor * CameraMovement.maxSpeed) * Time.deltaTime * 40f + Random.Range(-noise/2f, noise/2f);
        }
        else
        {
            return rot + Random.Range(-noise * 2f, noise * 2f);
        }
    }

    public override float MouseY()
    {
        return 0f;
    }


    void Start()
    {
        objective = transform.rotation;
        scene = GameObject.Find("Scene").transform;
        nextReact = reactTime + Random.Range(0f, tempNoise);
    }


    void Update()
    {
        if (debug)
        {
            Debug.Log(gameObjective);
            Debug.Log(timerReact);
            Debug.Log(hasObjective);
        }
        
        if (hasObjective && gameObjective)
            timerReact += Time.deltaTime;
        else
        {
            hasObjective = false;
            closest = 0.5f;
            timerReact = 0f;
            nextReact = reactTime + Random.Range(0f, tempNoise);
            foreach (Renderer t in scene.GetComponentsInChildren<Renderer>())
            {
                Vector3 screenPos = Camera.main.WorldToViewportPoint(t.transform.position);
                if (screenPos.z > 0f)
                {
                    float dist = Mathf.Abs(screenPos.x - 0.5f);
                    if (dist < closest)
                    {
                        float thisReaction = GetReactionTime();

                        closest = dist;
                        objective = Quaternion.LookRotation(t.transform.position - transform.position, Vector3.up);
                        hasObjective = true;

                        gameObjective = t.gameObject;
                    }
                }
            }
        }
            
    }

    float GetReactionTime()
    {
        return reactTime;
    }
}
