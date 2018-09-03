using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Spawner : NetworkBehaviour {
    public float cooldown;
    public GameObject pwrup;

    float cooldownC;
    bool spawned;
	// Use this for initialization
	void Start () {
        cooldownC = cooldown;
        spawned = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (isServer)
        {
            if (!spawned)
            {
                cooldownC -= Time.deltaTime;
                if (cooldownC <= 0)
                {
                    spawned = true;
                    GameObject pwr = Instantiate(pwrup, transform);
                    pwr.GetComponent<Powerup>().setSpawn(this);
                    NetworkServer.Spawn(pwr);
                }
            }
            
        }
	}
    public void consumed()
    {
        cooldownC = cooldown;
        spawned = false;
    }
}
