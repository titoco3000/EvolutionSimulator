using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GradientGraph : MonoBehaviour
{
    public Color[] listaDeCores;

    public void SetImage(Color[] cores)
    {
        listaDeCores = cores;
        GradientTotexture(GetComponent<Image>().sprite.texture, SetGradient(cores));

    }

    Gradient SetGradient(Color[] colors)
    {
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKey = new GradientColorKey[colors.Length]; 
        GradientAlphaKey[] alphaKey = new GradientAlphaKey[colors.Length];
        float escala = 1.00f / (colors.Length-1);
        for (int i = 0; i < colors.Length; i++)
        {
            float time = i * escala;
            colorKey[i].color = colors[i];
            colorKey[i].time = time;

            alphaKey[i].alpha = 1.0f;
            alphaKey[i].time = time;
        }
        gradient.SetKeys(colorKey, alphaKey);
        return gradient;
    }
    
    Texture2D GradientTotexture(Texture2D original, Gradient gradient)
    {
        int width = original.width;
        int height = original.height;
        //circula por cada uma das colunas da textura
        for (int i = 0; i < width; i++)
        {
            Color corDaColuna = gradient.Evaluate((float)i / (float)width);
            for (int k =0; k < height; k++)
            {
                original.SetPixel(i, k, corDaColuna);

            }
        }
        original.Apply();
        return original;
    }

}
