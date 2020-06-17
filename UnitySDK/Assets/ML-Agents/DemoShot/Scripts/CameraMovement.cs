using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public static float maxSpeed = 60f;

    public static float width, height;

    public static float camAngleHor = 90f;
    public static float camAngleVer = 60f;

    public IBotMovement botMovement;
    AgentShoot nnAgent;
    //private bool playWithBot = true;
    public bool useNeuralNet = false;

    private float x = 0f, y = 0f;
    bool click = false;

    //Vector3 lastPosition;
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(transform.rotation.eulerAngles);
        nnAgent = GetComponent<AgentShoot>();

        //width - 90 g
        //1000 - x

        //x = 90 * pixels / w

        //width = Screen.width;
        //height = Screen.height;

        width = 1024; //por si cambia la resolucion
        height = 576;

        //lastPosition = Input.mousePosition;

        //Cursor.lockState = CursorLockMode.Locked;

        // Si esta compilado dabe usar el bot
        /*
        #if UNITY_STANDALONE
            useNeuralNet = false;
        #endif*/
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playWithBot = !playWithBot;
        }*/

        if(Input.GetKeyDown(KeyCode.N))
        {
            useNeuralNet = !useNeuralNet;
        }

        if (useNeuralNet)
        {
            x = Mathf.Clamp(nnAgent.GetX() / 2f, -1f, 1f); //clamp hasta un maximo posible
            y = Mathf.Clamp(nnAgent.GetY() / 2f, -1f, 1f);

            //y = 0f;

            click = nnAgent.GetClick();
        }
        else //if (playWithBot)
        {
            x = Mathf.Clamp(botMovement.MouseX() / 2f, -1f, 1f); //clamp hasta un maximo posible
            y = Mathf.Clamp(botMovement.MouseY() / 2f, -1f, 1f);

            click = botMovement.Click();
        }
        /*
        else //esta parte esta repetida en heuristic
        {
            x = Mathf.Clamp(Input.GetAxis("Mouse X") / 2f, -1f, 1f); //clamp hasta un maximo posible
            y = Mathf.Clamp(Input.GetAxis("Mouse Y") / 2f, -1f, 1f);

            if (Input.GetMouseButtonDown(0))
            {
                click = true;
            }
            else
            {
                click = false;
            }
        }*/
        //Vector3 diff = lastPosition - Input.mousePosition;

        //lastPosition = Input.mousePosition;



        float rotY = camAngleHor * x * maxSpeed / width;
        float rotX = -camAngleVer * y * maxSpeed / height;

        //Debug.Log(x);
        //Debug.Log(rotY);
        //Debug.Log(Input.GetAxis("Mouse X"));

        //transform.Rotate(new Vector3(rotX, rotY, 0f));

        //Debug.Log(transform.rotation.eulerAngles);

        float angleY = (transform.parent.rotation.eulerAngles.y + rotY) % 360f;
        float angleX = (transform.rotation.eulerAngles.x + rotX) % 360f;

        if (angleX > 90f && angleX < 270f) angleX = angleX < 180f ? 90f : 270f; //LIMITAR A 90 porque falla ahora
                                                                                //posible solucion: girar en eje X en objeto padre y en Y en objeto hijo por separado (como una articulacion de 2 ejes)


        transform.parent.rotation = Quaternion.Euler(0f, angleY, 0f);

        transform.localRotation = Quaternion.Euler(angleX, 0f, 0f);

        //transform.rotation = Quaternion.Euler((Mathf.Clamp((transform.rotation.eulerAngles.x + rotX + 90f)%360, 0f, 180f) - 90f)%360f, (transform.rotation.eulerAngles.y + rotY)% 360f, 0f);

        if (click)
        {
            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(new Vector3(width/2f, height/2f));

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;

                Destroy(hit.transform.parent.gameObject);
                Spawn.currentPlanes--;
            }
        }
    }

    public float GetX()
    {
        return x;
    }

    public float GetY()
    {
        return y;
    }

    public bool GetClick()
    {
        return click;
    }

    public float GetBotX()
    {
        return Mathf.Clamp(botMovement.MouseX() / 2f, -1f, 1f);
    }
}
