using UnityEngine;

public class RenderToImage : MonoBehaviour
{

    public Texture2D ImageToRenderInto;
    public Camera CameraToUse;
    public int skimpFactor;
    private int tries = 0;

    private void Update()
    {
        tries++;

        if (tries % skimpFactor == 0)
        {
            //        ImageToRenderInto = RTImage(CameraToUse);
            //    }
            //}

            Debug.Log("skimped");

            //// Take a "screenshot" of a camera's Render Texture.
            //Texture2D RTImage(Camera camera)
            //{
            // The Render Texture in RenderTexture.active is the one
            // that will be read by ReadPixels.
            var currentRT = RenderTexture.active;
            RenderTexture.active = CameraToUse.targetTexture;

            // Render the camera's view.
            CameraToUse.Render();

            // Make a new texture and read the active Render Texture into it.
            Texture2D image = new Texture2D(CameraToUse.targetTexture.width, CameraToUse.targetTexture.height);

            ImageToRenderInto.ReadPixels(new Rect(0, 0, CameraToUse.targetTexture.width, CameraToUse.targetTexture.height), 0, 0);
            ImageToRenderInto.Apply();

            // Replace the original active Render Texture.
            RenderTexture.active = currentRT;
            //   return image;
        }
    }
}