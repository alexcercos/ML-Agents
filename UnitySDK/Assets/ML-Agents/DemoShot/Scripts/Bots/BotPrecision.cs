using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotPrecision : IBotMovement
{
    //Igual que botOneMove pero con imprecision al escoger objetivos

    Transform scene;

    AgentShoot agentShoot;

    public float imprecision = 1.2f;

    public static float FPS = 60f;

    float xMove = 0f, yMove = 0f;

    float interpX = 0f, interpY = 0f; //De 0 a 1 (pero puede salir)
    float done = 0f; // para interpolar la funcion (de 0 a 1)
    float last = 0f;

    public float moveNoiseX = 0.0f;
    public float moveNoiseY = 0.0f;

    public List<AnimationCurve> curves;
    //public AnimationCurve shapeX, shapeY;

    public float angleX, angleY = 0f;
    float currentX, currentY = 0f; //angulo actual


    public float timeClic = 0.95f;
    public float clicVariance = 0.05f;

    float nextTimeClic = 0.95f;

    public float time = 0f;
    public bool perform = false; //trigger (si no es auto)

    bool idle;

    bool doClic = false;

    bool cheatRewardsClicked = false;

    public bool invertIdle = false;

    public List<AnimationCurve> shapedTRCurves;
                                                // 0: idle tolerable range shape
                                                // 1: idle punish factor
                                                // 2: idle reward factor
                                                // 3: seek tolerable range shape
                                                // 4: seek punish factor
                                                // 5: seek reward factor

    void Start()
    {
        scene = GameObject.Find("Scene").transform;

        agentShoot = GetComponent<AgentShoot>();

        nextTimeClic = timeClic + Random.Range(-clicVariance, clicVariance);
    }

    private void Update()
    {
        //movimientos solo valen en el rango -1, 1
        
        if (perform)
        {
            PerformMove();
        }
        else
        {
            idle = false;
            SearchForObjective();
            //buscar nuevo target
            xMove = 0f; //reinicia en el siguiente frame
            yMove = 0f;

            doClic = false;
        }

        
    }

    public void PerformMove()
    {
        
        last = done;
        done += 1 / (FPS * time);

        if (doClic) doClic = false;
        else if (done > nextTimeClic && last < nextTimeClic && !idle)
        {
            doClic = true;
        }

        //interpX = done; //lineal ahora
        //interpY = done;

        if (idle)
        {
            //Las curvas eran 0 en el apartado de click, 1 en movimiento en los ejes
            interpX = curves[2].Evaluate(done); //shape X
            interpY = curves[2].Evaluate(done); //shape Y

            SearchForObjective();
        }
        else
        {
            interpX = curves[1].Evaluate(done); //shape X
            interpY = curves[1].Evaluate(done); //shape Y
        }

        //float limitDecay = 2f * (Mathf.Abs(done - 0.5f) - 0.5f); //en 0 y 1 no hay ruido para asegurar que acaba en el sitio
        float limitDecay = 1f - Mathf.Pow(2f * done - 1f, 2f);
        float timeDecay = Mathf.Clamp((time-0.2f)*3f, 0f, 1f); //movimientos cortos sin ruido

        float newX = OutLerp(0f, angleX, interpX) + (Random.Range(-moveNoiseX, moveNoiseX) * limitDecay * timeDecay);
        float newY = OutLerp(0f, angleY, interpY) + (Random.Range(-moveNoiseY, moveNoiseY) * limitDecay * timeDecay); //nuevos angulos
        

        //xMove = (angleX * CameraMovement.width) / (time * FPS * CameraMovement.maxSpeed * CameraMovement.camAngleHor);
        //yMove = (angleY * CameraMovement.height) / (time * FPS * CameraMovement.maxSpeed * CameraMovement.camAngleVer);

        xMove = ((newX - currentX) * CameraMovement.width) / (CameraMovement.maxSpeed * CameraMovement.camAngleHor);
        yMove = ((newY - currentY) * CameraMovement.height) / (CameraMovement.maxSpeed * CameraMovement.camAngleVer);

        currentX = newX;
        currentY = newY;


        if (done >= 1f)
        {
            if (!idle && !cheatRewardsClicked)
                agentShoot.ClickRewardsCheatMissed();

            cheatRewardsClicked = false;
            perform = false;
            last = 0f;
            done = 0f;
            currentX = 0f;
            currentY = 0f;

            nextTimeClic = timeClic + Random.Range(-clicVariance, clicVariance);
        }
    }

    public void ResetAgent()
    {
        cheatRewardsClicked = false;
        perform = false;
        last = 0f;
        done = 0f;
        currentX = 0f;
        currentY = 0f;

        nextTimeClic = timeClic + Random.Range(-clicVariance, clicVariance);
    }

    public override bool Click()
    {
        
        if (doClic)
            agentShoot.BotClickEvent();

        return doClic;
        /*
        if (doClic)
        {
            //Debug.Log("clicked at" + done);
            doClic = false;
            return true;
        }
        else
            return false;*/
    }

    public override float MouseX()
    {
        //if (Mathf.Abs(xMove) > 1f) Debug.Log("X overflow: " + xMove);
        return xMove * 2f;
    }

    public override float MouseY()
    {
        //if (Mathf.Abs(yMove) > 1f) Debug.Log("Y overflow: " + yMove);
        return yMove * 2f;
    }

    private void SearchForObjective()
    {
        //Cuando acaba un movimiento, busca objetivo

        //Considerar angulo en Y cuando es idle

        
        float closest = 1f; //0.51f
        foreach (Renderer t in scene.GetComponentsInChildren<Renderer>())
        {
            
            //if (t.CompareTag("decal")) continue;
            /*
            Vector3 screenPos = Camera.main.WorldToViewportPoint(t.transform.position);
            if (screenPos.z > 0f)
            {
                float dist = Mathf.Abs(screenPos.x - 0.5f);
                float distY = Mathf.Abs(screenPos.y - 0.5f);


                if (dist < closest && distY <=0.5f)
                {
                    closest = dist;
                    Quaternion objective = Quaternion.LookRotation(t.transform.position - transform.position, Vector3.up);
                    angleX = (objective.eulerAngles.y - transform.rotation.eulerAngles.y);
                    angleY = -objective.eulerAngles.x + transform.rotation.eulerAngles.x;
                }
            }*/

            if (t.isVisible)
            {
                Quaternion objective = Quaternion.LookRotation(t.transform.position - transform.position, Vector3.up);
                angleX = (objective.eulerAngles.y - transform.rotation.eulerAngles.y);
                angleY = -objective.eulerAngles.x + transform.rotation.eulerAngles.x;

                Vector3 screenPos = Camera.main.WorldToViewportPoint(t.transform.position);

                closest = Mathf.Abs(screenPos.x - 0.5f);
            }
        }
        

        if (closest < 1f) //0.51f
        {
            time = (Mathf.Pow(4f, closest) - 0.8f)/2f;

            idle = false;

            cheatRewardsClicked = false;
            last = 0f;
            done = 0f;
            currentX = 0f;
            currentY = 0f;
            interpX = 0f;
            interpY = 0f;

            nextTimeClic = timeClic + Random.Range(-clicVariance, clicVariance);
        }
        else if (!idle)
        {
            // idle
            idle = true;

            if (invertIdle)
            {
                angleX = Random.Range(-50f, -20f);
            }
            else
            {
                angleX = Random.Range(20f, 50f);
            }

            angleY = Random.Range(-5f, 5f) + transform.rotation.eulerAngles.x;

            time = Mathf.Abs(angleX)/ 90f + Random.Range(-0.1f, 0.1f);

            cheatRewardsClicked = false;
            last = 0f;
            done = 0f;
            currentX = 0f;
            currentY = 0f;
            interpX = 0f;
            interpY = 0f;
        }

        if (angleX > 180f) angleX -= 360f;
        if (angleX < -180f) angleX += 360f;
        if (angleY > 180f) angleY -= 360f;
        if (angleY < -180f) angleY += 360f;

        if (closest < 1f)
        {
            angleX *= Random.Range(1f, imprecision);
        }

        //sacar perform aqui
        perform = true;
    }

    private float OutLerp(float a, float b, float t)
    {
        return a * (1 - t) + b * t;
    }

    public float GetCheatRewards(float tolerableRange, float punMultiplier, float rewMultiplier)
    {
        if (done >= nextTimeClic && last < nextTimeClic && !idle)
        {
            if (cheatRewardsClicked) return 0f;
            cheatRewardsClicked = true;
            return rewMultiplier;
        }
        else
        {
            if (idle) return -punMultiplier * 2f;

            float dist = Mathf.Abs(timeClic - done);

            if (dist <= clicVariance)
            {
                if (cheatRewardsClicked) return 0f;
                cheatRewardsClicked = true;
                return rewMultiplier * (clicVariance - dist) / clicVariance;
            }
            else return Mathf.Max(punMultiplier * (clicVariance - dist) / clicVariance, -punMultiplier * 2f);
        }
        
    }

    public void GetCheatTRShapedValues(ref float angleRange, ref float punFactor, ref float rewFactor)
    {
        if (idle)
        {
            //Debug.Log(done);
            angleRange = shapedTRCurves[0].Evaluate(done);
            punFactor = shapedTRCurves[1].Evaluate(done);
            rewFactor = shapedTRCurves[2].Evaluate(done);
        }
        else
        {
            angleRange = shapedTRCurves[3].Evaluate(done);
            punFactor = shapedTRCurves[4].Evaluate(done);
            rewFactor = shapedTRCurves[5].Evaluate(done);
        }
    }
}
