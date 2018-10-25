using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Hitbox : NetworkBehaviour {
    public float force = 400;
    public int damage = 1;

    [SyncVar]
    protected NetworkInstanceId owner;

    [Server]
    public void setOwner(NetworkInstanceId own)
    {
        owner = own;
    }

    List<GameObject> hit = new List<GameObject>();
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //void OnTriggerEnter(Collider other)
    //{
    //    if (isServer && other.gameObject != NetworkServer.FindLocalObject(owner) && !hit.Contains(other.gameObject))
    //    {
    //        //print(owner);
    //        hit.Add(other.gameObject);
    //        




    //    }
    //}
}
