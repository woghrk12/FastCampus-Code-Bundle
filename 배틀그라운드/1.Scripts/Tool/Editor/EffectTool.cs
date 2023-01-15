using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Text;
using UnityObject = UnityEngine.Object;

public class EffectTool : EditorWindow
{
    public int uiWidthLarge = 300;
    public int uiWidthMiddle = 200;
    private static int selection = 0;
    private Vector2 scrollPos1 = Vector2.zero;
    private Vector2 scrollPos2 = Vector2.zero;

    private GameObject effectSource = null;

    private static EffectData effectData;

    [MenuItem("Tools/Effect Tool")]
    private static void Init()
    {
        effectData = ScriptableObject.CreateInstance<EffectData>();
        effectData.LoadData();
        selection = 0;
        EffectTool t_window = GetWindow<EffectTool>(false, "Effect Tool");
        t_window.Show();
    }

    private void OnGUI()
    {
        if (effectData == null) return;

        EditorGUILayout.BeginVertical();
        {
            UnityObject t_source = effectSource;
            EditorHelper.EditorToolTopLayer(effectData, ref selection, ref t_source, uiWidthMiddle);
            effectSource = t_source as GameObject;

            EditorGUILayout.BeginHorizontal();
            {
                EditorHelper.EditorToolListLayer(ref scrollPos1, effectData, ref selection, ref t_source, uiWidthLarge);
                effectSource = t_source as GameObject;

                EditorGUILayout.BeginVertical();
                {
                    scrollPos2 = EditorGUILayout.BeginScrollView(scrollPos2);
                    {
                        if (effectData.GetDataCount() > 0)
                        {
                            EditorGUILayout.BeginVertical();
                            {
                                EditorGUILayout.Separator();
                                EditorGUILayout.LabelField("ID", selection.ToString(), GUILayout.Width(uiWidthLarge));
                                effectData.names[selection] = EditorGUILayout.TextField("Name", effectData.names[selection], GUILayout.Width(uiWidthLarge * 1.5f));
                                effectData.effectClips[selection].effectType = (EEffectType)EditorGUILayout.EnumPopup("Effect Type", effectData.effectClips[selection].effectType, GUILayout.Width(uiWidthLarge));
                                EditorGUILayout.Separator();
                                if (effectSource == null && effectData.effectClips[selection].effectName != string.Empty)
                                {
                                    effectData.effectClips[selection].PreLoad();
                                    effectSource = Resources.Load(effectData.effectClips[selection].EffectFullPath) as GameObject;
                                }
                                effectSource = EditorGUILayout.ObjectField("Effect", effectSource, typeof(GameObject), false, GUILayout.Width(uiWidthLarge * 1.5f)) as GameObject;
                                if (effectSource != null)
                                {
                                    effectData.effectClips[selection].effectPath = EditorHelper.GetPath(effectSource);
                                    effectData.effectClips[selection].effectName = effectSource.name;
                                }
                                else
                                {
                                    effectData.effectClips[selection].effectPath = string.Empty;
                                    effectData.effectClips[selection].effectName = string.Empty;
                                    effectSource = null;
                                }
                                EditorGUILayout.Separator();
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Reload Settings"))
            {
                effectData = CreateInstance<EffectData>();
                effectData.LoadData();
                selection = 0;
                effectSource = null;
            }
            if (GUILayout.Button("Save Settings"))
            {
                effectData.SaveData();
                CreateEnumStructure();
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    public void CreateEnumStructure()
    {
        string t_enumName = "EffectList";
        StringBuilder t_builder = new StringBuilder();

        t_builder.AppendLine();

        int t_length = effectData.names != null ? effectData.names.Length : 0;
        for (int i = 0; i < t_length; i++)
        {
            if (effectData.names[i] != string.Empty)
            {
                string t_name = effectData.names[i];
                t_name = string.Concat(t_name.Where(t_char => !char.IsWhiteSpace(t_char)));
                t_builder.AppendLine("    " + t_name + i + " = " + i + ",");
            }
        }
        EditorHelper.CreateEnumStructure(t_enumName, t_builder);
    }
}
