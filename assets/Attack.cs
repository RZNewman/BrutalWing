using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Attack : NetworkBehaviour {
    public float duration = 1;
    public float force = 400;

    [SyncVar]
    NetworkInstanceId owner;
    float birth;
    List<GameObject> hit = new List<GameObject>();
	// Use this for initialization
	void Start () {
        birth = Time.time;
	}
	
    [Server]
    public void setOwner(NetworkInstanceId own)
    {
        owner = own;
    }
    public override void OnStartClient()
    {
        GameObject par = ClientScene.FindLocalObject(owner);
        transform.parent = par.transform;
        transform.position = par.transform.position;
    }
	// Update is called once per frame
	void Update () {
        if (isServer && Time.time - birth > duration)
        {
            //Debug.Log(isServer);
            NetworkServer.FindLocalObject(owner).GetComponent<PlayerMover>().endAttack();
            
        }
	}
    void OnTriggerEnter(Collider other)
    {
        if(isServer &&other.gameObject!= NetworkServer.FindLocalObject(owner) && !hit.Contains(other.gameObject))
        {
            hit.Add(other.gameObject);
            other.GetComponent<PlayerMover>().getHit(force, transform.position);
        }
    }
}
