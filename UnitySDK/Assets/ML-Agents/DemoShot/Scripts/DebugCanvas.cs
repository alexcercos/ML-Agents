﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCanvas : MonoBehaviour
{
    public LineRenderer botLine;
    public LineRenderer agentLineUp;
    public LineRenderer agentLineDown;

    public LineRenderer stdLowLine;
    public LineRenderer stdHighLine;
    public LineRenderer avgLine;

    public LineRenderer clickBot;
    public LineRenderer clickAgent;

    public LineRenderer axisBotLine, axisAgentLine;

    public int axisLimit = 20;

    private Vector3[] botPoints;
    private Vector3[] agentPointsUp;
    private Vector3[] agentPointsDown;

    private Vector3[] stdLowPoints;
    private Vector3[] stdHighPoints;
    private Vector3[] avgPoints;

    private Vector3[] clickBotPoints, clickAgentPoints;

    private Vector3[] axisBotPoints, axisAgentPoints;

    public GameObject graphImage, axisImage;

    RectTransform graphTransform;

    bool lastShowGraphic = true;
    public bool showGraphic = true;
    bool lastShowAxis = false;
    public bool showAxis = false;

    // Start is called before the first frame update
    void Start()
    {
        botPoints = new Vector3[0];
        agentPointsUp = new Vector3[0];
        agentPointsDown = new Vector3[0];

        stdLowPoints = new Vector3[0];
        stdHighPoints = new Vector3[0];
        avgPoints = new Vector3[0];

        clickBotPoints = new Vector3[0];
        clickAgentPoints = new Vector3[0];

        axisBotPoints = new Vector3[0];
        axisAgentPoints = new Vector3[0];

        botLine.SetPositions(botPoints);
        agentLineUp.SetPositions(agentPointsUp);
        agentLineDown.SetPositions(agentPointsDown);

        stdLowLine.SetPositions(stdLowPoints);
        stdHighLine.SetPositions(stdHighPoints);
        avgLine.SetPositions(avgPoints);

        clickAgent.SetPositions(clickAgentPoints);
        clickBot.SetPositions(clickBotPoints);

        axisBotLine.SetPositions(axisBotPoints);
        axisAgentLine.SetPositions(axisBotPoints);

        graphTransform = graphImage.GetComponent<RectTransform>();

        if (graphTransform == null)
        {
            throw new System.Exception("Graph Image is not an UI element");
        }

        lastShowAxis = showAxis;
        if (showAxis)
            axisImage.SetActive(true);
        else
            axisImage.SetActive(false);

        lastShowGraphic = showGraphic;
        if (showGraphic)
            graphImage.SetActive(true);
        else
            graphImage.SetActive(false);

        if (showAxis && showGraphic)
        {
            graphTransform.anchoredPosition = new Vector2(-220f, -10f);
        }
        else
        {
            graphTransform.anchoredPosition = new Vector2(-10f, -10f);
        }
    }

    private void Update()
    {
        if (lastShowGraphic != showGraphic)
        {
            lastShowGraphic = showGraphic;

            if (showGraphic)
            {
                graphImage.SetActive(true);

                if (showAxis)
                {
                    graphTransform.anchoredPosition = new Vector2(-220f, -10f);
                }
                else
                {
                    graphTransform.anchoredPosition = new Vector2(-10f, -10f);
                }
            }
            else
            {
                graphImage.SetActive(false);
            }
        }

        if (lastShowAxis != showAxis)
        {
            lastShowAxis = showAxis;

            if (showAxis)
            {
                axisImage.SetActive(true);

                if (showGraphic)
                {
                    graphTransform.anchoredPosition = new Vector2(-220f, -10f);
                }
            }
            else
            {
                axisImage.SetActive(false);

                if (showGraphic)
                {
                    graphTransform.anchoredPosition = new Vector2(-10f, -10f);
                }
            }
        }
    }

    // Update is called once per frame
    public void UpdateLines()
    {
        UpdateClickLine(ref clickAgent, ref clickAgentPoints);
        UpdateClickLine(ref clickBot, ref clickBotPoints);
    }

    public void AddBotPoint(float point)
    {
        AddLinePoint(ref botLine, ref botPoints, new Vector3(point * 20f, -50f, -1f));
    }

    public void AddAgentPointUp(float point)
    {
        AddLinePoint(ref agentLineUp, ref agentPointsUp, new Vector3(point * 20f, -50f, 0f));
    }

    public void AddAgentPointDown(float point)
    {
        AddLinePoint(ref agentLineDown, ref agentPointsDown, new Vector3(point * 20f, -50f, 0f));
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

    public void AddClickLine(ref LineRenderer line, ref Vector3[] listPoints, float z)
    {
        Vector3[] newPoints = new Vector3[listPoints.Length + 3];

        newPoints[0] = new Vector3(-25f, -50f, z);
        newPoints[1] = new Vector3(25f, -50f, z);
        newPoints[2] = new Vector3(-25f, -50f, z);

        for (int i = 0; i < listPoints.Length; i++)
        {
            newPoints[i + 3] = new Vector3(listPoints[i].x, listPoints[i].y, listPoints[i].z);
        }

        listPoints = newPoints;

        line.positionCount = listPoints.Length;
        line.SetPositions(listPoints);
    }

    public void AddBotClick()
    {
        AddClickLine(ref clickBot, ref clickBotPoints, 0.2f);
    }

    public void AddAgentClick()
    {
        AddClickLine(ref clickAgent, ref clickAgentPoints, 0.3f);
    }

    public void AddBotAxisPoint(float x, float y)
    {
        if (showAxis)
            AddAxisPoint(ref axisBotLine, ref axisBotPoints, new Vector3(x * 50f, y * 50f, 10f));
    }

    public void AddAgentAxisPoint(float x, float y)
    {
        if (showAxis)
            AddAxisPoint(ref axisAgentLine, ref axisAgentPoints, new Vector3(x * 50f, y * 50f, 5f));
    }

    public void UpdateClickLine(ref LineRenderer line, ref Vector3[] listPoints)
    {
        if (listPoints.Length>=3 && listPoints[listPoints.Length-1].y > 50f)
        {
            Vector3[] newPoints = new Vector3[listPoints.Length - 3];
            for (int i = 0; i < newPoints.Length; i++)
            {
                newPoints[i] = new Vector3(listPoints[i].x, listPoints[i].y + 1f, listPoints[i].z);
            }

            listPoints = newPoints;
            line.positionCount = listPoints.Length;
            line.SetPositions(listPoints);
        }
        else
        {
            for (int i = 0; i < listPoints.Length; i++)
            {
                listPoints[i] = new Vector3(listPoints[i].x, listPoints[i].y + 1f, listPoints[i].z);
            }
            
            line.positionCount = listPoints.Length;
            line.SetPositions(listPoints);
        }
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

    private void AddAxisPoint(ref LineRenderer line, ref Vector3[] listPoints, Vector3 point)
    {
        if (listPoints.Length < axisLimit)
        {
            Vector3[] newPoints = new Vector3[listPoints.Length + 1];

            newPoints[0] = point;

            for (int i = 0; i < listPoints.Length; i++)
            {
                newPoints[i + 1] = new Vector3(listPoints[i].x, listPoints[i].y, listPoints[i].z);
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
                newPoints[i + 1] = new Vector3(listPoints[i].x, listPoints[i].y, listPoints[i].z);
            }
            listPoints = newPoints;

            line.positionCount = listPoints.Length;
            line.SetPositions(listPoints);
        }
    }
}
