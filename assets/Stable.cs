using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stable : MonoBehaviour {

    Quaternion rot;
    GameMngr gm;
	// Use this for initialization
	void Awake () {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameMngr>();
    }
	
	// Update is called once per frame
	void Update () {
        //transform.localRotation = Quaternion.Euler(33, 0, 0);

        transform.rotation = Quaternion.LookRotation(gm.clientFace-Vector3.up*-2);
    }
}
