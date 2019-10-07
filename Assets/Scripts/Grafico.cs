using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grafico : MonoBehaviour
{
    public Text textoDoTitulo;
    public Text XDoTitulo;
    public Text YDoTitulo;
    public Text textoEscala;
    public RectTransform Graph_Container;

    public Vector2 MinMin;
    public Vector2 MaxMax;

    public float lineWidth = 10f;
    public Color lineColor; 

    public Sprite circleSprite;
    public Sprite lineSprite;

    private int Height,
        Width;

    private GameObject linePrefab;


    private void Awake()
    {
        //Height = (int)(MaxMax.y - MinMin.y);
        //Width  = (int)(MaxMax.x - MinMin.x);
        Height = (int)Graph_Container.rect.height;
        Width = (int)Graph_Container.rect.width;
        linePrefab = Resources.Load("Image") as GameObject;
    }

    private void ClearGraph()
    {
        foreach(Transform child in Graph_Container.GetComponentInChildren<Transform>())
        {
            Destroy(child.gameObject);
        }
    }

    public void SetGraph(string title, List<float> valores, int escala)
    {
        //determina o maior número e se deve arredondar
        int max = Arredondar(FindMax(valores));
        SetGraph(title, valores, escala, max);
    }

    public void SetGraph(string title, List<float> valores, int escala, int max)
    {
        ClearGraph();
        //Coloca o título no lugar
        textoDoTitulo.text = title;
        

        //coloca no gráfico a legenda numérica
        YDoTitulo.text = max.ToString();
        XDoTitulo.text = valores.Count.ToString();
        //coloca a legenda da escala
        textoEscala.text = GetTimeScale(escala);

        float divider = valores.Count - 1 <= 0 ? 1 : valores.Count-1;

        float columnGap = Width / divider ;
        float rowGap = Height / max;

        //Coloca os traços para cada número
        Vector2 lastCoord = Vector2.zero;
        for (int i = 0; i < valores.Count; i++)
        {
            //desenha traço indo de x1,y1 a x2,y2
            float x = columnGap * i;
            float y = rowGap * valores[i];

            Vector2 finalPosition = new Vector2(x, y);

            CreateCircle(finalPosition);

            //se não é o primeiro, desenhar traços
            if (i != 0)
            {
                CreateUiLine(finalPosition, lastCoord);
                //Vector3 dir = (lastCoord - new Vector2(x,y)).normalized;
                //CreateLine(new Vector2((lastCoord.x + x) / 2, (lastCoord.y + y) / 2), GetAngleFromVectorFloat(dir));
            }
            lastCoord = new Vector2(x, y);

        }
    }


    private float FindMax(List<float> list)
    {
        if (list.Count == 0)
        {
            Debug.LogError("EmptyList");
        }
        float maxAge = list[0];
        foreach (int type in list)
        {
            if (type > maxAge)
            {
                maxAge = type;
            }
        }
        return maxAge;
    }

    private int Arredondar(float num)
    {
        //detecta decimais
        int numero = (int)num;
        
        while ((numero / 10) * 10 != numero)
        {
            numero++;
        }
        return numero;
    }

    private float RegraDeTres(float x, float max, float newMax)
    {
        return x * newMax / max;
    }

    private void CreateCircle(Vector2 anchorPosition)
    {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(Graph_Container, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchorPosition;
        rectTransform.sizeDelta = new Vector2(5, 5);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
    }

    private void CreateLine(Vector2 anchorPosition, float rotation = 0)
    {
        GameObject gameObject = new GameObject("line", typeof(Image));
        gameObject.GetComponent<Image>().color = lineColor;
        gameObject.transform.SetParent(Graph_Container, false);
        gameObject.GetComponent<Image>().sprite = lineSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchorPosition;
        rectTransform.sizeDelta = new Vector2(110 , lineWidth);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, rotation));
    }

    private void CreateUiLine(Vector2 start, Vector2 end, float thickness = 5f)
    {
        GameObject gameObject = new GameObject("line", typeof(Image));
        gameObject.GetComponent<Image>().color = lineColor;
        gameObject.transform.SetParent(Graph_Container, false);
        gameObject.GetComponent<Image>().sprite = lineSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2((start.x + end.x) / 2, (start.y + end.y) / 2);
        rectTransform.sizeDelta = new Vector2(Vector2.Distance(start, end), lineWidth);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

        Vector3 dir = (end - start).normalized;
        rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, GetAngleFromVectorFloat(dir)));
    }

    private float GetAngle(Vector2 A, Vector2 B)
    {
        float angulo = ((B.y - A.y) / (B.x - A.x));
        Debug.Log(angulo);
        return angulo * 49f;
    }
    public static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }
    private string GetTimeScale(int medida)
    {
        if(medida> 60)
        {
            medida /= 60;
            return "(" + medida + "min)";
        }
        return "(" + medida + "s)";
    }


}
