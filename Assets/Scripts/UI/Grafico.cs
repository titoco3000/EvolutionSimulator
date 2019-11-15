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
    public Text textoTempoTotal;
    public RectTransform Graph_Container;

    public GameObject GraficoNumerico;
    public GameObject GraficoColorido;

    public Vector2 MinMin;
    public Vector2 MaxMax;

    public float lineWidth = 10f;
    public Color lineColor; 

    public Sprite circleSprite;
    public Sprite lineSprite;

    private int Height,
        Width;

    private GameObject linePrefab;
    private GradientGraph gradient;


    private void Awake()
    {
        //Height = (int)(MaxMax.y - MinMin.y);
        //Width  = (int)(MaxMax.x - MinMin.x);
        Height = (int)Graph_Container.rect.height;
        Width = (int)Graph_Container.rect.width;
        linePrefab = Resources.Load("Image") as GameObject;
        gradient = GraficoColorido.GetComponent<GradientGraph>();

    }


    public void SetGraph(string title, List<float> valoresCriatura, List<float> valoresPredador, float escala)
    {
        //determina o maior número e se deve arredondar
        List<float> valores = new List<float>();
        valores.AddRange(valoresCriatura);
        valores.AddRange(valoresPredador);

        int max = Arredondar(FindMax(valores));
        SetGraph(title, valoresCriatura, valoresPredador, escala, max);
    }

    public void SetGraph(string title, List<float> valoresCriatura, List<float> valoresPredador, float escala, int max)
    {
        ClearGraph();
        //Coloca o título no lugar
        textoDoTitulo.text = title;
        

        //coloca no gráfico a legenda numérica
        YDoTitulo.text = max.ToString();
        XDoTitulo.text = valoresCriatura.Count.ToString();
        //coloca a legenda da escala
        textoEscala.text = "(" + GetTimeScale(escala) + ")";
        textoTempoTotal.text = GetTimeScale(valoresCriatura.Count * escala);

        float divider = valoresCriatura.Count - 1 <= 0 ? 1 : valoresCriatura.Count-1;

        float columnGap = Width / divider ;
        float rowGap = Height / max;

        //Coloca os traços para cada número
        Vector2 lastCoordCriatura = Vector2.zero;
        Vector2 lastCoordPredador = Vector2.zero;

        for (int i = 0; i < valoresCriatura.Count; i++)
        {
            //desenha traço indo de x1,y1 a x2,y2
            float x = columnGap * i;

            Vector2 positionCriatura = new Vector2(x, rowGap * valoresCriatura[i]);
            Vector2 positionPredador = new Vector2(x, rowGap * valoresPredador[i]);

            CreateCircle(positionCriatura, new Vector2(i, valoresCriatura[i]));
            CreateCircle(positionPredador, new Vector2(i, valoresPredador[i]));


            //se não é o primeiro, desenhar traços
            if (i != 0)
            {
                CreateUiLine(positionCriatura, lastCoordCriatura, Color.white);
                CreateUiLine(positionPredador, lastCoordPredador, Color.red);
            }
            lastCoordCriatura = positionCriatura;
            lastCoordPredador= positionPredador;

        }
    }

    public void SetGraph(string title, List<Color> coresCriatura, List<Color> coresPredador, float escala)
    {
        ClearGraph();
        //Coloca o título no lugar
        textoDoTitulo.text = title;

        //coloca a legenda da escala
        textoEscala.text = "(" + GetTimeScale(escala) + ")";
        textoTempoTotal.text = GetTimeScale(coresCriatura.Count * escala);


        //Primeiro resume a lista, para ter apenas 8 items
        coresCriatura = ResumeTo(coresCriatura, 8); 
        coresPredador = ResumeTo(coresPredador, 8);

        //depois coloca seu valor final
        gradient.SetImages(coresCriatura.ToArray(), coresPredador.ToArray());


    }

    public void DisplayGraph(string tipo)
    {
        if(tipo == "Colorido")
        {
            GraficoColorido.SetActive(true);
            GraficoNumerico.SetActive(false);
        }
        else
        {

            GraficoColorido.SetActive(false);
            GraficoNumerico.SetActive(true);
        }
    }
    private void ClearGraph()
    {
        foreach(Transform child in Graph_Container.GetComponentInChildren<Transform>())
        {
            Destroy(child.gameObject);
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

    private void CreateCircle(Vector2 anchorPosition, Vector2 rawValues)
    {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(Graph_Container, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchorPosition;
        rectTransform.sizeDelta = new Vector2(5, 5);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

        DotValues dotValues = gameObject.AddComponent<DotValues>();
        dotValues.X = rawValues.x;
        dotValues.Y = rawValues.y;
    }
    private void CreateUiLine(Vector2 start, Vector2 end, Color color, float thickness = 5f)
    {
        GameObject gameObject = new GameObject("line", typeof(Image));
        gameObject.GetComponent<Image>().color = color;
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
    
    //código do CodeMonkey
    public static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }
    private string GetTimeScale(float medida)
    {
        int segundos = (int)medida % 60;
        int minutosBruto = (int)medida / 60;
        int minutos = (int)minutosBruto % 60;
        int horasBruto = (int)minutosBruto / 60;
        int horas = (int)horasBruto % 24;
        int diasBruto = (int)horasBruto / 24;

        return
            (diasBruto != 0 ? diasBruto.ToString() + "dias" : "") +
            (horas != 0 ? horas.ToString() + "h" : "") +
            (minutos != 0 ? minutos.ToString() + "min" : "") +
            (segundos != 0 ? segundos.ToString() + "s" : "");

    }

    public List<Color> ResumeTo(List<Color> original, int novoTamanho)
    {
        if (original.Count <= novoTamanho)
            return original;

        //torna a lista par
        if(original.Count % 2 != 0)
        {
            original.RemoveAt(original.Count - 1);
        }
        while (original.Count > 8)
        {
            List<Color> listaSimplificada = new List<Color>();
            listaSimplificada.Add(original[0]);
            for (int i = 1; i + 1 < original.Count; i += 2)
            {
                listaSimplificada.Add((original[i] + original[i+1])/2);
            }
            original = listaSimplificada;

        }
        return original;
        

    }
}
