using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotNoReaction : IBotMovement
{
    Transform scene;

    Quaternion objective;
    bool hasObjective = false;
    float rot = -0.3f;

    float reactTimeAverage = 0.25f;
    float reactTimeNext = 0.25f;
    float timerReact = 0f;

    public override bool Click()
    {
        return true;
    }

    public override float MouseX()
    {
        // Imprecision en movimiento continuo
        if (rot<0)
            rot = Mathf.Clamp(rot + Random.Range(-0.01f, 0.01f), -0.35f, -0.25f);
        else
            rot = Mathf.Clamp(rot + Random.Range(-0.01f, 0.01f), 0.25f, 0.35f);

        if (/*timerReact>reactTimeNext*/ hasObjective)
        {
            float angle = objective.eulerAngles.y - transform.rotation.eulerAngles.y;
            if (angle > 180f) angle -= 360f;
            if (angle < -180f) angle += 360f;


            //Debug.Log(angle);
            if (Mathf.Abs(angle) < 1f)
            {
                hasObjective = false;
                rot = -rot;
                timerReact = 0f;
                reactTimeNext = reactTimeAverage + Random.Range(-reactTimeAverage / 5f, reactTimeAverage / 5f);
            }
            
            return (Mathf.Clamp(angle, -20f, 20f) + Random.Range(-2f, 2f)) * CameraMovement.width / (CameraMovement.camAngleHor * CameraMovement.maxSpeed) * Time.deltaTime * 20f;
        } else
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
        
        if (!hasObjective)
            foreach (Renderer t in scene.GetComponentsInChildren<Renderer>())
            {
                if (t.isVisible) //worldtoviewcamera
                {
                    objective = Quaternion.LookRotation(t.transform.position - transform.position, Vector3.up);
                    hasObjective = true; 
                    //Debug.Log(t.gameObject.name + " visible");
                } else
                {
                    //Debug.Log(t.gameObject.name + " NOT visible");
                }
            }
        else
        {
            timerReact += Time.deltaTime;
        }
    }
}
