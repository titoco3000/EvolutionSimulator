using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderDuo : Button
{
    private RectTransform rectTransform;
    protected override void Start()
    {
        base.Start();
        rectTransform = GetComponent<RectTransform>();
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        Vector3 rot = rectTransform.rotation.eulerAngles;
        rot.z += 180;
        rectTransform.rotation = Quaternion.Euler(rot);
    }
}
