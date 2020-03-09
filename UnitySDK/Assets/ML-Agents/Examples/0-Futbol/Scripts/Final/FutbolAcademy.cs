using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class FutbolAcademy : Academy
{
    private void Start()
    {
        Physics.gravity = new Vector3(0f, -27f, 0f);
    }
    public override void AcademyReset()
    {
        
    }
}
