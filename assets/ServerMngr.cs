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
    public GameObject spawnPre;
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

    bool teams = false;

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
        BuffableLoose.baseline();
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
        if (teams)
        {
            Dictionary<int, List<Player>> teamPlayers = new Dictionary<int, List<Player>>();
            foreach (Player p in players)
            {
                if (!teamPlayers.ContainsKey(p.team))
                {
                    teamPlayers.Add(p.team, new List<Player>() { p});
                }
                else
                {
                    teamPlayers[p.team].Add(p);
                }
            }
            List<GameObject> teamSpawns = new List<GameObject>();
            int spawnNum = 0;
            List<GameObject> newSpawns = new List<GameObject>();
            foreach (GameObject s in spawns)
            {
                teamSpawns.Add(s);
            }

            foreach(int team in teamPlayers.Keys)
            {
                int numPlayers = teamPlayers[team].Count;
                GameObject teamSpawn = teamSpawns[spawnNum];
                spawnNum = (spawnNum + 1) % (teamSpawns.Count);
                for (int i =0; i< numPlayers; i++)
                {
                    GameObject s =Instantiate(spawnPre, 
                        teamSpawn.transform.position+
                        teamSpawn.transform.forward* ((i+1)/2 *(Mathf.Pow(-1,i)))*2

                        , teamSpawn.transform.rotation);
                    newSpawns.Add(s);
                    NetworkServer.Spawn(s);
                    teamPlayers[team][i].spawnInd = newSpawns.Count - 1;
                }
            }
            spawns = newSpawns.ToArray();
            foreach(GameObject s in teamSpawns)
            {
                Destroy(s);
            }

            foreach (Player p in players)
            {
                p.ghost.RpcInitilize();
            }
        }
        else
        {
            int spawnNum = 0;
            foreach (Player p in players)
            {
                p.spawnInd = spawnNum;
                spawnNum = (spawnNum + 1) % (spawns.Length);
                p.ghost.RpcInitilize();
            }
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
            Player p = new Player(id, connections.Count, g);
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

    public void makeTeams()
    {
        teams = !teams;
        if (teams)
        {
            int t = 1;
            for (int i = 0; i < players.Count; i++)
            {
                players[i].team = t;
                //t += 1; //same team line
                t %= 3;//numTeams+1
                t = t == 0 ? 1 : t;
            }
        }
        else
        {
            for(int i = 0; i < players.Count; i++)
            {
                players[i].team = i + 1;
            }
        }
        foreach (Player p in players)
        {
            //Debug.Log(p.team);
        }
    }
}
