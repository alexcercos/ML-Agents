using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCanvas : MonoBehaviour
{
    public LineRenderer agentLine;
    public LineRenderer botLineUp;
    public LineRenderer botLineDown;

    public LineRenderer stdLowLine;
    public LineRenderer stdHighLine;
    public LineRenderer avgLine;

    private Vector3[] agentPoints;
    private Vector3[] botPointsUp;
    private Vector3[] botPointsDown;

    private Vector3[] stdLowPoints;
    private Vector3[] stdHighPoints;
    private Vector3[] avgPoints;

    // Start is called before the first frame update
    void Start()
    {
        agentPoints = new Vector3[0];
        botPointsUp = new Vector3[0];
        botPointsDown = new Vector3[0];

        stdLowPoints = new Vector3[0];
        stdHighPoints = new Vector3[0];
        avgPoints = new Vector3[0];

        agentLine.SetPositions(agentPoints);
        botLineUp.SetPositions(botPointsUp);
        botLineDown.SetPositions(botPointsDown);

        stdLowLine.SetPositions(stdLowPoints);
        stdHighLine.SetPositions(stdHighPoints);
        avgLine.SetPositions(avgPoints);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddAgentPoint(float point)
    {
        AddLinePoint(ref agentLine, ref agentPoints, new Vector3(point * 20f, -50f, -1f));
    }

    public void AddBotPointUp(float point)
    {
        AddLinePoint(ref botLineUp, ref botPointsUp, new Vector3(point * 20f, -50f, 0f));
    }

    public void AddBotPointDown(float point)
    {
        AddLinePoint(ref botLineDown, ref botPointsDown, new Vector3(point * 20f, -50f, 0f));
    }

    public void AddStdLowPoint(float point)
    {
        AddLinePoint(ref stdLowLine, ref stdLowPoints, new Vector3(point * 20f, -50f, 0.6f));
    }

    public void AddStdHighPoint(float point)
    {
        AddLinePoint(ref stdHighLine, ref stdHighPoints, new Vector3(point * 20f, -50f, 0.6f));
    }

    public void AddAveragePoint(float point)
    {
        AddLinePoint(ref avgLine, ref avgPoints, new Vector3(point * 20f, -50f, 0.6f));
    }


    private void AddLinePoint(ref LineRenderer line, ref Vector3[] listPoints, Vector3 point)
    {
        if (listPoints.Length < 100)
        {
            Vector3[] newPoints = new Vector3[listPoints.Length + 1];

            newPoints[0] = point;

            for (int i = 0; i < listPoints.Length; i++)
            {
                newPoints[i + 1] = new Vector3(listPoints[i].x, listPoints[i].y + 1f, listPoints[i].z);
            }
            listPoints = newPoints;

            line.positionCount = listPoints.Length;
            line.SetPositions(listPoints);
        }
        else
        {
            Vector3[] newPoints = new Vector3[listPoints.Length];

            newPoints[0] = point;

            for (int i = 0; i < listPoints.Length - 1; i++)
            {
                newPoints[i + 1] = new Vector3(listPoints[i].x, listPoints[i].y + 1f, listPoints[i].z);
            }
            listPoints = newPoints;

            line.positionCount = listPoints.Length;
            line.SetPositions(listPoints);
        }
    }
}
