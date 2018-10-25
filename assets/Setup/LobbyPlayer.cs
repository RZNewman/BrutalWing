using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyPlayer : NetworkBehaviour {
    public Image i;
    Text t;
    [SyncVar]
    string disp;
    [SyncVar]
    public bool readyB=false;
	// Use this for initialization
	void Start () {
        //i = GetComponentInChildren<Image>();
        t = GetComponentInChildren<Text>();
        
	}
    private void Update()
    {
        i.enabled = readyB;
        t.text = disp;
    }
    [Server]
    public void ready(bool r)
    {
        readyB = r;
    }
    public void player(int id)
    {
        disp = id.ToString();
    }
    [ClientRpc]
    public void RpcParent()
    {
        //Debug.Log("rpcLob");
        transform.parent = GameObject.FindGameObjectWithTag("Lobby").transform;
        
    }
}
