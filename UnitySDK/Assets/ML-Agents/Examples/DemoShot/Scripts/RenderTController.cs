using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTController : MonoBehaviour
{
    public RenderTexture current;
    public List<RenderTexture> previousFrames;

    int startCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        startCount = 0;
        for (int i = 0; i < previousFrames.Count; i++)
        {
            if (previousFrames[i]==null)
            {
                previousFrames[i] = new RenderTexture(current.width, current.height, current.depth);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (startCount < 2) // Para reiniciarlos en el segundo frame
        {
            if (startCount == 1)
            {
                for (int i =  0; i < previousFrames.Count; i++)
                {
                    Graphics.Blit(current, previousFrames[i]);
                }
            }

            startCount++;
        }
    }

    private void LateUpdate()
    {
        
        for (int i = previousFrames.Count - 1; i > 0; i--)
        {
            Graphics.Blit(previousFrames[i - 1], previousFrames[i]);
        }

        Graphics.Blit(current, previousFrames[0]);
        
    }
}
