using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyState : MonoBehaviour {
    AbilSelector ab;
    public bool ghosted = false;
	// Use this for initialization
	void Start () {
        //ab = GameObject.FindGameObjectWithTag("Abil").GetComponent<AbilSelector>();
        ab = AbilSelector.instance;
	}
	
	// Update is called once per frame
	void Update () {
        if (ab.holding && ghosted)
        {
            GetComponent<Button>().interactable = true;
        }
	}
}
