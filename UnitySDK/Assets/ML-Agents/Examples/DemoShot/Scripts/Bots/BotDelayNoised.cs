using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotDelayNoised : IBotMovement
{
    Transform scene;

    Quaternion objective;
    bool hasObjective = false;
    float rot = -0.3f;

    float reactTimeAverage = 0.2f;
    float reactTimeNext = 0.2f;
    float reactTimeDesv = 0.05f;

    float timerReact = 0f;

    public float noise = 0.3f;

    float closest = 0.5f;

    public override bool Click()
    {
        return true;
    }

    public override float MouseX()
    {
        // Imprecision en movimiento continuo
        if (rot < 0)
            rot = Mathf.Clamp(rot + Random.Range(-0.02f, 0.02f), -0.35f, -0.25f);
        else
            rot = Mathf.Clamp(rot + Random.Range(-0.02f, 0.02f), 0.25f, 0.35f);

        if (timerReact > reactTimeNext)
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
                reactTimeNext = reactTimeAverage + Random.Range(-reactTimeAverage / 5f, reactTimeAverage / 5f);
            }

            return Random.Range(-noise/2f, noise/2f) + Mathf.Clamp(angle, -15f, 15f) * CameraMovement.width / (CameraMovement.camAngleHor * CameraMovement.maxSpeed) * Time.deltaTime * 40f;
        }
        else
        {
            return rot + Random.Range(-noise, noise);
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
    }


    void Update()
    {
        if (hasObjective)
            timerReact += Time.deltaTime;

        foreach (Renderer t in scene.GetComponentsInChildren<Renderer>())
        {
            Vector3 screenPos = Camera.main.WorldToViewportPoint(t.transform.position);
            if (screenPos.z > 0f)
            {
                float dist = Mathf.Abs(screenPos.x - 0.5f);
                if (dist < closest)
                {
                    float thisReaction = GetReactionTime();

                    if (thisReaction < reactTimeNext - timerReact)
                    {
                        closest = dist;
                        objective = Quaternion.LookRotation(t.transform.position - transform.position, Vector3.up);
                        hasObjective = true;
                    }
                }
            }
        }
    }

    float GetReactionTime()
    {
        return reactTimeAverage + Random.Range(-reactTimeDesv, reactTimeDesv);
    }
}