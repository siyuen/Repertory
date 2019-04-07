using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Recyle {

    public const string NAME = "RecyleData.txt";
    public List<string> pathList = new List<string>();
    private static Recyle instance;

    public static Recyle Instance()
    {
        if (instance == null)
            instance = new Recyle();
        return instance;
    }

    public Recyle()
    {
        LoadInfo();
    }

    /// <summary>
    /// 添加前查重排序
    /// </summary>
    private bool CheckList(string info)
    {
        bool b = false;
        int idx = -1;
        for (int i = 0; i < pathList.Count; i++)
        {
            if (pathList[i].Equals(info))
            {
                idx = i;
                b = true;
                break;
            }
        }
        if (b)
        {
            pathList.RemoveAt(idx);
            pathList.Insert(0, info);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 放入回收站
    /// </summary>
    public void PushPool(string info)
    {
        StreamWriter sw;
        string path = Application.dataPath + "/Output/" + NAME;
        if (!File.Exists(path))
        {
            sw = File.CreateText(path);//创建一个用于写入 UTF-8 编码的文本  
            Debug.Log("文件创建成功！");
        }
        else
        {
            sw = File.AppendText(path);//打开现有 UTF-8 编码文本文件以进行读取  
        }
        if (CheckList(info))
        {
            //将记录重新排序
            sw.Close();
            sw.Dispose();
            File.Delete(path);
            sw = File.CreateText(path);
            for (int i = 0; i < pathList.Count; i++)
                sw.WriteLine(pathList[i]);//以行为单位写入字符串  
        }
        else
        {
            //换行
            sw.WriteLine("\n");
            sw.WriteLine(info);//以行为单位写入字符串  
        }
        sw.Close();
        sw.Dispose();//文件流释放  
    }

    /// <summary>
    /// 加载记录
    /// </summary>
    private void LoadInfo()
    {
        StreamReader sr;
        string path = Application.dataPath + "/Output/" + NAME;
        if (File.Exists(path))
        {
            sr = File.OpenText(path);
        }
        else
        {
            Debug.LogWarning("Not find files!");
            return;
        }
        pathList = new List<string>();
        string str;
        while ((str = sr.ReadLine()) != null)
            if (!string.IsNullOrEmpty(str))
                pathList.Add(str);//加上str的临时变量是为了避免sr.ReadLine()在一次循环内执行两次  
        sr.Close();
        sr.Dispose();  
    }

    /// <summary>
    /// 清除记录
    /// </summary>
    public void ClearPath()
    {
        StreamReader sr;
        string path = Application.dataPath + "/Output/" + NAME;

        if (File.Exists(path))
        {
            File.Delete(path);
            File.CreateText(path);
        }
        else
        {
            Debug.LogWarning("Not find files!");
            return;
        }
    }
}
