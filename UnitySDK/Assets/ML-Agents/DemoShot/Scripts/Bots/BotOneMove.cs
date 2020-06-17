using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotOneMove : IBotMovement
{
    public static float FPS = 60f;

    float xMove = 0f, yMove = 0f;

    float interpX = 0f, interpY = 0f; //De 0 a 1 (pero puede salir)
    float done = 0f; // para interpolar la funcion (de 0 a 1)

    public List<AnimationCurve> curves;
    public AnimationCurve shapeX, shapeY;

    public float angleX, angleY = 0f;
    public float currentX, currentY = 0f; //angulo actual

    public float time = 0f;
    public bool perform = false;

    private void Update()
    {
        //movimientos solo valen en el rango -1, 1
        
        if (perform)
        {
            PerformMove();
        }
        else
        {
            //buscar nuevo target
            xMove = 0f; //reinicia en el siguiente frame
            yMove = 0f;
        }

    }

    public void PerformMove()
    {
        done += 1 / (FPS * time);

        //interpX = done; //lineal ahora
        //interpY = done;
        interpX = shapeX.Evaluate(done);
        interpY = shapeY.Evaluate(done);

        float newX = OutLerp(0f, angleX, interpX);
        float newY = OutLerp(0f, angleY, interpY); //nuevos angulos

        //xMove = (angleX * CameraMovement.width) / (time * FPS * CameraMovement.maxSpeed * CameraMovement.camAngleHor);
        //yMove = (angleY * CameraMovement.height) / (time * FPS * CameraMovement.maxSpeed * CameraMovement.camAngleVer);

        xMove = ((newX - currentX) * CameraMovement.width) / (CameraMovement.maxSpeed * CameraMovement.camAngleHor);
        yMove = ((newY - currentY) * CameraMovement.height) / (CameraMovement.maxSpeed * CameraMovement.camAngleVer);

        currentX = newX;
        currentY = newY;


        if (done >= 1f)
        {
            perform = false;
            done = 0f;
            currentX = 0f;
            currentY = 0f;
        }
    }

    public override bool Click()
    {
        return true;
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

    private float OutLerp(float a, float b, float t)
    {
        return a * (1 - t) + b * t;
    }
}
