using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class Ball3DEjerciciosAgent : Agent
{
    public GameObject Pelota;

    private Rigidbody rbPelota;

    public float velocidadRotacion = 120f;

    private void Awake()
    {
        rbPelota = Pelota.GetComponent<Rigidbody>();
    }

    public override void AgentReset()
    {
        transform.rotation = Quaternion.identity;
        rbPelota.velocity = Vector3.zero;

        Pelota.transform.localPosition = new Vector3(0, 0.5f, 0);
    }

    public override void CollectObservations()
    {
        AddVectorObs(transform.localRotation.x);
        AddVectorObs(transform.localRotation.z);
        AddVectorObs(Pelota.transform.localPosition);
        AddVectorObs(rbPelota.velocity);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        float rotacionHorizontal = 0, rotacionVertical = 0;

        rotacionHorizontal = Mathf.Clamp(vectorAction[0], -1, 1);
        rotacionVertical = Mathf.Clamp(vectorAction[1], -1, 1);

        transform.Rotate(Vector3.forward, rotacionHorizontal * velocidadRotacion * Time.deltaTime);

        transform.Rotate(Vector3.right, rotacionVertical * velocidadRotacion * Time.deltaTime);



        if (!IsDone())
        {
            AddReward(0.1f);
        }

        if ((Pelota.transform.localPosition.y < -0.5f) || Mathf.Abs(Pelota.transform.localPosition.x) > 0.55f
            || Mathf.Abs(Pelota.transform.localPosition.z) > 0.55f)
        {
            AddReward(-10f);
            Done();
        }

    }
}
