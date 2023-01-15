using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base class of data
/// </summary>
public class BaseData : ScriptableObject
{
    public const string dataDirectory = "/9.ResourcesData/Resources/Data/";
    public string[] names = null;

    public BaseData()
    {

    }

    public int GetDataCount()
    {
        return names == null ? 0 : names.Length;
    }

    public string[] GetNameList(bool p_isShowID, string p_filterWord = "")
    {
        string[] t_retList = new string[0];
        if (names == null) return t_retList;

        t_retList = new string[names.Length];

        for (int i = 0; i < names.Length; i++)
        {
            if (p_filterWord != "" && !names[i].ToLower().Contains(p_filterWord.ToLower())) continue;
            t_retList[i] = p_isShowID ? i.ToString() + " : " + names[i] : names[i];
        }

        return t_retList;
    }

    public virtual void AddData(string p_newName) { }
    public virtual void RemoveData(int p_idx) { }
    public virtual void ClearData() { }
    public virtual void CopyData(int p_idx) { }
}
