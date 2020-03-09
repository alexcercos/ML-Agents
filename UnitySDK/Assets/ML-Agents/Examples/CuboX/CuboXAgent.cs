using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class CuboXAgent : Agent
{
    public GameObject Objetivo;
    public float Speed = 3f;

    public override void AgentReset()
    {
        gameObject.transform.localPosition = Vector3.zero;
        Objetivo.transform.localPosition = ObtenerPosicionObjetivo(CuboXAcademy.Separacion);
    }

    private Vector3 ObtenerPosicionObjetivo(float separacion)
    {
        return new Vector3(Random.Range(1.05f, separacion) * (Random.value <= 0.5 ? 1 : -1), 0f, Random.Range(1.05f, separacion) * (Random.value <= 0.5 ? 1 : -1));
    }

    public override void CollectObservations()
    {
        AddVectorObs(transform.localPosition.x);
        AddVectorObs(transform.localPosition.z);
        AddVectorObs(Objetivo.transform.localPosition.x);
        AddVectorObs(Objetivo.transform.localPosition.z);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        //float distanciaInicialAlObjetivo = Vector3.Distance(Objetivo.transform.localPosition, transform.localPosition);

        float newX = transform.localPosition.x + (vectorAction[0] * Speed * Time.deltaTime);
        newX = Mathf.Clamp(newX, -7.3f, 7.3f);

        float newZ = transform.localPosition.z + (vectorAction[1] * Speed * Time.deltaTime);
        newZ = Mathf.Clamp(newZ, -7.3f, 7.3f);

        transform.localPosition = new Vector3(newX, 0, newZ);

        /*
        float distanciaFinalAlObjetivo = Vector3.Distance(Objetivo.transform.localPosition, transform.localPosition);

        if (distanciaFinalAlObjetivo < distanciaInicialAlObjetivo)
        {
            AddReward(0.1f);
        }
        else
        {
            AddReward(-0.3f);
        }*/
    }

    public override float[] Heuristic()
    {
        var action = new float[2];

        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        return action;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Objetivo"))
        {
            AddReward(30f);
            Done();
        }
    }
}
