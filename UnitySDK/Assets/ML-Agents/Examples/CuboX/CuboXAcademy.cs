using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class CuboXAcademy : Academy
{
    public static float Separacion;

    public override void AcademyReset()
    {
        Separacion = resetParameters["separacion"];
    }
}
