using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Ability : MonoBehaviour, IPointerClickHandler
{
    public string AttackPre;
    public GameObject self;
    ElementPanel ele;
    Image img;
    Text txt;
	// Use this for initialization
	void Start () {
        img = GetComponent<Image>();
        txt = GetComponentInChildren<Text>();
        renderCD(0);
	}
	public void renderCD(float cooldown)
    {
        if (cooldown <= 0)
        {
            txt.text = "";
            //txt.enabled = false;
            img.color = new Color(1,1,1);
        }
        else
        {
            //txt.enabled = true;
            txt.text = ((int)cooldown).ToString();
            img.color = new Color(1,1,1,0.3f);
        }
    }
    public void setElement(ElementPanel e)
    {
        ele = e;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (ele)
        {
            ele.clicked(gameObject);
        }
    }
}
