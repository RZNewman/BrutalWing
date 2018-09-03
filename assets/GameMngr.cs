using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameMngr: NetworkBehaviour {

    [HideInInspector]
    public Vector3 clientFace = Vector3.zero;

    NetworkConnection clientC;
    NetworkInstanceId clientId;

    public Text scoreboard;
    public GameObject aPanel;
    public GameObject a1, a2;
    Canvas canv;
	// Use this for initialization
	void Start () {
        //scoreboard = GetComponentInChildren<Text>();
        canv = GetComponentInChildren<Canvas>();

    }

    public void display(int[] score)
    {
        
        RpcDisplayScore(score);
    }
    [ClientRpc]
    void RpcDisplayScore(int[] score)
    {

        string scoreS = "";
        string scorePre = "";
        for (int i = 0; i < score.Length; i++)
        {
            if (i == clientC.connectionId)
            {
                scorePre = "" + score[i];
            }
            else
            {
                scoreS += " - " + score[i];
            }

        }
        scoreS = scorePre + scoreS;
        scoreboard.text = scoreS;
    }

    [Server]
    public void assignConnection(NetworkConnection nc, NetworkInstanceId id)
    {
        //client = nc;
        TargetSetConnection(nc, id);
    }
    [TargetRpc]
    void TargetSetConnection(NetworkConnection nc, NetworkInstanceId id)
    {
        clientC = nc;
        clientId = id;
    }

    public void setCam(Camera c)
    {
        canv.worldCamera = c;
        canv.planeDistance = 2;
    }
    
    public void popAs(AbilSelector a)
    {
        a1 = Instantiate(a.abilI1, aPanel.transform);
        a1.transform.position -= Vector3.right*70;
        a2 = Instantiate(a.abilI2, aPanel.transform);
        a2.transform.position -= Vector3.left * 70;
    }

    public void renderCD(float a1CD, float a2CD)
    {
        a1.GetComponent<Ability>().renderCD(a1CD);
        a2.GetComponent<Ability>().renderCD(a2CD);
    }
    // Update is called once per frame
    

}
