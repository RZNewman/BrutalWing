using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Health : NetworkBehaviour {
    public int maxHealth = 4;
    public RectTransform slider;

    [SyncVar]
    int team;
    GameMngr gm;
    [SyncVar]
    int health;
	// Use this for initialization
	void Start () {
        health = maxHealth;
        
        
    }
	
    public bool change(int i)
    {
        health += i;
        if(health> maxHealth)
        {
            health = maxHealth;
        }
        RpcDisplay(health);
        //slider.localPosition = new Vector3(-(1 / maxHealth / 2) * (maxHealth - health), 0);
        //slider.position = new Vector3(-.25f, 0);
        return dead;
    }
    [ClientRpc]
    void RpcDisplay(int hp)
    {
        slider.localScale = new Vector3(((float)hp) / maxHealth, 1);
        
    }
    public bool dead
    {
        get
        {
            return health <= 0;
        }
    }
    [Server]
    public void setTeam(int t)
    {
        team = t;
        RpcTeamColor(t);
    }
    [ClientRpc]
    void RpcTeamColor(int t)
    {
        if (!gm)
        {
            gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameMngr>();
        }
        //Debug.Log(gm.clientTeam);
        //Debug.Log(t);
        if (gm.clientTeam == t)
        {
            slider.GetComponent<Image>().color = Color.blue;
        }
    }
}
