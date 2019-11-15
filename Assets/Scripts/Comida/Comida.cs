using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comida : MonoBehaviour
{
    public bool Comido = false;

    private float FadeRate = .04f;

    public int valorNutricional = 300;

    public virtual void Comer(Criatura cliente)
    {
        if (!Comido)
        {
            StartCoroutine(ReduceToZero(transform, FadeRate));
            Comido = true;  
        }
    }

    public virtual IEnumerator ReduceToZero(Transform target, float rate = 0.06f)
    {
        Vector3 escala = target.localScale;
        while (escala.x > 0)
        {
            escala -= Vector3.one * rate;
            target.localScale = escala;
            yield return new WaitForSeconds(.05f);
        }
        Destroy(target.gameObject);
    }
}
