using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Collections;

public class EventMgr {

    public delegate void NormalDelegate(EventParam param);
    public Dictionary<string, NormalDelegate> eventDic;
    public List<EventParam> paramList;

    private static EventMgr instance;

    public EventMgr()
    {
        eventDic = new Dictionary<string, NormalDelegate>();
        paramList = new List<EventParam>();
    }

    public static EventMgr Instance()
    {
        if (instance == null)
            instance = new EventMgr();

        return instance;
    }

    public void AddEventListener(string eventId, NormalDelegate func)
    {
        if (eventDic.ContainsKey(eventId))
            eventDic[eventId] += func;
        else
            eventDic[eventId] = func;
    }

    public void RemoveEventListener(string eventId, NormalDelegate func)
    {
        if (eventDic.ContainsKey(eventId))
            eventDic[eventId] -= func;
    }

    public void DispathEvent(string eventId, EventParam param)
    {
        if (eventDic.ContainsKey(eventId))
        {
            eventDic[eventId](param);
            param.ClearData();
            paramList.Add(param);
        }
    }

    public EventParam GetParam(int intParam = 0, string strParam = "", GameObject mySelf = null, GameObject target = null)
    {
        if (paramList.Count > 0)
        {
            EventParam param = paramList[0];
            paramList.RemoveAt(0);
            param.IntParam = intParam;
            param.StrParam = strParam;
            param.Myself = mySelf;
            param.Target = target;

            return param;
        }
        else
            return new EventParam(intParam, strParam, mySelf, target);
    }
}
