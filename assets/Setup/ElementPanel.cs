using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementPanel : MonoBehaviour {
    List<GameObject> abils;
    bool selected = false;
    GameObject select1, select2;
    AbilPanel panel;
	// Use this for initialization
	void Start () {
        abils = new List<GameObject>();
		foreach(Transform child in transform)
        {
            Ability ab = child.GetComponent<Ability>();
            ab.setElement(this);
            abils.Add(child.gameObject);
        }
	}
	public void setPanel(AbilPanel p)
    {
        panel = p;
    }
	// Update is called once per frame
    public void clicked(GameObject a)
    {
        if (!selected)
        {
            selected = true;
            panel.select(this);

            select1 = a;
            select1.GetComponent<Ability>().renderCD(1);

            if(select1 == transform.GetChild(0).gameObject)
            {
                select2 = transform.GetChild(1).gameObject;
            }
            else
            {
                select2 = transform.GetChild(0).gameObject;
            }
            select2.GetComponent<Ability>().renderCD(2);
        }
        else if(a == select1)
        {
            //nothing
        }
        else if(a == select2)
        {
            select2 = select1;
            select1 = a;
            select1.GetComponent<Ability>().renderCD(1);
            select2.GetComponent<Ability>().renderCD(2);
        }
        else
        {
            select2.GetComponent<Ability>().renderCD(0);
            select2 = select1;
            select2.GetComponent<Ability>().renderCD(2);
            select1 = a;
            select1.GetComponent<Ability>().renderCD(1);
        }
    }
    public void deselect()
    {
        selected = false;
        select1.GetComponent<Ability>().renderCD(0);
        select2.GetComponent<Ability>().renderCD(0);
    }
    public GameObject[] getSelected()
    {
        GameObject[] sel = new GameObject[2];
        sel[0] = select1;
        sel[1] = select2;
        return sel;
    }
}
