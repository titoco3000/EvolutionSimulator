using UnityEngine;

public class TimeManager : MonoBehaviour
{

    public float[] Velocidades;
    private int current;
    public void SetTime(float t)
    {
        Time.timeScale = t;
    }

    public void OnButton()
    {
        current = (current >= Velocidades.Length - 1 ? 0 : current + 1);
        SetTime(Velocidades[current]);
    }



   

}
