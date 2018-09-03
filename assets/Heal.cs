using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : Powerup {

	// Use this for initialization
	void Start () {
		
	}

    protected override void powerup(GameObject player)
    {
        player.GetComponent<Health>().change(2);
    }
}
