using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalCollider : MonoBehaviour
{
    public bool team;
    private MatchController mc;

    private void Awake()
    {
        mc = transform.parent.Find("SoccerFieldTwos").GetComponent<MatchController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ball"))
        {
            mc.GoalScored(team);
        }
    }
}
