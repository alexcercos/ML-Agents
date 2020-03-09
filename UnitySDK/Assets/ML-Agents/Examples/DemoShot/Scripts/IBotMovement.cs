using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IBotMovement : MonoBehaviour
{
    public abstract float MouseX(); //movimiento del mouse en X normalizado
    public abstract float MouseY(); //movimiento del mouse en Y normalizado
    public abstract bool Click();
}
