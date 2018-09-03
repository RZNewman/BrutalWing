using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Powerup : NetworkBehaviour {

    Spawner spawn;
    void OnTriggerEnter(Collider other)
    {
        if (isServer)
        {
            powerup(other.gameObject);
            spawn.consumed();
            Destroy(gameObject);
        }
        
    }
    public void setSpawn(Spawner s)
    {
        spawn = s;
    }
    protected abstract void powerup(GameObject player);

}
