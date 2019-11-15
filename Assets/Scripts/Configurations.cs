using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Configurations : MonoBehaviour
{
    public InputField quantidadeDeCriaturas;
    public InputField quantidadeDePredadores;
    public InputField mutationChance,
        mutationRate;
    public InputField valorNutricionalCriatura,
        valorNutricionalComida;

    public Toggle ReiniciarNaMorteDePredadores;
    public Toggle ConcienciaAmbiental;


    public Transform Gear1;
    public Transform Gear2;
    public Animator MenuAnimator;

    bool gearWasHovered;
    bool gearIsHovered;

    bool menuIsHovered;


    private void Start()
    {
        Time.timeScale = 1;
    }
    private void Update()
    {
        if(gearIsHovered && !gearWasHovered)
        {
            gearWasHovered = true;
            StartCoroutine(RotateGears(0, 180));
            MenuAnimator.SetBool("Revelar", true);
        }
        else if (!gearIsHovered && gearWasHovered && !menuIsHovered)
        {
            gearWasHovered = false;
            StartCoroutine(RotateGears(180,0));
            MenuAnimator.SetBool("Revelar", false);

        }
    }

    public void MouseEnterGear()
    {
        gearIsHovered = true;
    }
    public void MouseExitGear()
    {
        gearIsHovered = false;
    }

    public void MouseEnterMenu()
    {
        menuIsHovered = true;
    }
    public void MouseExitMenu()
    {
        menuIsHovered = false;
    }

    public void StartSimulation()
    {
        DataHolder holder = FindObjectOfType<DataHolder>();
        holder.numeroDeCriaturas = (int)GetValueFromInput(quantidadeDeCriaturas);
        holder.numeroDePredadores = (int)GetValueFromInput(quantidadeDePredadores);

        holder.mutationChance = GetValueFromInput(mutationChance);
        holder.mutationRate = GetValueFromInput(mutationRate);

        holder.valorNutricionalComida = (int)GetValueFromInput( valorNutricionalComida);
        holder.valorNutricionalCriaturas = (int)GetValueFromInput(valorNutricionalCriatura);

        holder.ReiniciarNaMorteDePredadores = ReiniciarNaMorteDePredadores.isOn;
        holder.ConcienciaAmbiental = ConcienciaAmbiental.isOn;
        FindObjectOfType<SceneSwapper>().WarpTo(1);
    
    }



    float progress = 1;
    IEnumerator RotateGears(float from, float to)
    {
        StopCoroutine(RotateGears(to, from));
        progress = 1 - progress;
        float speed = 2f;

        Quaternion originalRotation1 = Quaternion.Euler(0, 0,  from / Gear1.localScale.x);
        Quaternion originalRotation2 = Quaternion.Euler(0, 0,  -from / Gear2.localScale.x);
        Quaternion TargetRotation1 = Quaternion.Euler(0, 0,  to / Gear1.localScale.x);
        Quaternion TargetRotation2 = Quaternion.Euler(0, 0, -to / Gear2.localScale.x);


        while(progress < 1)
        {
            progress += speed * Time.deltaTime;
            Gear1.rotation = Quaternion.Lerp(originalRotation1, TargetRotation1, progress);
            Gear2.rotation = Quaternion.Lerp(originalRotation2, TargetRotation2, progress);
            yield return new WaitForEndOfFrame();
        }
    }

    private float GetValueFromInput(InputField field)
    {
        return float.Parse(field.text == "" ? field.placeholder.GetComponent<Text>().text : field.text);

    }
}
