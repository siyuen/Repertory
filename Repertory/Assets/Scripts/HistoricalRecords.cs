using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

/// <summary>
/// 处理查询记录
/// 顺便优化scrollview
/// </summary>
public class HistoricalRecords : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    public const int COUNT = 50;
    public Button clearBtn;

    private ScrollRect view;
    private RectTransform viewPointTrans;
    private RectTransform contentTrans;
    private Vector2 cellSize;

    private List<Button> btnList = new List<Button>();
    private List<GameObject> curPool = new List<GameObject>();

    //记录头尾
    private int curHead;
    private int curTail;
    //记录pos
    private Vector3 beginPos;
    private Vector3 curPos;
    private Vector3 endPos;

    void Awake()
    {
        view = this.GetComponent<ScrollRect>();
        Init();
    }

    /// <summary>
    /// 清空，放入资源池
    /// </summary>
    public void Clear()
    {
        for (int i = 0; i < curPool.Count; i++)
        {
            ResourcesMgr.Instance().PushPool(PrefabDefine.RECORD, curPool[i]);
        }
    }

    /// <summary>
    /// 刷新view
    /// </summary>
    public void Refresh()
    {
        Clear();
        InitUI();
    }

    private void Init()
    {
        viewPointTrans = view.transform.Find("Viewport").GetComponent<RectTransform>();
        contentTrans = viewPointTrans.transform.Find("Content").GetComponent<RectTransform>();
        clearBtn.onClick.AddListener(OnClickClear);

        InitUI();
    }

    private void InitUI()
    {
        float viewHeight = view.GetComponent<RectTransform>().sizeDelta.y;
        cellSize = contentTrans.GetComponent<GridLayoutGroup>().cellSize;
        float cellHeight = cellSize.y;
        //view能容纳个数
        float count = viewHeight / cellHeight;
        //根据需要加载个数
        int needCount = Recyle.Instance().pathList.Count;
        //根据需要加载个数初始化contentsize
        contentTrans.GetComponent<RectTransform>().sizeDelta = new Vector2(0, needCount * cellHeight);
        if (count - (int)count > 0)
            count = (int)count + 1;
        //实际需要
        int realCount = 0;
        if (needCount >= (int)count)
            realCount = (int)count;
        else
            realCount = needCount;

        for (int i = 0; i < realCount; i++)
        {
            GameObject go = ResourcesMgr.Instance().PopPool(PrefabDefine.RECORD);
            go.name = "record_" + i;
            go.transform.SetParent(contentTrans);
            go.transform.localScale = Vector3.one;
            go.transform.Find("Text").GetComponent<Text>().text = Recyle.Instance().pathList[i];

            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate()
            {
                OnClickRecord(go);
            });
        }
        curHead = 0;
        curTail = (int)count - 1;
    }

    private void OnClickClear()
    {
        Recyle.Instance().ClearPath();

        Clear();
    }

    /// <summary>
    /// 点击历史记录
    /// </summary>
    private void OnClickRecord(GameObject btn)
    {
        Text txt = btn.transform.Find("Text").GetComponent<Text>();

        EventMgr.Instance().DispathEvent(EventDefine.ONCLICK_RECARD, EventMgr.Instance().GetParam(0, txt.text));
    }

    /// <summary>
    /// 指定一个 item让其定位到ScrollRect中间
    /// </summary>
    public void CenterOnItem(int idx)
    {
        if (idx > btnList.Count)
            return;
    }

    #region 循环利用item
    /// <summary>
    /// 拖动过程中刷新UI
    /// </summary>
    void RefreshUI()
    {
        int needCount = Recyle.Instance().pathList.Count;
        //判断向上还是向下
        if (curPos.y > beginPos.y)
        {
            //Debug.Log("向下");
            if (curTail >= needCount - 1)
                return;
            int count = curPool.Count;
            int idx = 0;
            for (int i = 0; i < count; i++)
            {
                if (curPos.y + curPool[idx].transform.localPosition.y >= 0)
                {
                    GameObject go = curPool[idx];
                    curPool.RemoveAt(idx);
                    ResourcesMgr.Instance().PushPool(PrefabDefine.RECORD, go);
                    AddItemInGrid(++curTail);
                }
                else
                    return;
            }
        }
        else if (curPos.y < beginPos.y)
        {
            //Debug.Log("向上");
            if (curHead <= 0)
                return;
            int count = curPool.Count;
            int idx = count - 1;
            float height = view.GetComponent<RectTransform>().sizeDelta.y;
            for (int i = count - 1; i >= 0; i--)
            {
                if (curPos.y + height + curPool[idx].transform.localPosition.y < cellSize.y)
                {
                    GameObject go = curPool[idx];
                    curPool.RemoveAt(idx);
                    ResourcesMgr.Instance().PushPool(PrefabDefine.RECORD, go);
                    AddItemInGrid(--curHead, false);
                }
                else
                    return;
            }
        }
    }

    /// <summary>
    /// 添加进grid
    /// </summary>
    void AddItemInGrid(int id, bool tail = true)
    {
        //添加到最后一个
        GameObject newGO = ResourcesMgr.Instance().PopPool(PrefabDefine.RECORD);
        newGO.transform.SetParent(contentTrans);
        newGO.transform.localScale = Vector3.one;
        if (tail)
        {
            //Debug.Log("添加到尾部:" + id);
            newGO.transform.localPosition = curPool[curPool.Count - 1].transform.localPosition - new Vector3(0, cellSize.y, 0);
            curHead++;
            curPool.Add(newGO);
        }
        else
        {
            //Debug.Log("添加到头部:" + id);
            newGO.transform.localPosition = curPool[0].transform.localPosition + new Vector3(0, cellSize.y, 0);
            curTail--;
            curPool.Insert(0, newGO);
        }
        newGO.name = "record_" + id;
        newGO.transform.Find("Text").GetComponent<Text>().text = Recyle.Instance().pathList[id];
        beginPos = contentTrans.localPosition;
    }
	
    /// <summary>
    /// 开始拖动
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        beginPos = contentTrans.localPosition;
        //Debug.Log("初始位置:" + beginPos);
    }

    /// <summary>
    /// 拖动中
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        curPos = contentTrans.localPosition;
        //Debug.Log("当前位置:" + curPos);
        RefreshUI();
    }

    /// <summary>
    /// 结束拖动
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        endPos = contentTrans.localPosition;
    }
    #endregion
}