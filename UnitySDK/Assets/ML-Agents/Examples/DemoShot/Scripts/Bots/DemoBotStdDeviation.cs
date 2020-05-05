using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoBotStdDeviation : IBotMovement
{
    public IBotMovement botToImitate;

    public override bool Click()
    {
        return botToImitate.Click();
    }

    public override float MouseX()
    {
        return botToImitate.MouseX();
    }

    public override float MouseY()
    {
        return botToImitate.MouseY();
    }
}
