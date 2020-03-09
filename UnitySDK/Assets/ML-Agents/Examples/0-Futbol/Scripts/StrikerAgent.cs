using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class StrikerAgent : Agent
{
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
        rbPelota.velocity = Vector3.zero;
        rbPelota.angularVelocity = Vector3.zero;
        pelota.transform.localRotation = Quaternion.Euler(Vector3.zero);

        //para entrenamiento curriculum con agente azul

        transform.localPosition = new Vector3(Random.Range(StrikerAcademy.minZ, StrikerAcademy.maxZ), 0.5f, Random.Range(-StrikerAcademy.posicionX, StrikerAcademy.posicionX));

        pelota.transform.localPosition = new Vector3(Random.Range(StrikerAcademy.pelotaMinZ, StrikerAcademy.pelotaMaxZ), 1f, Random.Range(-StrikerAcademy.xPelota, StrikerAcademy.xPelota));


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
        Vector3 newPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        newPos += transform.forward * Mathf.Clamp(vectorAction[1], -1f, 1f) * Speed * Time.deltaTime;
        newPos += transform.right * Mathf.Clamp(vectorAction[0], -1f, 1f) * Speed * Time.deltaTime;


        transform.localPosition = newPos;

        kickPower = vectorAction[1] > 0f ? Mathf.Clamp(vectorAction[1], -1f, 1f) : 0.5f;

        //penalizar tiempo
        AddReward(-Time.deltaTime * Vector3.Distance(transform.localPosition, pelota.transform.localPosition) / 5f);

        AddReward(((-(Vector3.Distance(porteriaRivalDer, pelota.transform.position) + Vector3.Distance(porteriaRivalIzq, pelota.transform.position)) / 2f)+5f)*Time.deltaTime/2f);

        //transform.Rotate(transform.up, RotationSpeed * Mathf.Clamp(vectorAction[2], -1f, 1f), Space.Self);
        /*
        if (GoalieAcademy.velPelota == 0)
        {
            AddReward(-Time.deltaTime);
        }*/
        //penalizar por girar
        //AddReward(-Mathf.Abs(Mathf.Clamp(vectorAction[2], -1f, 1f)) * Time.deltaTime);
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
        float force = 2000f * kickPower;
        if (c.gameObject.CompareTag("ball"))
        {
            Vector3 dir = c.GetContact(0).point - transform.position;

            dir = dir.normalized;
            c.gameObject.GetComponent<Rigidbody>().AddForce(dir * force);
        }
    }
}
