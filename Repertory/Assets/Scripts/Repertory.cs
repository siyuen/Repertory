using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEditor;
using UnityEngine.Serialization;

public class Repertory : MonoBehaviour {

    //@消除转义
    public string PATH;//E:/SiYuen/Practice/Repertory/Assets
    //文件后缀
    public const string EXTENSION_AVI = ".avi";
    public const string EXTENSION_WMV = ".wmv";
    public const string EXTENSION_MP4 = ".mp4";
    public const string EXTENSION_MKV = ".mkv";
    public const string EXTENSION_RMVB = ".rmvb";
    public const string EXTENSION_DIVX = ".divx";
    public const string TEST = ".txt";
    //每行个数
    public const int ROW_COUNT = 10;
    //框
    public const string KUANG = "[]";
    //前缀最小长度
    public const int PRE_MIN = 4;
    //名字最小长度
    public const int NAME_MIN = 3;
    //引用ui
    public Button btn;
    public Button reNameBtn;
    public InputField outputText;
    public InputField inputText;
    public InputField debugText;
    public InputField recyleText;
    public HistoricalRecords recordView;
    //模式
    public Toggle singleNumber;
    public Toggle allNumber;
    public enum SearchType
    {
        single,
        all,
    }
    private string num;
    private SearchType type;
    private HistoricalRecords records;

	void Awake () {
        type = SearchType.single;
        InitUI();
	}

    void InitUI()
    {
        singleNumber.onValueChanged.AddListener(FirstType);
        singleNumber.transform.Find("InputField").GetComponent<InputField>().onEndEdit.AddListener(OnSubmitPath);
        allNumber.onValueChanged.AddListener(SecondType);

        btn.onClick.AddListener(LogPath);
        reNameBtn.onClick.AddListener(ResetName);

        EventMgr.Instance().AddEventListener(EventDefine.ONCLICK_RECARD, OnClickRecard);
    }

    #region 事件
    //[MenuItem("Tools/遍历文件")]
    private void LogPath()
    {
        RefreshOutPut();
        GetPath();
        if (string.IsNullOrEmpty(PATH))
        {
            debugText.text = "路径不能为空";
            return;
        }

        //遍历文件存入list
        List<string> data = new List<string>();
        Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();
        if (type == SearchType.single)
            GetDirs(PATH, ref data);
        else if (type == SearchType.all)
            GetDirs(PATH, ref data);
        Helper.ListStringSort(ref data);

        //处理前缀
        string first = data.Count == 0 ? "" : data[0].PadLeft(NAME_MIN, '0');
        string pre = string.IsNullOrEmpty(inputText.text) ? "" : inputText.text;
        if (pre.Length < PRE_MIN) pre = pre.PadRight(PRE_MIN, ' ');
        pre += KUANG[0];

        //写入string
        StreamWriter sw;
        string path = Application.dataPath + "/Output" + "/" + "output.txt";
        if (!File.Exists(path))
            sw = File.CreateText(path);//创建一个用于写入 UTF-8 编码的文本  
        else
        {
            File.Delete(path);
            sw = File.CreateText(path);
        }
        int row = 1;
        string number = first;
        //以行为单位写入text
        string rowString = pre + first;
        for (int i = 1; i < data.Count; i++)
        {
            //处理名字
            string name = data[i];
            if (name.Length < NAME_MIN) name = name.PadLeft(NAME_MIN, '0');
            //处理隔行
            row += 1;
            if (row <= ROW_COUNT)
            {
                number = number + "," + name;
                rowString = rowString + "," + name;
                //若最后且不满一行
                if (i == data.Count - 1)
                    sw.WriteLine(rowString + KUANG[1]);
            }
            else
            {
                sw.WriteLine(rowString);
                number = number + ",\n" + name.PadLeft(Helper.GetNumberSpace(pre), ' ');
                row -= ROW_COUNT;
                rowString = name.PadLeft(8, ' ');
            }
        }
        sw.Close();
        sw.Dispose();//文件流释放  
        Debug.Log(pre + number + KUANG[1]);
        num = pre + number + KUANG[1];
        outputText.text = outputText.text + num;
        
        SetDebug(data);
        //更新历史记录并刷新
        Recyle.Instance().PushPool(PATH);
        recordView.Refresh();
    }

    /// <summary>
    /// 重置文件名字,去除番号与前缀中间的其他符号
    /// </summary>
    private void ResetName()
    {
        RefreshOutPut();
        GetPath();
        if (string.IsNullOrEmpty(PATH))
        {
            debugText.text = "路径不能为空";
            return;
        }

        string[] files = Directory.GetFiles(PATH);
        foreach (var path in files)
        {
            string extension = Path.GetExtension(path);
            //获取所有文件夹中包含后缀为xx的路径  
            if (extension == EXTENSION_AVI || extension == EXTENSION_WMV ||
                extension == EXTENSION_MP4 || extension == EXTENSION_MKV ||
                extension == EXTENSION_RMVB || extension == EXTENSION_DIVX ||
                extension == TEST)
            {
                //从真实文件名开始添加
                string name = path.Substring(PATH.Length);
                string pre = string.IsNullOrEmpty(inputText.text) ? "" : inputText.text;

                RemovePre(ref name);
                var oldName = name;
                Helper.String2Int2(ref name);
                var newName = name;
                name = PATH + "\\" + pre + "_" + name;

                if (File.Exists(path))
                {
                    Debug.Log(oldName + "替换成:" + newName);
                    File.Move(path, name);
                }
                //data.Add(name);
            }
        }
    }

    /// <summary>
    /// 点击历史记录,传路径
    /// </summary>
    private void OnClickRecard(EventParam param)
    {
        singleNumber.transform.Find("InputField").GetComponent<InputField>().text = param.StrParam;
        OnSubmitPath(param.StrParam);
    }
    /// <summary>
    /// 输出日志
    /// </summary>
    private void SetDebug(List<string> data)
    {
        //记录文件数
        debugText.text = "文件数:" + data.Count + "\n";
        debugText.text = debugText.text + "相同番号:" + Helper.SearchSameName(ref data) + "\n";
    }

    /// <summary>
    /// 单一番号模式,自动补番号
    /// </summary>
    private void FirstType(bool check)
    {
        GameObject input = singleNumber.transform.Find("InputField").gameObject;
        input.SetActive(check);
        if (check)
            type = SearchType.single;
    }

    /// <summary>
    /// 全部番号模式
    /// </summary>
    private void SecondType(bool check)
    {
        allNumber.transform.Find("InputField").gameObject.SetActive(check);
        if (check)
            type = SearchType.all;
    }

    /// <summary>
    /// 提交路径,单一模式下补全
    /// </summary>
    public void OnSubmitPath(string text)
    {
        if (string.IsNullOrEmpty(text)) return;

        PATH = @text;
        if (type == SearchType.single)
        {
            string pre = Helper.GetPathPre(PATH);
            inputText.text = pre;
        }
        else if (type == SearchType.all)
        {
        }
    }

    #endregion

    #region 方法
    private void GetPath()
    {
        if (type == SearchType.single)
            PATH = singleNumber.transform.Find("InputField").GetComponent<InputField>().text;
        else
            PATH = allNumber.transform.Find("InputField").GetComponent<InputField>().text;
    }

    /// <summary>
    /// 刷新界面
    /// </summary>
    private void RefreshOutPut()
    {
        debugText.text = "";
        outputText.text = "";
    }

    private void GetDirs(string dirPath, ref List<string> data)
    {
        string[] files = Directory.GetFiles(dirPath);
        foreach (string path in files)
        {
            string extension = System.IO.Path.GetExtension(path);
            //获取所有文件夹中包含后缀为xx的路径  
            if (extension == EXTENSION_AVI || extension == EXTENSION_WMV ||
                extension == EXTENSION_MP4 || extension == EXTENSION_MKV ||
                extension == EXTENSION_RMVB || extension == EXTENSION_DIVX ||
                extension == TEST)
            {
                //从真实文件名开始添加
                string name = path.Substring(PATH.Length);
                //去掉后缀
                int ext = name.IndexOf(".");
                name = name.Substring(0, ext);
                RemovePre(ref name);
                Helper.String2Int(ref name);
                data.Add(name);
            }
        }
    }

    private void GetDirs(string dirPath, ref Dictionary<string, List<string>> data)
    {
        string[] files = Directory.GetFiles(dirPath);
        foreach (string path in files)
        {
            string extension = System.IO.Path.GetExtension(path);
            //获取所有文件夹中包含后缀为xx的路径  
            if (extension == EXTENSION_AVI || extension == EXTENSION_WMV ||
                extension == EXTENSION_MP4 || extension == EXTENSION_MKV ||
                extension == EXTENSION_RMVB || extension == EXTENSION_DIVX)
            {
                //从真实文件名开始添加
                string name = path.Substring(PATH.Length);
                //去掉后缀
                int ext = name.IndexOf(".");
                name = name.Substring(0, ext);
                RemovePre(ref name);
                Helper.String2Int(ref name);
                //data.Add(name);
            }
        }
    }
    /// <summary>
    /// 处理前缀
    /// </summary>
    private void RemovePre(ref string name)
    {
        string pre = string.IsNullOrEmpty(inputText.text) ? "" : inputText.text;
        name = name.Substring(pre.Length + 1);
    }
    #endregion
}