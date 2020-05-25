using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotDelayUniform : IBotMovement
{
    public Transform scene;

    GameObject gameObjective;
    Quaternion objective;
    bool hasObjective = false;

    public bool debug = false;

    float rot = -0.3f;

    float reactTime = 0.2f;

    float timerReact = 0f;

    float closest = 0.5f;

    public override bool Click()
    {
        return true;
    }

    public override float MouseX()
    {
        if (timerReact > reactTime)
        {
            float angle = objective.eulerAngles.y - transform.rotation.eulerAngles.y;
            if (angle > 180f) angle -= 360f;
            if (angle < -180f) angle += 360f;


            //Debug.Log(angle);
            if (Mathf.Abs(angle) < 1f)
            {
                hasObjective = false;
                closest = 2.0f;
                //rot = -rot;
                timerReact = 0f;
            }

            return Mathf.Clamp(angle, -15f, 15f) * CameraMovement.width / (CameraMovement.camAngleHor * CameraMovement.maxSpeed) * Time.deltaTime * 40f;
        }
        else
        {
            return rot;
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
            closest = 2.0f;
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
