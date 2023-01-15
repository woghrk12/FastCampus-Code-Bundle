using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;

public class EditorHelper
{

	/// <summary>
	/// 경로 계산 함수.
	/// </summary>
	/// <param name="p_clip"></param>
	/// <returns></returns>
	public static string GetPath(UnityEngine.Object p_clip)
	{
		string retString = string.Empty;
		retString = AssetDatabase.GetAssetPath(p_clip);
		string[] path_node = retString.Split('/'); //Assets/9.ResourcesData/Resources/Sound/BGM.wav
		bool findResource = false;
		for (int i = 0; i < path_node.Length - 1; i++)
		{
			if (findResource == false)
			{
				if (path_node[i] == "Resources")
				{
					findResource = true;
					retString = string.Empty;
				}
			}
			else
			{
				retString += path_node[i] + "/";
			}

		}

		return retString;
	}

	/// <summary>
	/// Data 리스트를 enum structure로 뽑아주는 함수.
	/// </summary>
	public static void CreateEnumStructure(string enumName, StringBuilder data)
	{
		string templateFilePath = "Assets/Editor/EnumTemplate.txt";

		string entittyTemplate = File.ReadAllText(templateFilePath);

		entittyTemplate = entittyTemplate.Replace("$DATA$", data.ToString());
		entittyTemplate = entittyTemplate.Replace("$ENUM$", enumName);
		string folderPath = "Assets/1.Scripts/GameData/";
		if (Directory.Exists(folderPath) == false)
		{
			Directory.CreateDirectory(folderPath);
		}

		string FilePath = folderPath + enumName + ".cs";
		if (File.Exists(FilePath))
		{
			File.Delete(FilePath);
		}
		File.WriteAllText(FilePath, entittyTemplate);
	}

	public static void EditorToolTopLayer(BaseData p_data, ref int p_selection, ref UnityEngine.Object p_source, int p_uiWidth)
	{
		EditorGUILayout.BeginHorizontal();
		{
			if (GUILayout.Button("Add", GUILayout.Width(p_uiWidth)))
			{
				p_data.AddData("New Data");
				p_selection = p_data.GetDataCount() - 1;
				p_source = null;
			}
			if (GUILayout.Button("Copy", GUILayout.Width(p_uiWidth)))
			{
				p_data.CopyData(p_selection);
				p_selection = p_data.GetDataCount() - 1;
				p_source = null;
			}
			if (p_data.GetDataCount() > 0 && GUILayout.Button("Remove", GUILayout.Width(p_uiWidth)))
			{
				p_source = null;
				p_data.RemoveData(p_selection);
			}
			if (p_data.GetDataCount() > 0 && GUILayout.Button("Clear", GUILayout.Width(p_uiWidth)))
			{
				p_source = null;
				p_data.ClearData();
			}
			if (p_selection > p_data.GetDataCount() - 1) p_selection = p_data.GetDataCount() - 1;
		}
		EditorGUILayout.EndHorizontal();
	}

	public static void EditorToolListLayer(ref Vector2 p_scrollPos, BaseData p_data, ref int p_selection, ref UnityEngine.Object p_source, int p_uiWidth)
	{
		EditorGUILayout.BeginVertical(GUILayout.Width(p_uiWidth));
		{
			EditorGUILayout.Separator();
			EditorGUILayout.BeginVertical("box");
			{
				p_scrollPos = EditorGUILayout.BeginScrollView(p_scrollPos);
				{
					if (p_data.GetDataCount() > 0)
					{
						int t_lastSelection = p_selection;
						p_selection = GUILayout.SelectionGrid(p_selection, p_data.GetNameList(true), 1);
						if (t_lastSelection != p_selection) p_source = null;
					}
				}
				EditorGUILayout.EndScrollView();
			}
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndVertical();
	} 

}
