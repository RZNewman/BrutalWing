using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AbilPanel : MonoBehaviour {
    public AbilSelector holder;
    ElementPanel selected;
	// Use this for initialization
	void Start () {
        foreach (Transform child in transform)
        {
            ElementPanel ele = child.GetComponent<ElementPanel>();
            if (ele)
            {
                ele.setPanel(this);
            }
            
        }
    }
    public void select(ElementPanel e)
    {
        if (selected)
        {
            selected.deselect();
        }
        selected = e;
    }
	
    public void Proceed()
    {
        if (selected)
        {
            GameObject[] sel = selected.getSelected();
            holder.hold(sel[0], sel[1]);
            
            //Debug.Break();
            //SceneManager.LoadScene(1);
        }
    }
}
