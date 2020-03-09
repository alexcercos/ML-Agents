using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class PushAcademy : Academy
{
    public override void InitializeAcademy()
    {
        Physics.gravity *= 5;
    }
}
