using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalController : MonoBehaviour
{
    //public GoalieAgent agent;
    //float punish = -20f;

    public StrikerAgent agent;
    float punish = -50f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ball"))
        {
            agent.AddReward(punish);
            agent.Done();
        }
    }
}
