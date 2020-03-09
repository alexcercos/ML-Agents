using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class StrikerAcademy : Academy
{
    public static float posicionX;
    public static float xPelota;
    public static float minZ;
    public static float maxZ;
    public static float pelotaMinZ;
    public static float pelotaMaxZ;

    public override void AcademyReset()
    {
        posicionX = resetParameters["posicionx"];
        xPelota = resetParameters["xpelota"];
        minZ = resetParameters["minz"];
        maxZ = resetParameters["maxz"];
        pelotaMinZ = resetParameters["pelotaminz"];
        pelotaMaxZ = resetParameters["pelotamaxz"];
    }
    
}