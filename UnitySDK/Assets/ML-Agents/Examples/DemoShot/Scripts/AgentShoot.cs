using System.Collections;
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

    public ObservationsType receiveObservations = ObservationsType.NONE;

    private float maxX = 0f, minX = 0f;
    bool iClick = true;

    List<float> lastX;
    //List<float> lastY;

    //List<float> lastClick; //pensar en hacer con probabilidad en vez de bools

    // lista movimientos reales para rewards (avg-std)
    List<float> realX;

    float minimumSTD = 0.1f;

    public float initWeight = 1f;
    public float loss = 1f;
    public float minimum = 1f;

    const float AU_PROP = 0.618034f;

    float average = 0f;
    float stDesv = 0f;
    float oX = 0f;

    public float stdMultiplier = 3f;

    public float waitForImpulse = 0.25f;

    Queue<MoveInfo> realImpulse;
    Queue<MoveInfo> agentImpulse;

    //public float threshold = 0.01f;

    public bool demoHeuristic = true;

    private void Start()
    {
        cameraAgent = GetComponent<CameraMovement>();
        
        lastX = new List<float>();
        

        realX = new List<float>();

        for (int i = 0; i<60; i++)
        {
            lastX.Add(0f);

            realX.Add(0f);
        }

        realImpulse = new Queue<MoveInfo>();
        agentImpulse = new Queue<MoveInfo>();
    }

    public override void AgentReset()
    {
        /*
        lastX = new List<float>();
        realX = new List<float>();


        for (int i = 0; i < 60; i++)
        {
            lastX.Add(0f);
            realX.Add(0f);
        }*/
    }

    public override void CollectObservations()
    {
        if (receiveObservations == ObservationsType.PREVIOUS25)
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
        else if (receiveObservations == ObservationsType.LAST)
        {
            AddVectorObs(lastX[0]);
        }
    }


    public override void AgentAction(float[] vectorAction, string textAction)
    {
        //comparar la del bot con la del agente

        bool stdAgent = false;

        maxX = Mathf.Clamp(vectorAction[0], -1f, 1f);

        if (vectorAction.Length == 1)
        {
            minX = Mathf.Clamp(vectorAction[0], -1f, 1f);
        }
        else
        {
            stdAgent = true;
            minX = Mathf.Clamp(vectorAction[1], -1f, 1f);
        }
        

        oX = cameraAgent.GetBotX(); // Se usa para recompensas solo


        //Debug canvas

        graphicCanvas.AddAgentPoint(oX);

        graphicCanvas.AddBotPointDown(minX);
        if (stdAgent)
            graphicCanvas.AddBotPointUp(maxX);

        // Recompensas

        average = Average(ref realX);
        stDesv = StdDeviation(ref realX, average);

        //graphicCanvas.AddAveragePoint(average); //Debug std
        graphicCanvas.AddStdHighPoint(average + stDesv);
        graphicCanvas.AddStdLowPoint(average - stDesv);


        //RewardsInsideRange(average + stDesv, average - stDesv, maxX, 1f, 10f);

        //RewardsAfterImpulse(maxX, oX, ref realImpulse, ref agentImpulse, average, stDesv, 250f, 250f, 5000f);

        RewardsLinearOne(maxX, oX, average, stDesv, 100f, 10f);

        //actualizar listas

        lastX.RemoveAt(59);
        lastX.Insert(0, Mathf.Lerp(minX, maxX, Random.Range(0f, 1f))); // En vez de oX, para que no tenga "chuleta"

        //lastX.Insert(0, oX);

        realX.RemoveAt(59);
        realX.Insert(0, oX);


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
        var action = new float[1]; //3

        if (!demoHeuristic)
        {
            action[0] = Input.GetAxis("Mouse X"); //clamp hasta un maximo posible
            //action[1] = Input.GetAxis("Mouse Y");
        }
        else
        {
            oX = cameraAgent.GetBotX(); // Para asegurar que devuelve la del bot, aunque juegue el agente

            // DEMO RECORDER

            //0=max 1= min

            action[0] = oX;

            //action[0] = (average + stDesv < oX + threshold) ? oX + threshold : average + stDesv;
            //action[1] = (average - stDesv > oX - threshold) ? oX - threshold : average - stDesv;

        }

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

    public bool ReversedPunish(float minimumX, float maximumX, float punish)
    {
        if (minimumX > maximumX)
        {
            AddReward(punish / totalSteps);
            return true;
        }

        return false;
    }

    public void RewardsCoherence(float minimumX, float maximumX, float stDesvistion, float avg, float coherence, float originalX, float rFactor, float pFactor)
    {
        float reward = 0f;

        if (coherence > 0f) // Movimiento coherente, solo penaliza
        {
            float punish = DistanceToStDeviations(minimumX, maximumX, stDesvistion, avg); //relativo al std

            //Debug.Log("punish= " + punish);

            reward = pFactor * punish * coherence;
        }
        else
        {
            float precision = RelativePointsByDistance(minimumX, maximumX, originalX);

            //Debug.Log("precision= " + precision);

            reward = rFactor * precision * (-coherence);

        }
        //Debug.Log("coherence= " + coherence);
        //Debug.Log("reward= " + reward);

        AddReward(reward / totalSteps);
    }
    
    public void RewardsLinearIndividual(float minimumX, float maximumX, float originalX)
    {
        float relMin = originalX - minimumX;
        float relMax = maximumX - originalX;

        if (relMin > 0f)
            AddReward(2f * (1f - relMin) / totalSteps);
        else
            AddReward(2f * relMin / totalSteps);

        if (relMax > 0f)
            AddReward(2f * (1f - relMax) / totalSteps);
        else
            AddReward(2f * relMax / totalSteps);
    }

    public void RewardsLinearSimple(float minimumX, float maximumX, float originalX, float factor)
    {
        
        float reward = 0f;

        // Recompensas lineales simples

        // Si son positivas estan correctas, sino no han encerrado el punto

        float maxDist = maximumX - originalX;
        float minDist = originalX - minimumX;

        if (maxDist < 0f || minDist < 0f) // Solo puede ser una de las 2, sino estaria invertido
        {
            reward = (-1f - Mathf.Min(minDist, maxDist)) * factor;
        }
        else
        {
            reward = (1f - maxDist - minDist) * factor; // Positivo, hasta 1
        }
        
        AddReward(reward / totalSteps);
    }

    public void RewardsPrecision(float maximumX, float minimumX, float coherence, float std)
    {
        float agentStd = (maxX - minX) / 2f;

        float reward = 0f;

        // Recompensas por precision

        float precision = RelativePointsByDistance(minX, maxX, oX);

        float rewardFactor = 0f;
        float stdRelation = 1f;

        if (precision > 0f)
        {
            rewardFactor = RewardFactor(coherence);
            stdRelation = DeviationFactor(std, agentStd, coherence, true);
        }
        else
        {
            rewardFactor = PunishFactor(coherence);
            stdRelation = DeviationFactor(std, agentStd, coherence, false);
        }

        reward = precision * rewardFactor * stdRelation;
        

        AddReward(reward / totalSteps);
    }

    public void RewardsTolerableRange(float difference, float tolerableRange, float maxRange)
    {
        if (difference > tolerableRange)
        {
            //Lineales
            AddReward(-40f * (Mathf.Min(difference, tolerableRange) - tolerableRange) / (totalSteps * (maxRange - tolerableRange)));
            //No lineales
            //AddReward(-diffX / (3000f * tolerableRange));

        }
        else
        {
            //Lineales
            AddReward(40f * (1f - difference / tolerableRange) / totalSteps);
            //No lineales
            //AddReward(Mathf.Clamp(tolerableRange / (difference + 0.001f), 1f, 10f) / 3000f);
        }
    }

    public void RewardsStdExponential(float originalX, float minimumX, float maximumX, float stDesvistion, float avg, float coherence, float mulFactor)
    {
        float reward;
        if (originalX > maximumX || originalX < minimumX) // Fuera de la estimacion
        {
            float factor = PunishFactor(coherence);
            float relPunish = RelativePunish(maximumX, minimumX, originalX, stDesvistion);

            reward = -(-1f + Mathf.Pow(1f + factor, relPunish)) * mulFactor; //mulFactor=20
        }
        else // Dentro de la estimacion
        {
            float factor = RewardFactor(coherence);
            float relReward = RelativeReward(maximumX, minimumX, originalX, stDesvistion);

            reward = (-1f + Mathf.Pow(1f + factor, relReward)) * mulFactor;
        }

        AddReward(reward / totalSteps);
    }

    public void RewardsInsideRange(float maximum, float minimum, float move, float mulFactor, float punFactor)
    {
        // Maximo y minimo son avg +- std

        if (move <= maximum && move >=minimum)
        {
            AddReward(1f * mulFactor / totalSteps);
        }
        else
        {
            float outside = Mathf.Max(move - maximum, minimum - move); // Distancia absoluta

            AddReward(-outside * punFactor / totalSteps);
        }
    }

    public void RewardsAfterImpulse(float move, float originalX, ref Queue<MoveInfo> impulseReal, ref Queue<MoveInfo> impulseAgent, float averageMove, float std, float rewardFactor, float failFactor, float missFactor)
    {
        if (move > averageMove + std || move < average - std)
        {
            impulseAgent.Enqueue(new MoveInfo(Time.frameCount, move));
        }
        if (originalX > averageMove + std || originalX < average - std)
        {
            impulseAgent.Enqueue(new MoveInfo(Time.frameCount, originalX));
        }

        while (impulseReal.Count > 0 && impulseAgent.Count > 0)
        {
            // Los 2 tienen, se comparan directamente
            MoveInfo original = impulseReal.Dequeue();
            MoveInfo agent = impulseAgent.Dequeue();

            float timeDiff = (original.frame - agent.frame) * Time.deltaTime;
            float relativeReward = Mathf.Exp(-5f * Mathf.Pow(timeDiff, 2f)) - 0.05f; //Gauss

            float difference = 0f;
            float diffFactor = -1f;

            if (Mathf.Sign(original.move) == Mathf.Sign(agent.move)) // Distinto sentido no da recompensa
            {
                difference = Mathf.Abs(original.move - agent.move);

                diffFactor = Mathf.Pow(2f, 0.5f - 5 * difference);
            }

            AddReward(relativeReward * rewardFactor * diffFactor / totalSteps);

            if (impulseAgent.Peek().frame > agent.frame + 1)
            {
                // Eliminar movimiento completo restante del original

                while (impulseReal.Peek().frame == original.frame + 1)
                {
                    original = impulseReal.Dequeue();

                    // No se penaliza
                }
            }
        }

        while (impulseReal.Count > 0 && Mathf.Abs(impulseReal.Peek().frame - Time.frameCount) * Time.deltaTime > waitForImpulse)
        {
            // Hay movimientos y ya ha pasado su tiempo maximo, no se recompensan

            MoveInfo thisMove = impulseReal.Dequeue();

            float outside = Mathf.Max(thisMove.move - (averageMove + std), (averageMove - std) - thisMove.move); // Distancia absoluta

            AddReward(-outside * missFactor / totalSteps);
        }

        while (impulseAgent.Count > 0 && Mathf.Abs(impulseAgent.Peek().frame - Time.frameCount) * Time.deltaTime > waitForImpulse)
        {
            // Ha hecho un movimiento a destiempo

            MoveInfo thisMove = impulseAgent.Dequeue();

            float outside = Mathf.Max(thisMove.move - (averageMove + std), (averageMove - std) - thisMove.move); // Distancia absoluta

            AddReward(-outside * failFactor / totalSteps);
        }
    }

    public void RewardsLinearOne(float move, float original, float average, float std, float rewardFactor, float punishFactor)
    {
        // Para entrenamientos con una accion, puntua impulsos y penaliza coherentes
        float reward = 0f;

        if (original > average - std && original < average + std) // Movimiento coherente, solo penaliza
        {
            float punish = 2f - Mathf.Abs(original - move);
            

            reward = -punishFactor * punish;
        }
        else
        {
            float precision = 0.2f - Mathf.Abs(original - move);
            

            reward = rewardFactor * precision;

        }

        AddReward(reward / totalSteps);
    }

    public float Average(ref List<float> previousMoves)
    {
        // Para el bot uniforme
        return -0.15f;

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
        // Para el bot uniforme
        return 0.02f;

        float std = 0f;
        float total = 0f;

        float weight = initWeight;

        foreach (float f in previousMoves)
        {
            std += Mathf.Pow(f - average, 2) * weight * stdMultiplier;
            total += weight;

            weight = Mathf.Max(minimum, weight / loss);
        }

        if (total > 0)
            return Mathf.Sqrt(std / total);
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
    }
    
}

public struct MoveInfo
{
    public int frame;
    public float move;

    public MoveInfo(int f, float m)
    {
        frame = f;
        move = m;
    }
}

public enum ObservationsType
{
    PREVIOUS25, LAST, NONE
}