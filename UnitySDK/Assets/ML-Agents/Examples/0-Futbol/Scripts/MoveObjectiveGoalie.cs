using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectiveGoalie : MonoBehaviour
{
    public GoalieAgent agent;
    float reward = 40f;
    void Update()
    {
        
        if (GoalieAcademy.velPelota > 0)
        {
            if (transform.localPosition.x>-13f)
                transform.localPosition = transform.localPosition + new Vector3( - 30f/4f * Time.deltaTime, 0f, 0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ball"))
        {
            agent.AddReward(reward + 13f - transform.localPosition.x);
            agent.Done();
        }
    }
}
