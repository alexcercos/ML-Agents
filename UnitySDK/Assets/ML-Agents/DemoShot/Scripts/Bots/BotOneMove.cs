using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotOneMove : IBotMovement
{
    Transform scene;

    AgentShoot agentShoot;

    public static float FPS = 60f;

    float xMove = 0f, yMove = 0f;

    float interpX = 0f, interpY = 0f; //De 0 a 1 (pero puede salir)
    float done = 0f; // para interpolar la funcion (de 0 a 1)
    float last = 0f;

    public float moveNoise = 0.0f;

    public List<AnimationCurve> curves;
    //public AnimationCurve shapeX, shapeY;

    public float angleX, angleY = 0f;
    float currentX, currentY = 0f; //angulo actual


    public float timeClic = 0.9f;

    public float time = 0f;
    public bool perform = false; //trigger (si no es auto)

    bool idle;

    bool doClic = false;

    bool cheatRewardsClicked = false;

    void Start()
    {
        scene = GameObject.Find("Scene").transform;

        agentShoot = GetComponent<AgentShoot>();
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
            SearchForObjective();
            //buscar nuevo target
            xMove = 0f; //reinicia en el siguiente frame
            yMove = 0f;
        }

    }

    public void PerformMove()
    {
        
        last = done;
        done += 1 / (FPS * time);

        if (doClic) doClic = false;
        if (done >= timeClic && last < timeClic && !idle)
        {
            doClic = true;
        }

        //interpX = done; //lineal ahora
        //interpY = done;

        if (idle)
        {
            interpX = curves[2].Evaluate(done); //shape X
            interpY = curves[2].Evaluate(done); //shape Y
        }
        else
        {
            interpX = curves[0].Evaluate(done); //shape X
            interpY = curves[1].Evaluate(done); //shape Y
        }

        float limitDecay = Mathf.Abs(done - 0.5f) - 0.5f; //en 0 y 1 no hay ruido para asegurar que acaba en el sitio
        float timeDecay = Mathf.Clamp((time-0.2f)*2f, 0f, 1f); //movimientos cortos sin ruido

        float newX = OutLerp(0f, angleX, interpX) + (Random.Range(-moveNoise, moveNoise) * limitDecay * timeDecay);
        float newY = OutLerp(0f, angleY, interpY) + (Random.Range(-moveNoise, moveNoise) * limitDecay * timeDecay); //nuevos angulos
        

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
            done = 0f;
            currentX = 0f;
            currentY = 0f;
        }
    }

    public override bool Click()
    {
        if (doClic)
            agentShoot.BotClickEvent();

        return doClic;
        if (doClic)
        {
            //Debug.Log("clicked at" + done);
            doClic = false;
            return true;
        }
        else
            return false;
    }

    public override float MouseX()
    {
        if (Mathf.Abs(xMove) > 1f) Debug.Log("X overflow: " + xMove);
        return xMove * 2f;
    }

    public override float MouseY()
    {
        if (Mathf.Abs(yMove) > 1f) Debug.Log("Y overflow: " + yMove);
        return yMove * 2f;
    }

    private void SearchForObjective()
    {
        //Cuando acaba un movimiento, busca objetivo

        //Considerar angulo en Y cuando es idle

        float closest = 0.51f;
        foreach (Renderer t in scene.GetComponentsInChildren<Renderer>())
        {
            Vector3 screenPos = Camera.main.WorldToViewportPoint(t.transform.position);
            if (screenPos.z > 0f)
            {
                float dist = Mathf.Abs(screenPos.x - 0.5f);
                if (dist < closest)
                {
                    closest = dist;
                    Quaternion objective = Quaternion.LookRotation(t.transform.position - transform.position, Vector3.up);
                    angleX = objective.eulerAngles.y - transform.rotation.eulerAngles.y;
                    angleY = -objective.eulerAngles.x + transform.rotation.eulerAngles.x;
                }
            }
        }
        

        if (closest < 0.51f)
        {
            time = (Mathf.Pow(4f, closest) - 0.8f)/2f;

            idle = false;
        }
        else
        {
            // idle
            idle = true;
            angleX = Random.Range(20f, 50f);

            angleY = Random.Range(-5f, 5f) + transform.rotation.eulerAngles.x;

            time = angleX / 90f + Random.Range(-0.1f, 0.1f);
        }

        if (angleX > 180f) angleX -= 360f;
        if (angleX < -180f) angleX += 360f;
        if (angleY > 180f) angleY -= 360f;
        if (angleY < -180f) angleY += 360f;

        //sacar perform aqui
        perform = true;
    }

    private float OutLerp(float a, float b, float t)
    {
        return a * (1 - t) + b * t;
    }

    public float GetCheatRewards(float tolerableRange, float punMultiplier, float rewMultiplier)
    {
        if (done >= timeClic && last < timeClic && !idle)
        {
            if (cheatRewardsClicked) return 0f;
            cheatRewardsClicked = true;
            return rewMultiplier;
        }
        else
        {
            if (idle) return -punMultiplier;

            float dist = Mathf.Abs(timeClic - done);

            if (dist <= tolerableRange)
            {
                if (cheatRewardsClicked) return 0f;
                cheatRewardsClicked = true;
                return rewMultiplier * dist / tolerableRange;
            }
            else return Mathf.Max(punMultiplier * (tolerableRange - dist) / tolerableRange, -punMultiplier * 3f);
        }
        
    }
}
