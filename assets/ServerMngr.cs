using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerMngr : NetworkBehaviour
{

    GameObject[] spawns;
    GameMngr gm;

    int spawnNum = 0;
    Dictionary<int, int> score;
    // Use this for initialization
    void Start()
    {
        spawns = GameObject.FindGameObjectsWithTag("Respawn");
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameMngr>();
        score = new Dictionary<int, int>();
        Buffable.baseline();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void addScore(int id)
    {


        score[id] += 1;


        gm.display(export());
        
        
    }
    public void register(int id)
    {
        if (!score.ContainsKey(id))
        {
            score.Add(id, 0);
        }
    }
    int[] export()
    {
        int[] s = new int[score.Count];
        score.Values.CopyTo(s, 0);
        return s;
    }
    public NetworkInstanceId assignSpawn()
    {
        GameObject spawn = spawns[spawnNum];
        //print(spawn);
        spawnNum = (spawnNum + 1) % (spawns.Length);
        NetworkInstanceId sid = spawn.GetComponent<NetworkIdentity>().netId;
        //print(sid);
        //for (uint i = 0; i < 10; i++)
        //{
        //    print(i + "  sev  " + NetworkServer.FindLocalObject(new NetworkInstanceId(i)));
        //}
        return sid;

    }
}
