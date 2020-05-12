using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTController : MonoBehaviour
{
    public RenderTexture current;
    public List<RenderTexture> previousFrames;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < previousFrames.Count; i++)
        {
            if (previousFrames[i]==null)
            {
                previousFrames[i] = new RenderTexture(16, 16, current.depth);
                Graphics.Blit(current, previousFrames[i]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        for (int i = previousFrames.Count-1; i > 0; i--)
        {
            Graphics.Blit(previousFrames[i-1], previousFrames[i]);
        }

        Graphics.Blit(current, previousFrames[0]);
    }
}
