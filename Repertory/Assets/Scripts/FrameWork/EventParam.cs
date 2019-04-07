using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventParam {

    private int intParam;
    public int IntParam { set { intParam = value; } get { return intParam; } }

    private string strParam;
    public string StrParam { set { strParam = value; } get { return strParam; } }

    private GameObject mySelf;
    public GameObject Myself { set { mySelf = value; } get { return mySelf; } }

    private GameObject target;
    public GameObject Target { set { target = value; } get { return target; } }
    
    public EventParam(int intParam = 0, string strParam = "", GameObject mySelf = null, GameObject target = null)
    {
        this.intParam = intParam;
        this.strParam = strParam;
        this.mySelf = mySelf;
        this.target = target;
    }

    public void ClearData()
    {
        intParam = 0;
        strParam = "";
        mySelf = null;
        target = null;
    }
}
