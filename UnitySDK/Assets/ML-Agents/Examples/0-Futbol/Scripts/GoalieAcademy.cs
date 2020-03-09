using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class GoalieAcademy : Academy
{
    public static float posicionX;
    public static float xPelota;
    public static float zApuntar;
    public static float velPelota;

    public override void AcademyReset()
    {
        posicionX = resetParameters["posicionx"];
        xPelota = resetParameters["xpelota"];
        zApuntar = resetParameters["zapuntar"];
        velPelota = resetParameters["velpelota"];
    }
}
