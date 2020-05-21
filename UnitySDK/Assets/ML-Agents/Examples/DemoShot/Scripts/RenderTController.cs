using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTController : MonoBehaviour
{
    public Texture2D defaultTexture;

    public RenderTexture current;
    public List<RenderTexture> previousFrames;

    public bool saveImg = false;

    // Start is called before the first frame update
    void Start()
    {

        for (int i = 0; i < previousFrames.Count; i++)
        {
            if (previousFrames[i]==null)
            {
                previousFrames[i] = new RenderTexture(current.width, current.height, current.depth);

                Graphics.Blit(defaultTexture, previousFrames[i]);
            }
        }

    }

    private void LateUpdate()
    {
        if (saveImg) // Se ejecuta en play time al cambiar la variable un solo frame
        {
            SaveTexture(current);
            saveImg = false;
        }

        for (int i = previousFrames.Count - 1; i > 0; i--)
        {
            Graphics.Blit(previousFrames[i - 1], previousFrames[i]);
        }

        Graphics.Blit(current, previousFrames[0]);
        
    }



    public void SaveTexture(RenderTexture rt)
    {
        byte[] bytes = toTexture2D(rt).EncodeToPNG();
        
        System.IO.File.WriteAllBytes(Application.dataPath + "/InitTexture.png", bytes);
    }

    Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }


}
