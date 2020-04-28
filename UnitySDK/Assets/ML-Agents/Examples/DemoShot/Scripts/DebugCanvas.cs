using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCanvas : MonoBehaviour
{
    public LineRenderer agentLine;
    public LineRenderer botLineUp;
    public LineRenderer botLineDown;

    private Vector3[] agentPoints;
    private Vector3[] botPointsUp;
    private Vector3[] botPointsDown;

    // Start is called before the first frame update
    void Start()
    {
        agentPoints = new Vector3[0];
        botPointsUp = new Vector3[0];
        botPointsDown = new Vector3[0];

        agentLine.SetPositions(agentPoints);
        botLineUp.SetPositions(botPointsUp);
        botLineDown.SetPositions(botPointsDown);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddAgentPoint(float point)
    {
        if (agentPoints.Length < 100)
        {
            Vector3[] newPoints = new Vector3[agentPoints.Length + 1];

            newPoints[0] = new Vector3(point * 20f, -50f, 0f);

            for (int i = 0; i < agentPoints.Length; i++)
            {
                newPoints[i + 1] = new Vector3(agentPoints[i].x, agentPoints[i].y + 1f, 0f);
            }
            agentPoints = newPoints;

            agentLine.positionCount = agentPoints.Length;
            agentLine.SetPositions(agentPoints);
        } 
        else
        {
            Vector3[] newPoints = new Vector3[agentPoints.Length];

            newPoints[0] = new Vector3(point * 20f, -50f, 0f);

            for (int i = 0; i < agentPoints.Length-1; i++)
            {
                newPoints[i + 1] = new Vector3(agentPoints[i].x, agentPoints[i].y + 1f, 0f);
            }
            agentPoints = newPoints;

            agentLine.positionCount = agentPoints.Length;
            agentLine.SetPositions(agentPoints);
        }
    }

    public void AddBotPointUp(float point)
    {
        if (botPointsUp.Length < 100)
        {
            Vector3[] newPoints = new Vector3[botPointsUp.Length + 1];

            newPoints[0] = new Vector3(point * 20f, -50f, 0f);

            for (int i = 0; i < botPointsUp.Length; i++)
            {
                newPoints[i + 1] = new Vector3(botPointsUp[i].x, botPointsUp[i].y + 1f, 0f);
            }
            botPointsUp = newPoints;

            botLineUp.positionCount = botPointsUp.Length;
            botLineUp.SetPositions(botPointsUp);
        }
        else
        {
            Vector3[] newPoints = new Vector3[botPointsUp.Length];

            newPoints[0] = new Vector3(point * 20f, -50f, 0f);

            for (int i = 0; i < botPointsUp.Length - 1; i++)
            {
                newPoints[i + 1] = new Vector3(botPointsUp[i].x, botPointsUp[i].y + 1f, 0f);
            }
            botPointsUp = newPoints;

            botLineUp.positionCount = botPointsUp.Length;
            botLineUp.SetPositions(botPointsUp);
        }
    }

    public void AddBotPointDown(float point)
    {
        if (botPointsDown.Length < 100)
        {
            Vector3[] newPoints = new Vector3[botPointsDown.Length + 1];

            newPoints[0] = new Vector3(point * 20f, -50f, 0f);

            for (int i = 0; i < botPointsDown.Length; i++)
            {
                newPoints[i + 1] = new Vector3(botPointsDown[i].x, botPointsDown[i].y + 1f, 0f);
            }
            botPointsDown = newPoints;

            botLineDown.positionCount = botPointsDown.Length;
            botLineDown.SetPositions(botPointsDown);
        }
        else
        {
            Vector3[] newPoints = new Vector3[botPointsDown.Length];

            newPoints[0] = new Vector3(point * 20f, -50f, 0f);

            for (int i = 0; i < botPointsDown.Length - 1; i++)
            {
                newPoints[i + 1] = new Vector3(botPointsDown[i].x, botPointsDown[i].y + 1f, 0f);
            }
            botPointsDown = newPoints;

            botLineDown.positionCount = botPointsDown.Length;
            botLineDown.SetPositions(botPointsDown);
        }
    }
}
