using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MoreInfo : MonoBehaviour
{
    public string Texto;

    private GameObject panel;
    private void Start()
    {
        panel = transform.GetChild(0).gameObject;
        panel.GetComponentInChildren<Text>().text = Texto;
    }

    public void OnHoverEnter()
    {
        panel.SetActive(true);
    }
    public void OnHoverExit()
    {
        panel.SetActive(false);

    }
}
