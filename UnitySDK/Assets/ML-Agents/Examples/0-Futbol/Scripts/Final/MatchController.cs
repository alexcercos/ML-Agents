using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class MatchController : MonoBehaviour
{
    /*
    float goalieReward = 5f;
    float strikerReward = 80f;
    float goaliePunish = -150f;
    float strikerPunish = -10f;*/

    float goalieReward = 0f;
    float strikerReward = 50f;
    float goaliePunish = -80f;
    float strikerPunish = -5f;

    float goalieRetention = 0f;

    float multGoalieRew = 10f;
    float multStrikerRew = 1.25f;
    float multStrikerPun = 10f;
    float multGoaliePun = 0.9f;

    private FinalGoalie blueGoalie;
    private FinalStriker blueStriker;
    private FinalGoalie purpleGoalie;
    private FinalStriker purpleStriker;

    GameObject pelota;
    Rigidbody rbPelota;

    public static Agent lastTouched = null;

    private void Awake()
    {
        blueGoalie = transform.parent.Find("BlueGoalie").GetComponent<FinalGoalie>();
        blueStriker = transform.parent.Find("BlueStriker").GetComponent<FinalStriker>();
        purpleGoalie = transform.parent.Find("PurpleGoalie").GetComponent<FinalGoalie>();
        purpleStriker = transform.parent.Find("PurpleStriker").GetComponent<FinalStriker>();

        pelota = transform.parent.Find("Soccer Ball").gameObject;
        rbPelota = pelota.GetComponent<Rigidbody>();
        
    }

    public void ResetBall()
    {
        rbPelota.velocity = new Vector3(Random.Range(0f, 1f)>0.5 ? -2f : 2f, 0f, 0f);
        rbPelota.angularVelocity = Vector3.zero;
        pelota.transform.localRotation = Quaternion.Euler(Vector3.zero);
        pelota.transform.localPosition = new Vector3(0f, 1.0f, 0f);
    }

    void ResetMatch(bool team)
    {
        blueGoalie.Done();
        purpleGoalie.Done();
        blueStriker.Done();
        purpleStriker.Done();
    }

    public void GoalScored(bool team)
    {
        if (team)
        {
            blueGoalie.AddReward((blueGoalie == lastTouched)?strikerReward * multGoalieRew: goalieReward);
            blueStriker.AddReward((blueStriker == lastTouched) ? strikerReward * multStrikerRew : strikerReward);

            purpleGoalie.AddReward((purpleGoalie == lastTouched) ? goaliePunish * multGoaliePun : goaliePunish);
            purpleStriker.AddReward((purpleStriker == lastTouched) ? strikerPunish * multStrikerPun : strikerPunish * purpleStriker.distanceToOwnGoal()/10f);
        }
        else
        {
            purpleGoalie.AddReward((purpleGoalie == lastTouched) ? strikerReward * multGoalieRew : goalieReward);
            purpleStriker.AddReward((purpleStriker == lastTouched) ? strikerReward * multStrikerRew : strikerReward);

            blueGoalie.AddReward((blueGoalie == lastTouched) ? goaliePunish * multGoaliePun : goaliePunish);
            blueStriker.AddReward((blueStriker == lastTouched) ? strikerPunish * multStrikerPun : strikerPunish*blueStriker.distanceToOwnGoal() / 8f);
        }
        ResetMatch(!team);
    }

    public void PunishGoalie(bool team)
    {
        if (team) blueGoalie.AddReward(goalieRetention);
        else purpleGoalie.AddReward(goalieRetention);

        ResetMatch(team);
    }

    public void PunishConstGoalie(bool team)
    {
        if (team) blueGoalie.AddReward(-Time.fixedDeltaTime / Time.deltaTime * 2f / 2000f);
        else purpleGoalie.AddReward(-Time.fixedDeltaTime / Time.deltaTime * 2f / 2000f);
    }
}
