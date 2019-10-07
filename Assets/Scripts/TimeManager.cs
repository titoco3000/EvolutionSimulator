using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public Light ColorRenderer;

    //acesso externo
    public bool Noite;

    //períodos(em segundos)
    private float DayLenght = 11f; 
    private float NightLenght = 5f;
    private float TransitionTime = 2f;
    //ciclo completo:
    // dia -> anoitecer -> noite -> amanhecer

    public Color dayColor;
    public Color nightColor;

    private float lastCicleStart = 0f;

    void Start()
    {
        Debug.Log("Startes");
    }

    // Update is called once per frame
    void Update()
    {
        float timeSinceLastCicle = Time.time - lastCicleStart;
        Debug.Log("horário: " + (int)timeSinceLastCicle);
        //Essa sequencia de verificações procura qm qual momento do dia está, de tras para frente
        //reiniciar o dia
        if (timeSinceLastCicle > DayLenght + NightLenght + (TransitionTime * 2))
        {
            Debug.Log("Starting day");
            lastCicleStart = Time.time;
            Noite = false;
        }
        //amanhecer
        else if (timeSinceLastCicle > DayLenght + TransitionTime + NightLenght)
        {
            float lerp = ScaleTo1(timeSinceLastCicle - (DayLenght + TransitionTime + NightLenght), TransitionTime * 30);
            ColorRenderer.color = Color.Lerp(ColorRenderer.color, dayColor, lerp);
        }
        //noite
        else if(timeSinceLastCicle > DayLenght + TransitionTime)
        {
            
        }
        //anoitecer
        else if(timeSinceLastCicle > DayLenght)
        {
            float lerp = ScaleTo1(timeSinceLastCicle - DayLenght , TransitionTime *30);
            Noite = true;
            ColorRenderer.color = Color.Lerp(ColorRenderer.color, nightColor,lerp);
        }
        
    }

    //faz uma regra de três para deixar entre 
    private float ScaleTo1(float num, float max)
    {
        return num / max;
    }

}
