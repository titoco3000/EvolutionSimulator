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

    private void OnLevelWasLoaded(int level)
    {
        current = Find(Velocidades, Time.timeScale);
    }

    int Find(float[] array, float value)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == value)
                return i;
        }
        return -1;
    }





}
