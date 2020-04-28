using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class AgentShoot : Agent
{
    public DebugCanvas graphicCanvas;

    CameraMovement cameraAgent;

    //float tolerableRange = 0.1f; //de -1 a 1 es el maximo posible, el rango es positivo>0

    float totalSteps = 2000f;

    //private float iX = 0f, iY = 0f;

    private float maxX = 0f, minX = 0f;
    bool iClick = true;

    List<float> lastX;
    List<float> lastY;

    List<float> lastClick; //pensar en hacer con probabilidad en vez de bools

    // lista movimientos reales para rewards (avg-std)
    List<float> realX;

    private void Start()
    {
        cameraAgent = GetComponent<CameraMovement>();
        
        lastX = new List<float>();

        //lastY = new List<float>();
        //lastClick = new List<float>();

        realX = new List<float>();

        for (int i = 0; i<60; i++)
        {
            lastX.Add(0f);
            //lastY.Add(0f);
            //lastClick.Add(0f);

            realX.Add(0f);
        }
    }

    public override void AgentReset()
    {
        lastX = new List<float>();
        //lastY = new List<float>();
        //lastClick = new List<float>();

        for (int i = 0; i < 60; i++)
        {
            lastX.Add(0f);
            //lastY.Add(0f);
            //lastClick.Add(0f);
        }
    }

    public override void CollectObservations()
    {
        
        int i = 0;
        while (i < 60)
        {
            AddVectorObs(lastX[i]);
            //AddVectorObs(lastY[i]);

            if (i <= 9) i++;
            else if (i <= 39) i += 3;
            else i += 4;
            //25x2
        }
    }


    public override void AgentAction(float[] vectorAction, string textAction)
    {
        //comparar la del bot con la del agente

        //iX = vectorAction[0];
        /*
        iY = vectorAction[1];
        iClick = vectorAction[2] > 0f;
        */

        maxX = vectorAction[0];
        minX = vectorAction[1];

        float oX = cameraAgent.GetX();
        //float oY = cameraAgent.GetY();

        //Debug canvas
        //graphicCanvas.AddBotPoint(iX); // No esta procesado aun             //CANVAS 2 PUNTOS
        graphicCanvas.AddAgentPoint(oX);

        graphicCanvas.AddBotPointDown(minX);
        graphicCanvas.AddBotPointUp(maxX);


        //float diffX = Mathf.Abs(iX - oX);
        //float diffY = Mathf.Abs(iY - oY);


        // Recompensas con desviacion tipica

        float average = Average(ref realX);
        float stDesv = StdDeviation(ref realX, average);

        float coherence = Coherence(average, stDesv, oX);

        float reward = 0f;

        if (minX > maxX) // No se permite
        {
            reward = -100f;
        }
        else if (oX > maxX ||oX < minX) // Fuera de la estimacion
        {
            float factor = PunishFactor(coherence);
            float relPunish = RelativePunish(maxX, minX, oX, stDesv);

            reward = -(-1f + Mathf.Pow(1f + factor, relPunish));
        }
        else // Dentro de la estimacion
        {
            float factor = RewardFactor(coherence);
            float relReward = RelativeReward(maxX, minX, oX, stDesv);

            reward = -1f + Mathf.Pow(1f + factor, relReward); 
        }

        AddReward(reward / 1000f); // se le puede restar un parametro de exigencia

        //recompensas por movimiento lineales
        //if (diffX > tolerableRange)
        /*
        if (diffX > ShotAcademy.tolerableRange)
        {
            AddReward(- 40f * (Mathf.Min(diffX, ShotAcademy.maximumRange) - ShotAcademy.tolerableRange) / (totalSteps * (ShotAcademy.maximumRange - ShotAcademy.tolerableRange)));
            //AddReward(-diffX / (3000f * ShotAcademy.tolerableRange));

        }
        else
        {
            AddReward(40f * (1f - diffX / ShotAcademy.tolerableRange) / totalSteps);
            //AddReward(Mathf.Clamp(ShotAcademy.tolerableRange / (diffX + 0.001f), 1f, 10f) / 3000f);
        }*/

        //el click no se compara exactamente igual, porque es instantaneo
        //AddReward click


        //actualizar listas
        lastX.RemoveAt(59);
        lastX.Insert(0, Mathf.Lerp(minX, maxX, Random.Range(0f, 1f))); // En vez de oX, para que no tenga "chuleta"

        realX.RemoveAt(59);
        realX.Insert(0, oX);
        //lastY.RemoveAt(59);
        //lastY.Insert(0, oY);
        
        //bools
    }

    public override void AgentOnDone()
    {
        base.AgentOnDone();
    }

    public float GetX()
    {
        return Random.Range(minX, maxX) * 2f; // Esta normalizado, para adaptar el agente
    }

    public float GetY()
    {
        return 0f * 2f;
    }

    public bool GetClick() // Cambiar por probabilidad
    {
        return iClick;
    }


    public override float[] Heuristic()
    {
        var action = new float[2]; //3

        action[0] = Input.GetAxis("Mouse X"); //clamp hasta un maximo posible
        action[1] = Input.GetAxis("Mouse Y");
        /*
        if (Input.GetMouseButtonDown(0))
        {
            action[2] = 1f;
        }
        else
        {
            action[2] = -1f;
        }*/

        return action;
    }



    public float Average(ref List<float> previousMoves)
    {
        float avg = 0f;
        foreach (float f in previousMoves)
        {
            avg += f;
        }

        return avg / previousMoves.Count;
    }

    public float StdDeviation(ref List<float> previousMoves, float average)
    {
        float std = 0f;
        foreach (float f in previousMoves)
        {
            std += Mathf.Pow(f - average, 2);
        }

        return std / previousMoves.Count;
    }

    // Positiva = dentro de la desviacion tipica; Negativa = fuera (incoherente)
    public float Coherence(float avg, float std, float move)
    {
        float dist = Mathf.Abs(avg - move);

        return (std - dist)/std;
    }

    public float RewardFactor(float coherence)
    {
        return 1f - Mathf.Pow(2, coherence - 1);
    }

    public float PunishFactor(float coherence)
    {
        return Mathf.Pow(2, coherence - 1);
    }

    public float RelativeReward(float max, float min, float result, float std)
    {
        float avgDist = (Mathf.Abs(max - result) + Mathf.Abs(min - result)) / 2f;

        float difference = Mathf.Max(avgDist - std, 0f);
        
        return 1f / (difference + 0.25f) + 1f;
    }

    public float RelativePunish(float max, float min, float result, float std)
    {
        float avgDist = (Mathf.Abs(max - result) + Mathf.Abs(min - result)) / 2f;

        float difference = Mathf.Max(avgDist - std, 0f);

        return -1f / (difference - 2.25f) + 1f;
    }
}
