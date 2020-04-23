using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class ShotAcademy : Academy
{
    public static float tolerableRange;
    public static float maximumRange;

    public override void AcademyReset()
    {
        tolerableRange = resetParameters["tolerableRange"];
        maximumRange = resetParameters["maximumRange"];
    }
}
