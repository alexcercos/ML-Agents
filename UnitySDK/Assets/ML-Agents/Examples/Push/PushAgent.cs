using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class PushAgent : Agent
{
    public GameObject Cubo;
    public GameObject Objetivo;
    public float Velocidad = 3f;
    public float FuerzaSalto = 14f;
    public bool enSuelo = false;
    private Rigidbody rb;
    private BoxCollider bc;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();
    }

    public override void AgentReset()
    {
        
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        ActualizarEnSuelo();

        switch ((int)vectorAction[0])
        {
            case 0:
                Mover(transform.right);
                break;
            case 1:
                Mover(-transform.right);
                break;
            case 2:
                if (enSuelo)
                    Saltar();
                break;
        }
    }

    private void Mover(Vector3 direccion)
    {
        rb.MovePosition(rb.position + direccion * Velocidad * Time.fixedDeltaTime);
    }

    private void Saltar()
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(transform.up * FuerzaSalto, ForceMode.Impulse);
    }

    public override void CollectObservations()
    {
        AddVectorObs(Cubo.transform.localPosition.x - transform.localPosition.x);
        AddVectorObs(transform.localPosition.y);
        AddVectorObs(Objetivo.transform.localPosition.x > Cubo.transform.localPosition.x ? 1f : 0f);
        AddVectorObs(enSuelo ? 1f : 0f);
    }

    private void ActualizarEnSuelo()
    {
        enSuelo = false;

        Collider[] colliders = Physics.OverlapBox(transform.position + Vector3.down * 0.05f, bc.size / 2f, transform.rotation);
        foreach (Collider c in colliders)
        {
            if (c!=null && c.CompareTag("walkableSurface"))
            {
                enSuelo = true;
                break;
            }
        }
    }

    public override float[] Heuristic()
    {
        float[] action = new float[1];
        action[0] = -1;

        if (Input.GetKey(KeyCode.RightArrow))
        {
            action[0] = 0;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            action[0] = 1;
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            action[0] = 2;
        }
        return action;
    }
}
