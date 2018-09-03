using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffableLoose : MonoBehaviour {
    List<Buff> buffs;
    Dictionary<string, float> stats;
    static Dictionary<string, float> BASESTATS;
    public static void baseline(){
        BASESTATS = new Dictionary<string, float>();
        BASESTATS.Add("moveMult", 1);
    }
    void Start()
    {
        stats = new Dictionary<string, float>(BASESTATS);
    }
    public void computeStats()
    {
        stats = new Dictionary<string, float>(BASESTATS);
        foreach (Buff b in buffs)
        {
            addBuff(b);
        }

    }
    void addBuff(Buff b)
    { 
        foreach(string mod  in b.mods.Keys)
        {
            stats[mod]+=b.mods[mod];
        }
    }
    
}
