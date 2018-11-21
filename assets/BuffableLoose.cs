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
        BASESTATS.Add("FullSprint", 0);
    }
    void Start()
    {
        stats = new Dictionary<string, float>(BASESTATS);
        buffs = new List<Buff>();
    }
    public void computeStats()
    {
        stats = new Dictionary<string, float>(BASESTATS);
        foreach (Buff b in buffs)
        {
            computeBuff(b);
        }

    }
    void computeBuff(Buff b)
    { 
        foreach(string mod  in b.mods.Keys)
        {
            stats[mod]+=b.mods[mod];
        }
    }
    public void addBuff(Buff b)
    {
        buffs.Add(b);
    }
    public void removeBuff(Buff b)
    {
        buffs.Remove(b);
    }
    public float buf(string key)
    {
        return stats[key];
    }

}
