using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper {

    #region 私有处理方法
    /// <summary>
    /// number排序,插排
    /// </summary>
    public static void ListStringSort(ref List<string> data)
    {
        if (data.Count == 0) return;

        for (int i = 1; i < data.Count; i++)
        {
            string str = data[0];
            for (int j = i; j > 0; j--)
            {
                int a, b;
                int.TryParse(data[j], out a);
                int.TryParse(data[j - 1], out b);
                if (a < b)
                {
                    string tem = data[j - 1];
                    data[j - 1] = data[j];
                    data[j] = tem;
                }
            }
        }
    }

    /// <summary>
    /// 获取前缀空格,编辑器中
    /// </summary>
    public static int GetNumberSpace(string name)
    {
        if (string.IsNullOrEmpty(name))
            return 0;

        int space = name.Length;
        int count = 0;
        //暂时只处理数字和字母
        for (int i = 0; i < name.Length; i++)
        {
            if (name[i] == ' ')
            {
                space--;
                count++;
            }
        }
        space = 3 + space * 2 + count;
        return space;
    }
    
    /// <summary>
    /// 获取前缀空格,text中
    /// </summary>
    /// <param name="name"></param>
    public static int GetNumberSpaceInText(string name)
    {
        if (string.IsNullOrEmpty(name))
            return 0;

        int space = name.Length;
        int count = 0;
        //暂时只处理数字和字母
        for (int i = 0; i < name.Length; i++)
        {
            if (name[i] == ' ')
            {
                space--;
                count++;
            }
        }
        space = 3 + space * 2 + count;
        return space;
    }

    /// <summary>
    /// 处理前缀与番号之间的符号,只记录番号
    /// </summary>
    public static void String2Int(ref string number)
    {
        if (string.IsNullOrEmpty(number))
            return;

        int count = number.Length;
        string newNumber = "";
        bool bDone = false;

        for (int i = 0; i < count; i++)
        {
            if ((int)number[i] >= 48 && (int)number[i] <= 57 && !bDone)
                newNumber += number[i];
            else
                if (i > 3)
                    bDone = true;
        }
        number = newNumber;
    }

    public static void String2Int2(ref string number)
    {
        if (string.IsNullOrEmpty(number))
            return;

        int count = number.Length;
        string newNumber = "";
        bool bStart = false;

        for (int i = 0; i < count; i++)
        {
            if ((int)number[i] >= 48 && (int)number[i] <= 57 && !bStart)
                bStart = true;
                
            if (bStart)
                newNumber += number[i];
        }
        number = newNumber;
    }

    /// <summary>
    /// 获取路径上一层级的名字
    /// </summary>
    public static string GetPathPre(string path)
    {
        string name = "";
        int idx = 0;
        for (int i = 0; i < path.Length; i++)
            if (path[i] == '/' || path[i] == '\\')
                idx = i;
        name = path.Substring(idx + 1);
        return name;
    }

    /// <summary>
    /// 查重
    /// </summary>
    public static string SearchSameName(ref List<string> data)
    {
        string same = "";
        for (int i = 1; i < data.Count; i++)
            if (data[i].Equals(data[i - 1]))
                same += data[i];
        return same;
    }

    #endregion
}
