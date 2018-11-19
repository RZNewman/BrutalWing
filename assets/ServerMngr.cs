using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ServerMngr : NetworkBehaviour
{
    public static ServerMngr instance;
    public bool ffa = true;
    public GameObject lobbyPre;
    GameObject[] spawns;
    GameMngr gm;
    class Player
    {
        public int conID;
        public int spawnInd;
        public int team;
        public PlayerGhost ghost;
        public bool ready;
        public LobbyPlayer lob;
        //public Player(int id, int spawn, int t)
        //{
        //    conID = id;
        //    spawnInd = spawn;
        //    team = t;

        //}
        public Player(int id, int t, PlayerGhost g)
        {
            conID = id;
            spawnInd = -1;
            team = t;
            ghost = g;
            ready = false;
            lob = null;

        }
    }
    List<int> connections;
    List<Player> players;
    int spawnNum = 0;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

    }
    void OnEnable()
    {

    }
    void debugPlay(Scene scene)
    {
        foreach (Player p in players)
        {
            Debug.Log(p.conID);
            Debug.Log(p.spawnInd);
            Debug.Log(p.team);
            Debug.Log(p.ghost);
        }
        Debug.Log("Unloaded    " +players.Count);
    }
    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log("Load 1");
        
        //Debug.Log("Load 2");
        if (scene.buildIndex != 2)
        {

            StartCoroutine(gameInit());
        }
        else
        {
            StartCoroutine(lobbyInit());
        }
    }
    // Use this for initialization
    void Start()
    {
        //gameInit();
        players = new List<Player>();
        connections = new List<int>();
        Buffable.baseline();
        //Debug.Log("enable");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    IEnumerator gameInit()
    {
        while(!players.TrueForAll(p => p.ghost.connectionToClient.isReady)){
            yield return null;
        }
        
        //foreach(Player p in players)
        //{ 
        //    NetworkServer.SetClientReady(p.ghost.connectionToClient);
        //}
        NetworkServer.SpawnObjects();
        spawns = GameObject.FindGameObjectsWithTag("Respawn");
        GameObject gmO = GameObject.FindGameObjectWithTag("GameController");
        gm = gmO.GetComponent<GameMngr>();
        foreach (Player p in players)
        {
            p.spawnInd = assignSpawn();
            p.ghost.RpcInitilize();
        }

    }
    IEnumerator lobbyInit()
    {
        while (!players.TrueForAll(p => p.ghost.connectionToClient.isReady))
        {
            yield return null;
        }
        foreach (Player p in players)
        {
            GameObject lobP = Instantiate(lobbyPre, GameObject.FindGameObjectWithTag("Lobby").transform);
            p.lob = lobP.GetComponent<LobbyPlayer>();
            p.lob.player(p.team);
            NetworkServer.Spawn(p.lob.gameObject);
            p.lob.RpcParent();
            p.ghost.SceneReset();


        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void register(int id, PlayerGhost g)
    {
        //Debug.Log("register");
        //foreach(Player p in players)
        //{
        //    Debug.Log(p.conID);
        //}
        if (!connections.Contains(id))
        {
            GameObject.FindGameObjectWithTag("Fight").GetComponent<Button>().interactable = false;
            connections.Add(id);
            Player p = new Player(id, connections.Count-1, g);
            GameObject lobP = Instantiate(lobbyPre, GameObject.FindGameObjectWithTag("Lobby").transform);

            p.lob = lobP.GetComponent<LobbyPlayer>();
            p.lob.player(p.team);
            players.Add(p);
            
        }
        foreach (Player p in players)
        {
            NetworkServer.Spawn(p.lob.gameObject);
            p.lob.RpcParent();
        }
    }
    public void ready(int id, bool r)
    {
        players.Find(p => p.conID == id).lob.ready(r);
        Button fight = GameObject.FindGameObjectWithTag("Fight").GetComponent<Button>();
        if (players.TrueForAll(p => p.lob.readyB))
        {
            fight.interactable = true;

        }
        else
        {
            fight.interactable = false;
        }
    }
  
    int assignSpawn()
    {

        //print(spawn);
        int sp = spawnNum;
        spawnNum = (spawnNum + 1) % (spawns.Length);
        return sp;

    }
    public struct spawnInfo
    {
        public NetworkInstanceId sid;
        public int team;
        public spawnInfo(NetworkInstanceId id, int t)
        {
            sid = id;
            team = t;
        }
    }
    public spawnInfo getSpawn(int connectionId)
    {
        Player p = players[connections.Find(id =>id==connectionId)];
        GameObject spawn = spawns[p.spawnInd];
        NetworkInstanceId sid = spawn.GetComponent<NetworkIdentity>().netId;
        //print(sid);
        //for (uint i = 0; i < 10; i++)
        //{
        //    print(i + "  sev  " + NetworkServer.FindLocalObject(new NetworkInstanceId(i)));
        //}
        spawnInfo si = new spawnInfo(sid, p.team);
        return si;
    }
}
