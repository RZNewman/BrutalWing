using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyState : MonoBehaviour {
    public AbilSelector ab;
    public bool ghosted = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (ab.holding && ghosted)
        {
            GetComponent<Button>().interactable = true;
        }
	}
}
