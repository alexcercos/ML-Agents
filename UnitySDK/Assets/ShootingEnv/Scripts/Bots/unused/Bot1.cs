using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot1 : IBotMovement
{
    Transform scene;

    float avgReactionTime = 0.3f;
    float desviation = 0.15f;

    float currentReaction = 0.2f;
    bool react = false;
    bool chooseObj = false;

    Quaternion objective;
    Quaternion randObjective;
    float angle = Mathf.Infinity;

    float timer = 0f;
    float rTimer = 0f;
    float maxR = 0.5f;

    private void Start()
    {
        objective = transform.rotation;
        scene = GameObject.Find("Scene").transform;

        setRandomObjective();
    }

    public override bool Click()
    {
        return false;
    }

    public override float MouseX()
    {
        if (chooseObj)
        {
            float angle = (objective.eulerAngles.x - transform.rotation.eulerAngles.x);
            if (angle > 180f) angle = angle - 360f;

            return angle / Quaternion.Angle(objective, transform.rotation);
        }
        else
        {
            float angle = (randObjective.eulerAngles.x - transform.rotation.eulerAngles.x);
            if (angle > 180f) angle = angle - 360f;

            return (randObjective.eulerAngles.x - transform.rotation.eulerAngles.x) / Quaternion.Angle(randObjective, transform.rotation);
        }
    }

    public override float MouseY()
    {
        if (chooseObj)
        {
            float angle = (objective.eulerAngles.y - transform.rotation.eulerAngles.y);
            if (angle > 180f) angle = angle - 360f;

            return angle / Quaternion.Angle(objective, transform.rotation);
        }
        else
        {
            float angle = (randObjective.eulerAngles.y - transform.rotation.eulerAngles.y);
            if (angle > 180f) angle = angle - 360f;

            return (randObjective.eulerAngles.y - transform.rotation.eulerAngles.y) / Quaternion.Angle(randObjective, transform.rotation);
        }
    }

    private void Update()
    {
        rTimer += Time.deltaTime;
        if (rTimer > maxR)
        {
            setRandomObjective();
            rTimer = 0f;
            maxR = 0.5f + Random.Range(-0.1f, 0.1f);
        }

        if (!react)
        {
            chooseObj = false;
            foreach (Renderer t in scene.GetComponentsInChildren<Renderer>())
            {
                if (t.isVisible)
                {
                    Quaternion thisRot = Quaternion.LookRotation(t.transform.position - transform.position);
                    float thisAngle = Quaternion.Angle(thisRot, transform.rotation);
                    if (thisAngle < angle)
                    {
                        objective = thisRot;
                        angle = thisAngle;
                        react = true;
                        currentReaction = avgReactionTime + Random.Range(-desviation, desviation);
                        Debug.Log("hhh");
                    }
                }
            }
        }
        else
        {
            timer += Time.deltaTime;
            if (timer > currentReaction)
            {
                chooseObj = true;
                timer = 0f;
                react = false;
            }
            else
            {
                chooseObj = false;
            }
        }

    }


    void setRandomObjective()
    {
        randObjective = Quaternion.Euler(transform.rotation.eulerAngles.x + Random.Range(-100f, 100f), Random.Range(-20f, 20f), 0f);
    }
}
