using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comida : MonoBehaviour
{
    public Transform QuemEstaComendo = null;
    public int valorNutricional = 10;
    public bool Comido = false;

    public void Comer(Criatura cliente)
    {
        if (!Comido)
        {
            StartCoroutine(FadeAfterCoroutine(.06f));
            cliente.FoodAmount += valorNutricional;
            Comido = true;  
        }
    }

    private IEnumerator FadeAfterCoroutine(float t)
    {
        Renderer renderer = GetComponent<Renderer>();
        while (renderer.material.color.a > 0)
        {
            Color color = renderer.material.color;
            color.a -= t;

            renderer.material.color = color;
            yield return new WaitForSeconds(.05f);
        }
        Destroy(this.gameObject);
    }
}
