using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresController : MonoBehaviour
{
    public List<GameObject> cameraAxis;

    int active = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Deben estar activos los que deban
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            cameraAxis[active].SetActive(false);
            
            active = (active + 1) % cameraAxis.Count;

            cameraAxis[active].SetActive(true);

            //reiniciar cosas
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            cameraAxis[active].SetActive(false);

            active = (active - 1) % cameraAxis.Count;

            cameraAxis[active].SetActive(true);

            //reiniciar cosas
        }

    }


    void ResetEnvironment()
    {
        //graficas y escenario, puede que spawner
    }
}
