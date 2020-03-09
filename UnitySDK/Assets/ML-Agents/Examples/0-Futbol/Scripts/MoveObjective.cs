using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjective : MonoBehaviour
{
    public StrikerAgent agent;
    float reward = 60f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ball"))
        {
            agent.AddReward(reward);
            agent.Done();
        }
    }
}
