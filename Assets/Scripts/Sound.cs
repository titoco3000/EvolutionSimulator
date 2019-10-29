using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
public class Sound : MonoBehaviour
{
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool Beep(uint dwFreq, uint dwDuration);

    public int tempo;

    private void Update()
    {
    }
    

    int currentFreq;
    int directionGoing;
    void SomEntreNotas(int freqA, int freqB)
    {
        int samples = 110;
        float total = Mathf.Abs(freqA - freqB);
        float aumentoPorFrame = total / samples;
        int freqMaior = (freqA < freqB ? freqA : freqB);
        if (currentFreq <= 0)
            directionGoing = 1;
        else if (currentFreq >= samples)
            directionGoing = -1;

        currentFreq += directionGoing;
        int frequency = freqMaior+ (int)(currentFreq * aumentoPorFrame);
        Debug.Log(frequency);
        Beep((uint)frequency, 300);
    }

}
