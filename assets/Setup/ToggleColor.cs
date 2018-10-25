using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleColor : MonoBehaviour {
    Image i;
    bool on = false;
	// Use this for initialization
	void Start () {
        i = GetComponent<Image>();
	}
	
	// Update is called once per frame
	public void tog()
    {
        if (on)
        {
            i.color = Color.white;
        }
        else
        {
            i.color = Color.green;
        }
        on = !on;
    }
}
