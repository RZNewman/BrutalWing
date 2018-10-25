using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerGhost : NetworkBehaviour {
    CamMove cam;
    AbilSelector abilities;
    public GameObject playerPre;
    public GameObject CamPre;
    GameMngr gm;
    ServerMngr sv;
    int myConId;

    [SyncVar]
    int team;

    GameObject spawn;
    PhysicalInput inp;

    //void OnEnable()
    //{
    //    SceneManager.sceneLoaded += OnSceneLoaded;
    //}

    //// called second

    public void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    // Use this for initialization
    public override void OnStartLocalPlayer () {
        //Debug.Log("start");
        //if (isLocalPlayer)
        //{
        inp = GetComponent<PhysicalInput>();
        CmdServerInit();

        //}
        
        Button b = GameObject.FindGameObjectWithTag("Ready").GetComponent<Button>();
        b.GetComponent<ReadyState>().ghosted = true;
        b.onClick.AddListener(readyUp);
        
	}
    bool ready = false;
    [Client]
    public void readyUp()
    {
        ready = !ready;
        CmdReady(ready);
    }
    [Command]
    public void CmdReady(bool r)
    {
        sv.ready(myConId, r);
    }
    [ClientRpc]
    public void RpcInitilize()
    {
        //Debug.Log("rpc");
        if (isLocalPlayer)
        {
            gameInit();
        }
        
    }
    public void gameInit()
    {
        //Debug.Log("init");
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameMngr>();
        //CmdAssignConnection();
        //CmdUpdateScore(netId);
        CmdGetSpawn();
        GameObject ab = GameObject.FindGameObjectWithTag("Abil");
        if (ab)
        {
            abilities = ab.GetComponent<AbilSelector>();
            gm.popAs(abilities);
        }
    }
    public Vector3 getGroundTarget()
    {
        return inp.groundTarget;
    }
    [Command]
    void CmdServerInit()
    {
        sv = GameObject.FindGameObjectWithTag("Server").GetComponent<ServerMngr>();
        myConId = connectionToClient.connectionId;

        sv.register(myConId, this);
    }
    [Command]
    void CmdGetSpawn()
    {
        #region Assign connections
        
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameMngr>();
        
        #endregion


        //NetworkInstanceId spawnID = sv.assignSpawn();
        ServerMngr.spawnInfo spnI = sv.getSpawn(myConId);
        NetworkConnection nc = connectionToClient;
        
        spawn = NetworkServer.FindLocalObject(spnI.sid);
        team = spnI.team;

        gm.assignConnection(connectionToClient, netId, team);
        //for (uint i = 0; i < 10; i++)
        //{
        //    print(i + "  cmd  " + NetworkServer.FindLocalObject(new NetworkInstanceId(i)));
        //}
        TargetSetSpawn(nc, spnI.sid);
        

    }
    [TargetRpc]
    void TargetSetSpawn(NetworkConnection nc, NetworkInstanceId sId)
    {
        //for(uint i =0; i<10; i++)
        //{
        //    print(i + "  rpc  " + ClientScene.FindLocalObject(new NetworkInstanceId(i)));
        //}
        spawn = ClientScene.FindLocalObject(sId);
        //print(spawn);
        //print(sId);
        gm.clientFace = spawn.transform.forward;
        GameObject c = Instantiate(CamPre);
        cam = c.GetComponent<CamMove>();
        inp.setCamera(c.GetComponent<Camera>());
        gm.setCam(c.GetComponent<Camera>());
        cam.setup(spawn);
        

    }

    // Update is called once per frame
    bool spawned = false;
	void Update () {
        if (!isLocalPlayer)
            return;
        inp.Poll();
		if(inp.jump&& !spawned)
        {
            //Debug.Log(spawn);
            if (spawn)
            {
                spawned = true;
                if (abilities)
                {
                    CmdSpawn(abilities.abilPre1, abilities.abilPre2);
                }
                else
                {
                    CmdSpawn(null, null);
                }
                
            }
            

        }
       
        //CmdSync(inp.export());
        
	}
    [Command]
    void CmdSpawn(string a1, string a2)
    {
        
        GameObject p = Instantiate(playerPre, spawn.transform.position, Quaternion.Euler(0, 0, 0));

        //GameObject p = Instantiate(playerPre, new Vector3(0,3), Quaternion.Euler(0, 0, 0));

        NetworkConnection nc = connectionToClient;

        NetworkServer.SpawnWithClientAuthority(p,nc);

        //p.GetComponent<NetworkIdentity>().AssignClientAuthority(nc);

        PlayerMover pm = p.GetComponent<PlayerMover>();
        //pm.inp = inp;
        pm.ghost = this;
        pm.team = team;
        if (a1!=null)
        {
            GameObject a1Pre = Resources.Load("attacks/" + a1) as GameObject;
            GameObject a2Pre = Resources.Load("attacks/" + a2) as GameObject;
            pm.abilPre1 = a1Pre;
            pm.abilPre2 = a2Pre;
        }
        

        TargetSpawn(nc,p.GetComponent<NetworkIdentity>().netId );



    }

    [TargetRpc]
    void TargetSpawn(NetworkConnection nc, NetworkInstanceId pId)
    {
        //if (!isLocalPlayer)
        //    return;
        GameObject p = ClientScene.FindLocalObject(pId);
        PlayerMover pm = p.GetComponent<PlayerMover>();
        pm.inp = inp;
        pm.ghost = this;
        spawned = true;
        cam.target = p;

    }
    //[Command]
    //void CmdSync(InputHandler.data d)
    //{
    //    inp.sync(d);
    //}
    public Vector3 spawnMutate(Vector3 move)
    {
        return move.x * spawn.transform.right + move.y * Vector3.up + move.z * spawn.transform.forward;
    }
    public void renderCD(float a1CD, float a2CD)
    {
        if (abilities)
        {
            gm.renderCD(a1CD, a2CD);
        }
        
    }
    public void charDied(int team)
    {
        spawned = false;
        if (isLocalPlayer)
        {
            CmdUpdateScore(team);
        }
    }
    [Command]
    void CmdUpdateScore(int team)
    {
        if(team==-1)
        {
            //lastHit = -1;
            //do nothing
        }
        else 
        {

            gm.addScore(team);

        }
   
        
    }

    void OnDestroy()
    {
        if (isLocalPlayer)
        {
            if (cam)
            {
                Destroy(cam.gameObject);
            }
            
        }
        
    }

}
