using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalControllerGoalie : MonoBehaviour
{
    public GoalieAgent agent;
    float punish = -20f;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ball"))
        {
            agent.AddReward(punish);
            agent.Done();
        }
    }
}
