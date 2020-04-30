using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class ShotAcademy : Academy
{
    public static float exigency;

    public override void AcademyReset()
    {
        exigency = resetParameters["exigency"];
    }
}
