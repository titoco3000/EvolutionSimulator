using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GraphInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool mouse_over = false;
    private RectTransform rectTransform;

    public RectTransform InfoObj;
    private Text InfoText;


    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        InfoText = InfoObj.GetComponentInChildren<Text>();
    }
    void Update()
    {
        if (mouse_over)
        {
            Transform dot = ClosestDot(Input.mousePosition);

            InfoObj.position = dot.position;
            InfoText.text = dot.GetComponent<DotValues>().Y.ToString();
        }

    }



    //retorna o ponto mais proximo
    Transform ClosestDot(Vector2 mouse)
    {
        DotValues[] allDots = GetComponentsInChildren<DotValues>();

        Transform ClosestDot = allDots[0].transform;
        float smallestDistance = Mathf.Abs(mouse.x-ClosestDot.transform.position.x);

        Transform closestRedDot = allDots[1].transform;

        for (int i = 1; i < allDots.Length; i++)
        {
            float distance = Mathf.Abs(mouse.x - allDots[i].transform.position.x);
            if(distance < smallestDistance)
            {
                smallestDistance = distance;
                ClosestDot = allDots[i].transform;
            }
            else if(distance == smallestDistance)
            {
                closestRedDot = allDots[i].transform;
            }


        }
        //decide entre closestRed e closest
        if(Mathf.Abs(mouse.y - ClosestDot.position.y) < Mathf.Abs(mouse.y - closestRedDot.position.y))
        {
            return ClosestDot;
        }
        return closestRedDot;

    }






    public void OnPointerEnter(PointerEventData eventData)
    {
        mouse_over = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouse_over = false;
        InfoObj.position = new Vector2(-100, 0);
    }
}
