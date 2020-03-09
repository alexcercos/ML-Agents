using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class FinalGoalie : Agent
{
    MatchController mc;

    const float PORT_X = 15f;
    const float PORT_Z = 4.1f;

    public float Speed = 8f;
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
            porteriaDer = new Vector3(transform.parent.position.x-PORT_X, transform.parent.position.y, transform.parent.position.z - PORT_Z);
            porteriaIzq = new Vector3(transform.parent.position.x - PORT_X, transform.parent.position.y, transform.parent.position.z + PORT_Z);
            porteriaRivalDer = new Vector3(transform.parent.position.x+PORT_X, transform.parent.position.y, transform.parent.position.z- PORT_Z);
            porteriaRivalIzq = new Vector3(transform.parent.position.x+PORT_X, transform.parent.position.y, transform.parent.position.z+PORT_Z);
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
        transform.localPosition = new Vector3(team?-15f:15f, 0.5f, 0f);

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

        //Debug.Log("p "+transform.InverseTransformPoint(pelota.transform.position));
        //Debug.Log("v "+transform.InverseTransformVector(rbPelota.velocity));

        //Debug.Log("1 "+transform.InverseTransformPoint(porteriaIzq));
        //Debug.Log("2 " + transform.InverseTransformPoint(porteriaDer));
        //Debug.Log("3 " + transform.InverseTransformPoint(porteriaRivalIzq));
        //Debug.Log("4 " + transform.InverseTransformPoint(porteriaRivalDer));

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
        Vector3 lastPos = new Vector3(transform.position.x, transform.position.y, transform.position.y);

        Vector3 newPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        newPos += transform.forward * Mathf.Clamp(vectorAction[1], -1f, 1f) * Speed * Time.deltaTime;
        newPos += transform.right * Mathf.Clamp(vectorAction[0], -1f, 1f) * Speed * Time.deltaTime;


        transform.localPosition = newPos;

        kickPower = vectorAction[1] > 0.1f ? Mathf.Clamp(vectorAction[1]/2f, -1f, 1f) : 0.25f;

        if (Vector3.Distance(pelota.transform.position, new Vector3(porteriaDer.x, porteriaDer.y, (porteriaDer.z + porteriaIzq.z) / 2f)) < 7f){
            if (distPelota <= Vector3.Distance(pelota.transform.localPosition, transform.localPosition))
            {
                AddReward(-10f / 1000f);
            }
            else
            {
                AddReward(15f / 1000f);
            }

            if (Vector3.Distance(pelota.transform.position, lastPos)<= Vector3.Distance(pelota.transform.position, transform.position))
            {
                AddReward(-50f / 1000f);
            }
            else
            {
                AddReward(30f / 1000f);
            }
        }
        

        /*

        //penalizar alejarse de la porteria
        AddReward( Mathf.Clamp(-Vector3.Distance(transform.position, new Vector3(porteriaDer.x, porteriaDer.y+0.5f, (porteriaDer.z+porteriaIzq.z)/2f)) + 1.5f, -3f, 2f) * Time.deltaTime);
        if (Mathf.Abs(transform.localPosition.x) >= 16f) AddReward(-Time.deltaTime * 8f);

        //penalizar cercania de balon
        float dist = Mathf.Clamp((Vector3.Distance(porteriaDer, pelota.transform.position) + Vector3.Distance(porteriaIzq, pelota.transform.position)) / 2f, 0f, 6.5f);
        AddReward(( dist - 6f) * Time.deltaTime / 5f);
        if (dist < 8f)
        {
            AddReward((-Vector3.Distance(transform.localPosition, pelota.transform.localPosition)+1f) * Time.deltaTime / 2f);
        }*/

        AddReward(10f/1000f);

        float zcoord = Mathf.Abs(transform.localPosition.z);
        if (zcoord > 2f)
        {//Anular recompensa si se aleja
            AddReward(-3f * (1.0f + (zcoord-3f) / 2f) / 1000f );
        }
        float xcoord = Mathf.Abs(Mathf.Abs(transform.localPosition.x)-PORT_X);
        if (xcoord > 1f)
        {//Anular recompensa si se aleja
            AddReward(-3f * (1.0f + (xcoord-1f) / 2f) / 1000f);
        }

    }

    public override float[] Heuristic()
    {
        var action = new float[3];

        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        //action[2] = Input.GetAxis("Fire1");
        return action;
    }

    void OnCollisionEnter(Collision c)
    {
        float force = 3500f * kickPower;
        if (c.gameObject.CompareTag("ball"))
        {
            AddReward(400f * kickPower / 1000f);

            Vector3 dir = c.GetContact(0).point - transform.position;

            dir = dir.normalized;
            c.gameObject.GetComponent<Rigidbody>().AddForce(dir * force);

            MatchController.lastTouched = this;
        }
    }
}
