using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PresController : MonoBehaviour
{
    Transform scene;

    public DebugCanvas debugCanvas;

    public List<GameObject> cameraAxis;

    public List<string> botNames;

    public Text agentName;

    public int active = 0;

    // Start is called before the first frame update
    void Start()
    {
        scene = GameObject.Find("Scene").transform;
        //Deben estar activos los que deban

        active = active % cameraAxis.Count;

        ResetEnvironment(cameraAxis[active]);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            cameraAxis[active].SetActive(false);
            
            active = (active + 1) % cameraAxis.Count;
            
            //reiniciar cosas
            ResetEnvironment(cameraAxis[active]); //camera axis se activa dentro
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            cameraAxis[active].SetActive(false);

            active = (active - 1) % cameraAxis.Count;
            
            //reiniciar cosas
            ResetEnvironment(cameraAxis[active]); //camera axis se activa dentro
        }

    }


    void ResetEnvironment(GameObject cameraAxis)
    {
        debugCanvas.ResetAllLines();

        //graficas y escenario, puede que spawner

        float height = 0f;
        bool invert = false;

        cameraAxis.transform.GetChild(0).GetComponent<AgentShoot>().GetDirectionAndHeight(ref invert, ref height);

        scene.GetComponent<SpawnCases>().ResetSpawner(height, invert, cameraAxis.transform);

        scene.GetComponent<Spawn>().ResetSpawner(height);

        foreach (Transform child in scene)
        {
            Destroy(child.gameObject);
        }
        
        if (active < botNames.Count)
        {
            agentName.text = botNames[active];
        }

        cameraAxis.SetActive(true);
    }
}
