using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoaliePunish : MonoBehaviour
{
    public bool team;
    private MatchController mc;

    float maxTime = 8f;
    float timer = 0f;

    private void Awake()
    {
        mc = transform.parent.Find("SoccerFieldTwos").GetComponent<MatchController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ball"))
        {
            timer = 0f;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("ball"))
        {
            timer += Time.fixedDeltaTime;

            mc.PunishConstGoalie(team);
            if (timer >= maxTime)
            {
                timer = 0f;
                mc.PunishGoalie(team);
            }

        }
    }
}
