﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class AgentShoot : Agent
{
    public DebugCanvas graphicCanvas;

    CameraMovement cameraAgent;

    //float tolerableRange = 0.1f; //de -1 a 1 es el maximo posible, el rango es positivo>0

    float totalSteps = 1000f;

    //private float iX = 0f, iY = 0f;

    private float maxX = 0f, minX = 0f;
    bool iClick = true;

    List<float> lastX;
    List<float> lastY;

    List<float> lastClick; //pensar en hacer con probabilidad en vez de bools

    // lista movimientos reales para rewards (avg-std)
    List<float> realX;

    float minimumSTD = 0.1f;

    float initWeight = 10f;
    float loss = 1.25f;
    float minimum = 0.5f;

    const float AU_PROP = 0.618034f;

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
        realX = new List<float>();
        //lastY = new List<float>();
        //lastClick = new List<float>();

        for (int i = 0; i < 60; i++)
        {
            lastX.Add(0f);
            realX.Add(0f);
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

        maxX = Mathf.Clamp(vectorAction[0], -1f, 1f);
        minX = Mathf.Clamp(vectorAction[1], -1f, 1f);

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

        graphicCanvas.AddAveragePoint(average); //Debug std
        graphicCanvas.AddStdHighPoint(average + stDesv);
        graphicCanvas.AddStdLowPoint(average - stDesv);

        float coherence = Coherence(average, stDesv, oX);

        float agentStd = (maxX - minX)/2f; // debe dar positivo, sino recibe -100

        float reward = 0f;
        if (minX > maxX)
        {
            reward = -150f;
        }
        else
        {
            /*
            float precision = RelativePointsByDistance(minX, maxX, oX);

            float rewardFactor = 0f;
            float stdRelation = 1f;

            if (precision>0f)
            {
                rewardFactor = RewardFactor(coherence);
                stdRelation = DeviationFactor(stDesv, agentStd, coherence, true);
            }
            else
            {
                rewardFactor = PunishFactor(coherence);
                stdRelation = DeviationFactor(stDesv, agentStd, coherence, false);
            }

            reward = precision * rewardFactor * stdRelation;*/

            if (coherence>0f) // Movimiento coherente, solo penaliza
            {
                float punish = DistanceToStDeviations(minX, maxX, stDesv, average); //relativo al std

                reward = -20f * punish * coherence;
            }
            else
            {
                float precision = RelativePointsByDistance(minX, maxX, oX);

                reward = 100f * precision * (-coherence);
            }
        }

        

        AddReward(reward / totalSteps);

        /*
        if (minX > maxX) // No se permite maximo y minimo invertidos
        {
            reward = -100f;
        }
        else if (oX > maxX ||oX < minX) // Fuera de la estimacion
        {
            float factor = PunishFactor(coherence);
            float relPunish = RelativePunish(maxX, minX, oX, stDesv);

            reward = -(-1f + Mathf.Pow(1f + factor, relPunish)) * 20f;
        }
        else // Dentro de la estimacion
        {
            float factor = RewardFactor(coherence);
            float relReward = RelativeReward(maxX, minX, oX, stDesv);

            reward = (-1f + Mathf.Pow(1f + factor, relReward)) * 20f; 
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
        float total = 0f;

        float weight = initWeight;

        foreach (float f in previousMoves)
        {
            avg += (f * weight);
            total += weight;

            weight = Mathf.Max(minimum, weight / loss);
        }
        if (total > 0)
            return avg / total;
        else
            return 0f;
    }

    public float StdDeviation(ref List<float> previousMoves, float average)
    {
        float std = 0f;
        float total = 0f;

        float weight = initWeight;

        foreach (float f in previousMoves)
        {
            std += Mathf.Pow(f - average, 2) * weight;
            total += weight;

            weight = Mathf.Max(minimum, weight / loss);
        }

        if (total > 0)
            return std / total + minimumSTD;
        else
            return minimumSTD;
    }

    // Positiva = dentro de la desviacion tipica; Negativa = fuera (incoherente)
    // 0 si el punto esta en linea con la desviacion tipica
    public float Coherence(float avg, float std, float move)
    {
        float dist = Mathf.Abs(avg - move);

        std = Mathf.Max(0.001f, std);

        return (std - dist)/std;
    }

    /// <summary>
    /// Acierto del agente al envolver el punto de forma centrada
    /// Solo para movimientos incoherentes
    /// </summary>
    /// <param name="min">Linea minima del agente</param>
    /// <param name="max">Linea maxima del agente</param>
    /// <param name="point">Punto que ha dado el bot a imitar</param>
    /// <returns>Puntuacion por precision</returns>
    public float RelativePointsByDistance(float min, float max, float point)
    {
        float center = (min + max) / 2f;

        float radius = Mathf.Abs(max - min) / 2f;

        float precision = Mathf.Min(Mathf.Abs(center - point) / radius, 6f);

        return 1f / (precision + AU_PROP) - AU_PROP;
        //1/(x+0.618034)-0.618034
    }

    /// <summary>
    /// Acierto del agente en las desviaciones estandar
    /// Usado en movimientos coherentes
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="std"></param>
    /// <param name="avg"></param>
    /// <returns></returns>
    public float DistanceToStDeviations(float min, float max, float std, float avg)
    {
        float minDist = Mathf.Abs(min - (avg - std));
        float maxDist = Mathf.Abs(max - (avg + std));

        return (minDist + maxDist) / (2f * std);
    }
    /*
    /// <summary>
    /// Factor de recompensa en funcion de la coherencia del punto con su desviacion estandar
    /// Usa la formula 2^(x-1)
    /// </summary>
    /// <param name="coherence">Coherencia del punto (menor que 1)</param>
    /// <returns>Factor de recompensa</returns>
    public float RewardFactor(float coherence)
    {
        coherence = Mathf.Max(coherence, -5f);
        return Mathf.Pow(2, - coherence);
    }

    /// <summary>
    /// Factor de penalizacion en funcion de la coherencia del punto con su desviacion estandar
    /// Usa la formula 1/(x + 0.25) + 0.2
    /// </summary>
    /// <param name="coherence">Coherencia del punto (menor que 1)</param>
    /// <returns>Factor de castigo</returns>
    public float PunishFactor(float coherence)
    {
        return 1f / (1.25f - coherence) + 0.2f;
    }*/

    /// <summary>
    /// Factor multiplicador de la relacion entre la std original y la estimada
    /// Utiliza la formula (x-1)^3+1
    /// </summary>
    /// <param name="std">Desviacion estandar del bot</param>
    /// <param name="agentStd">Desviacion estandar del agente (max-min)</param>
    /// <returns>Relacion entre std y agentStd</returns>
    public float DeviationFactor(float std, float agentStd, float coherence, bool isReward)
    {
        std = Mathf.Max(0.000001f, std);
        agentStd = Mathf.Max(0.000001f, agentStd);

        float stdRelation = Mathf.Min(std / agentStd, 1000f);


        if (stdRelation >= 1f) // El factor solo se aplica si el std de la red es mayor al del bot
        {
            return 1f;
        }

        float coherenceFactor = Mathf.Pow(2f, 1f - coherence);

        if (isReward)
        {
            return Mathf.Pow(stdRelation - 1, 3) / coherenceFactor + 1;
        }
        else
        {
            return -(5 * Mathf.Pow(stdRelation - 1, 3)) / coherenceFactor + 1;
        }
    }

    /*
    public float RewardFactor(float coherence) //0-1
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

        std = Mathf.Max(std, 0.001f);

        //return 1f / (difference + 0.25f) + 1f;
        return -Mathf.Log((difference / std) / 2 + 0.01f) * 2f - 0.5f;
    }

    public float RelativePunish(float max, float min, float result, float std)
    {
        float avgDist = (Mathf.Abs(max - result) + Mathf.Abs(min - result)) / 2f;

        float difference = Mathf.Max(avgDist - std, 0f);


        std = Mathf.Max(std, 1f); // Para que no haya castigos positivos

        //return -1f / (difference - 2.25f) + 1f;
        return Mathf.Pow(2, difference / std + 1) - 4;
    }*/
}
