using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotNoReaction : IBotMovement
{
    Transform scene;

    Quaternion objective;
    bool hasObjective = false;
    float rot = -0.1f;

    public override bool Click()
    {
        return true;
    }

    public override float MouseX()
    {
        
        if (hasObjective)
        {
            float angle = objective.eulerAngles.y - transform.rotation.eulerAngles.y;
            if (angle > 180f) angle -= 360f;

            //Debug.Log(angle);
            if (Mathf.Abs(angle) < 2f)
            {
                hasObjective = false;
                rot = -rot;
            }
            
            return Mathf.Clamp(angle, -100f, 100f) * CameraMovement.width / (CameraMovement.camAngleHor * CameraMovement.maxSpeed) * Time.deltaTime * 2f;
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
                if (t.isVisible)
                {
                    objective = Quaternion.LookRotation(t.transform.position - transform.position, Vector3.up);
                    hasObjective = true;
                    //Debug.Log(t.gameObject.name + " visible");
                } else
                {
                    //Debug.Log(t.gameObject.name + " NOT visible");
                }
            }
    }
}
