using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class FinalStriker : Agent
{
    MatchController mc;

    const float PORT_X = 15f;
    const float PORT_Z = 4.1f;

    public float Speed = 8f;
    //public float RotationSpeed = 10f;
    GameObject pelota;
    Rigidbody rbPelota;
    Vector3 porteriaIzq;
    Vector3 porteriaDer;
    Vector3 porteriaRivalIzq;
    Vector3 porteriaRivalDer;

    private LayerMask layers;
    float perceptionDist = 30f;

    float kickPower = 0f;

    //en teoria los rayos no golpean backfaces

    public bool team; //true = X negativa

    public override void InitializeAgent()
    {
        layers = LayerMask.GetMask("Default", "goalie", "striker");
        pelota = transform.parent.Find("Soccer Ball").gameObject;
        rbPelota = pelota.GetComponent<Rigidbody>();

        mc = transform.parent.Find("SoccerFieldTwos").GetComponent<MatchController>();

        //porterias
        if (team) //azul, izquierda
        {
            porteriaDer = new Vector3(transform.parent.position.x - PORT_X, transform.parent.position.y, transform.parent.position.z - PORT_Z);
            porteriaIzq = new Vector3(transform.parent.position.x - PORT_X, transform.parent.position.y, transform.parent.position.z + PORT_Z);
            porteriaRivalDer = new Vector3(transform.parent.position.x + PORT_X, transform.parent.position.y, transform.parent.position.z - PORT_Z);
            porteriaRivalIzq = new Vector3(transform.parent.position.x + PORT_X, transform.parent.position.y, transform.parent.position.z + PORT_Z);
        }
        else //morado, derecha
        {
            porteriaDer = new Vector3(transform.parent.position.x + PORT_X, transform.parent.position.y, transform.parent.position.z + PORT_Z);
            porteriaIzq = new Vector3(transform.parent.position.x + PORT_X, transform.parent.position.y, transform.parent.position.z - PORT_Z);
            porteriaRivalDer = new Vector3(transform.parent.position.x - PORT_X, transform.parent.position.y, transform.parent.position.z + PORT_Z);
            porteriaRivalIzq = new Vector3(transform.parent.position.x - PORT_X, transform.parent.position.y, transform.parent.position.z - PORT_Z);
        }
    }

    public override void AgentReset()
    {
        transform.localPosition = new Vector3(team ? -5f : 5f, 0.5f, 0f);

        mc.ResetBall();
    }

    private List<float> PerceiveRays()
    {
        List<float> rays = new List<float>();

        float[] angles = new float[] { -60f, -30f, 0f, 30f, 60f };

        foreach (float a in angles)
        {
            RaycastHit hit;
            Vector3 dir = Quaternion.AngleAxis(a, transform.up) * transform.forward;
            if (Physics.Raycast(transform.localPosition, dir, out hit, perceptionDist, layers))
            {
                rays.Add(hit.distance);
                //Debug.Log(a + " " + hit.distance);
            }
            else
            {
                rays.Add(perceptionDist);
                //Debug.Log(a + " " + perceptionDist);
            }
        }

        return rays;
    }

    public override void CollectObservations()
    {
        //23

        //pelota (6)
        AddVectorObs(transform.InverseTransformPoint(pelota.transform.position));
        AddVectorObs(transform.InverseTransformVector(rbPelota.velocity));

        //porteria (12)
        AddVectorObs(transform.InverseTransformPoint(porteriaIzq));
        AddVectorObs(transform.InverseTransformPoint(porteriaDer));
        AddVectorObs(transform.InverseTransformPoint(porteriaRivalIzq));
        AddVectorObs(transform.InverseTransformPoint(porteriaRivalDer));

        //rayos (5)
        
        AddVectorObs(PerceiveRays());
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        float distPelota = Vector3.Distance(pelota.transform.localPosition, transform.localPosition);
        float distRivPorteria = Vector3.Distance(pelota.transform.position, new Vector3(porteriaRivalDer.x, porteriaRivalDer.y, (porteriaRivalDer.z + porteriaRivalIzq.z) / 2f));
        float distPorteria = Vector3.Distance(pelota.transform.position, new Vector3(porteriaDer.x, porteriaDer.y, (porteriaDer.z + porteriaIzq.z) / 2f));

        Vector3 newPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        newPos += transform.forward * Mathf.Clamp(vectorAction[1], -1f, 1f) * Speed * Time.deltaTime;
        newPos += transform.right * Mathf.Clamp(vectorAction[0], -1f, 1f) * Speed * Time.deltaTime;


        transform.localPosition = newPos;

        kickPower = vectorAction[1] > 0.1f ? Mathf.Clamp(vectorAction[1], -1f, 1f) : 0.25f;

        //AddReward(-100f / 2000f);

        

        //penalizar alejarse del balon
        //AddReward(-Time.deltaTime * Mathf.Clamp(Vector3.Distance(transform.localPosition, pelota.transform.localPosition),0f, 8f) / 400f);

        //AddReward(-Time.deltaTime * Mathf.Abs(transform.localPosition.z) / 200f);

        //recompensar si esta cerca el balon de la otra porteria
        //AddReward( Mathf.Clamp((-(Vector3.Distance(porteriaRivalDer, pelota.transform.position) + Vector3.Distance(porteriaRivalIzq, pelota.transform.position)) / 2f)+4f, -0.1f, 5f) / 200f);


        if (rbPelota.velocity.magnitude < 1.0f)
        {
            AddReward(-30f / 1000f);
        }

        if (distPelota <= Vector3.Distance(pelota.transform.localPosition, transform.localPosition))
        {
            AddReward(-10f / 1000f);
        } else
        {
            AddReward(20f / 1000f);
        }


        if (distRivPorteria <= Vector3.Distance(pelota.transform.position, new Vector3(porteriaRivalDer.x, porteriaRivalDer.y, (porteriaRivalDer.z + porteriaRivalIzq.z) / 2f)))
        {
            AddReward(-20f / 1000f);
        }
        else
        {
            AddReward(25f / 1000f);
        }
        if (distPorteria >= Vector3.Distance(pelota.transform.position, new Vector3(porteriaDer.x, porteriaDer.y, (porteriaDer.z + porteriaIzq.z) / 2f)))
        {
            AddReward(-30f / 1000f);
        }
        else
        {
            AddReward(20f / 1000f);
        }

        /*
        if (team)
        {
            if (pelota.transform.localPosition.x - transform.localPosition.x - 0.5f< 0f)
            {
                AddReward(-2f / 2000f);
            } else AddReward(1f / 2000f);
        } else
        {
            if (pelota.transform.localPosition.x - transform.localPosition.x + 0.5f > 0f)
            {
                AddReward(-2f / 2000f);
            } else AddReward(1f / 2000f);
        }*/
    }

    public override float[] Heuristic()
    {
        var action = new float[3];

        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        //action[2] = Input.GetAxis("Fire1");
        return action;
    }

    public float distanceToOwnGoal()
    {
        return Vector3.Distance(transform.position, new Vector3(porteriaDer.x, porteriaDer.y, (porteriaDer.z + porteriaIzq.z) / 2f));
    }

    void OnCollisionEnter(Collision c)
    {
        float force = 3500f * kickPower;
        if (c.gameObject.CompareTag("ball"))
        {
            AddReward(40f / 1000f);
            Vector3 dir = c.GetContact(0).point - transform.position;

            dir = dir.normalized;
            c.gameObject.GetComponent<Rigidbody>().AddForce(dir * force);

            MatchController.lastTouched = this;
        }
    }
}
