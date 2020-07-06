using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RenderTController : MonoBehaviour
{
    [Serializable]
    public class PreviousFrame
    {
        PreviousFrame()
        {
            position = 0;
            frame = null;
        }
        public int position;
        public RenderTexture frame;
    }

    //public Texture2D defaultTexture;

    public RenderTexture current;
    public List<RenderTexture> renderQueue;

    int initFrameCount = 0;

    //Se da por hecho que este ordenado
    public List<PreviousFrame> previousFrames;
    
    int maxIndex;

    public bool saveImg = false;


    // Start is called before the first frame update
    void Start()
    {
        renderQueue = new List<RenderTexture>();

        if (previousFrames.Count > 0)
            maxIndex = previousFrames[previousFrames.Count - 1].position;
    }

    private void LateUpdate()
    {
        if (saveImg) // Se ejecuta en play time al cambiar la variable un solo frame
        {
            SaveTexture(current);
            saveImg = false;
        }

        if (initFrameCount > 2) //frames necesarios para que empiece a guardar rts
        {
            RenderTexture newRT = new RenderTexture(current.width, current.height, current.depth);
            Graphics.Blit(current, newRT);

            renderQueue.Insert(0, newRT);
        }
        else
        {
            initFrameCount++;
        }
        

        //Actualizar texturas para el siguiente frame

        for (int i = 0; i < previousFrames.Count; i++)
        {
            if (previousFrames[i].position < renderQueue.Count)
            {
                Graphics.Blit(renderQueue[previousFrames[i].position], previousFrames[i].frame);
            }
            else if (renderQueue.Count > 0)
            {
                Graphics.Blit(renderQueue[renderQueue.Count-1], previousFrames[i].frame);
            }
            else
            {
                Graphics.Blit(current, previousFrames[i].frame);
            }
        }

        if (renderQueue.Count > maxIndex + 1)
        {
            Destroy(renderQueue[renderQueue.Count-1]);
            renderQueue.RemoveAt(renderQueue.Count - 1);

            //GC.Collect();
        }



        //Graphics.Blit(current, previousFrames[0]);

        //Debug.Log(renderQueue.Count);
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