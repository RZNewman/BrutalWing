using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Spawnable : NetworkBehaviour {
    public float lifetime = 2;


    protected NetworkInstanceId player;
    protected GameObject owner;
    protected float birth;

    public void setPlayer(NetworkInstanceId p, NetworkInstanceId o)
    {
        player = p;
        owner = NetworkServer.FindLocalObject(o);
    }
    // Use this for initialization
    void Start()
    {
        birth = Time.time;
        Start2();
    }
    protected abstract void Start2();

    // Update is called once per frame
    void Update()
    {
        if (isServer && Time.time > birth + lifetime)
        {
            //print(Time.time + "  -   " + birth);
            
            Destroy(gameObject);
        }
    }
}
