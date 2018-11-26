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
    [HideInInspector]
    public int clientTeam;

    //server only
    Dictionary<int, int> score;
    bool gameOver = false;
    int winner = -1;
    public Text scoreboard;
    public GameObject aPanel;
    public GameObject a1, a2;
    Canvas canv;
	// Use this for initialization
	void Start () {
        //scoreboard = GetComponentInChildren<Text>();
        canv = GetComponentInChildren<Canvas>();
        score = new Dictionary<int, int>();
        if (isServer)
        {
            //GameObject.FindGameObjectWithTag("Server").GetComponent<ServerMngr>().gameInit();
        }
    }
    IEnumerator endGame()
    {
        yield return new WaitForSeconds(5);
        GameObject.FindGameObjectWithTag("Net").GetComponent<NetworkManager>().ServerChangeScene("AbilSelect");
    }
    [Server]
    public void registerScore(int team)
    {
        if (!score.ContainsKey(team))
        {
            score.Add(team, 0);
        }
        
        display();
        
    }
    [Server]
    public void addScore(int team)
    {
        if (!gameOver)
        {
            score[team] += 1;
            if (score[team] >= 10)
            {
                gameOver = true;
                winner = team;
                StartCoroutine(endGame());
            }

            display();
        }
       


    }
    public void display()
    {
        if (!gameOver)
        {
            //Printable.printDicInt(score);
            string[] board = new string[score.Count+1];
            foreach (int team in score.Keys)
            {
                //Debug.Log(team);
                board[team] = "Team " + team + ": " + score[team];

            }
            RpcDisplayScore(board);
        }
        else{
            RpcDisplayOver("Team " + winner + " is the winner");
        }
        
    }
    [ClientRpc]
    void RpcDisplayScore(string[] scores)
    {
        //foreach(string s in scores)
        //{
        //    Debug.Log(s);
        //}
        string scoreS = "";
        string scorePre = "";
        for(int i =1; i<scores.Length; i++)
        {
            string txt = scores[i];
            if (i == clientTeam)
            {
                scorePre = txt;
            }
            else
            {
                scoreS += " - "+txt;
            }
        }
        //Debug.Log(scorePre);
        //Debug.Log(scoreS);
        scoreS = scorePre + scoreS;
        scoreboard.text = scoreS;
    }
    [ClientRpc]
    void RpcDisplayOver(string win)
    {
   
        scoreboard.text = win;
    }

    [Server]
    public void assignConnection(NetworkConnection nc, NetworkInstanceId id, int team)
    {
        //client = nc;
        registerScore(team);
        TargetSetConnection(nc, id, team);
    }
    [TargetRpc]
    void TargetSetConnection(NetworkConnection nc, NetworkInstanceId id, int team)
    {
        clientC = nc;
        clientId = id;
        clientTeam = team;
    }

    public void setCam(Camera c)
    {
        canv.worldCamera = c;
        canv.planeDistance = 2;
    }
    
    public void popAs(AbilSelector a)
    {
        //Debug.Log("instant");
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
