using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Attack : NetworkBehaviour {
    public float cooldown = 0;
    public float duration = 1;

    
    public float windUp = 0.2f;
    

    

    [SyncVar]
    protected NetworkInstanceId owner;
    protected int ownerTeam;
    protected float birth;
    


    // Use this for initialization
    void Start () {
        birth = Time.time;
        
        
       
	}
    public override void OnStartClient()
    {
        GameObject par = ClientScene.FindLocalObject(owner);
        transform.parent = par.transform;
        transform.position = transform.parent.position;
        transform.rotation = transform.parent.rotation;
        visuals();
    }
    protected abstract void visuals();
    protected abstract void uptake(float cur);

    void Update()
    {
        if (isServer)
        {
            float cur = Time.time - birth;
            if (cur > duration)
            {
                //Debug.Log("death");

                NetworkServer.FindLocalObject(owner).GetComponent<PlayerMover>().RpcEndAttack();
            }
            else
            {
                //Debug.Log(cur);
                uptake(cur);
            }
        }
    }


    [Server]
    public void setOwner(NetworkInstanceId own,int team)
    {
        owner = own;
        ownerTeam = team;
    }

	// Update is called once per frame




}
