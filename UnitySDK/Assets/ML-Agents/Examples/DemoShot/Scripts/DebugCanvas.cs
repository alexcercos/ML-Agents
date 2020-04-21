using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCanvas : MonoBehaviour
{
    public LineRenderer agentLine;
    public LineRenderer botLine;

    private Vector3[] agentPoints;
    private Vector3[] botPoints;

    // Start is called before the first frame update
    void Start()
    {
        agentPoints = new Vector3[0];
        botPoints = new Vector3[0];

        agentLine.SetPositions(agentPoints);
        botLine.SetPositions(botPoints);
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

    public void AddBotPoint(float point)
    {
        if (botPoints.Length < 100)
        {
            Vector3[] newPoints = new Vector3[botPoints.Length + 1];

            newPoints[0] = new Vector3(point * 20f, -50f, 0f);

            for (int i = 0; i < botPoints.Length; i++)
            {
                newPoints[i + 1] = new Vector3(botPoints[i].x, botPoints[i].y + 1f, 0f);
            }
            botPoints = newPoints;

            botLine.positionCount = botPoints.Length;
            botLine.SetPositions(botPoints);
        }
        else
        {
            Vector3[] newPoints = new Vector3[botPoints.Length];

            newPoints[0] = new Vector3(point * 20f, -50f, 0f);

            for (int i = 0; i < botPoints.Length - 1; i++)
            {
                newPoints[i + 1] = new Vector3(botPoints[i].x, botPoints[i].y + 1f, 0f);
            }
            botPoints = newPoints;

            botLine.positionCount = botPoints.Length;
            botLine.SetPositions(botPoints);
        }
    }
}
