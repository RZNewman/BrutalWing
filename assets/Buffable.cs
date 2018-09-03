using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buffable : MonoBehaviour {
    List<Buff> buffs;
    Dictionary<string, data> stats;
    static Dictionary<string, data> BASESTATS;
    public static void baseline(){
        BASESTATS = new Dictionary<string, data>();
        //BASESTATS.Add("moveWorldX", new data(combineType.add, 0));
    }
    void Start()
    {
        stats = new Dictionary<string, data>(BASESTATS);
    }
    public void addBuff(Buff b)
    {
        buffs.Add(b);
        foreach(string mod  in b.mods.Keys)
        {
            stats[mod].add(b.mods[mod]);
        }
    }
    public void removeBuff(Buff b)
    {
        buffs.Remove(b);
        foreach (string mod in b.mods.Keys)
        {
            stats[mod].remove(b.mods[mod]);
        }
    }
    public enum combineType
    {
        add,
        multiply
    }
    public struct data
    {
        combineType c;
        float value;
        public data(combineType type, float v)
        {
            c = type;
            value = v;
        }
        public void add(float other)
        {
            switch (c)
            {
                case combineType.add:
                    value += other;
                    break;
                case combineType.multiply:
                    value *= other;
                    break;
            }
        }
        public void remove(float other)
        {
            switch (c)
            {
                case combineType.add:
                    value -= other;
                    break;
                case combineType.multiply:
                    value /= other;
                    break;
            }
        }

    }
}
