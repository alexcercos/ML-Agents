using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotTest : IBotMovement
{
    private float timerY;
    private float timerX;
    private float timerClick;

    private bool click;

    public override bool Click()
    {
        if (click) Debug.Log("clica");
        return click;
    }

    public override float MouseX()
    {
        return Mathf.Cos(timerX*0.1f) * 0.5f;
    }

    public override float MouseY()
    {
        return Mathf.Cos(timerY) * 0.01f;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        timerX = 0f;
        timerY = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        timerY += Time.deltaTime * Random.Range(-0.2f, 1.5f);
        timerX += Time.deltaTime * Random.Range(-0.2f, 1.5f);

        timerClick += Time.deltaTime;
        if (timerClick >= 0.5f)
        {
            click = true;
            timerClick = 0f;
        } else
        {
            click = false;
        }
    }
}
