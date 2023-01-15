using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Text;
using UnityObject = UnityEngine.Object;

public class SoundTool : EditorWindow
{
    public int uiWidthLarge = 450;
    public int uiWidthMiddle = 300;
    public int uiWidthSamll = 200;

    private static int selection = 0;
    private Vector2 scrollPos1 = Vector2.zero;
    private Vector2 scrollPos2 = Vector2.zero;

    private AudioClip soundSource;

    private static SoundData soundData;

    [MenuItem("Tools/SoundData Tool")]
    private static void Init()
    {
        soundData = CreateInstance<SoundData>();
        soundData.LoadData();
        selection = 0;
        SoundTool t_window = GetWindow<SoundTool>(false, "Sound Tool");
        t_window.Show();
    }

    private void OnGUI()
    {
        if (soundData == null) return;

        EditorGUILayout.BeginVertical();
        {
            UnityObject t_source = soundSource;
            EditorHelper.EditorToolTopLayer(soundData, ref selection, ref t_source, uiWidthMiddle);
            soundSource = t_source as AudioClip;

            EditorGUILayout.BeginHorizontal();
            {
                EditorHelper.EditorToolListLayer(ref scrollPos1, soundData, ref selection, ref t_source, uiWidthMiddle);
                soundSource = t_source as AudioClip;

                EditorGUILayout.BeginVertical();
                {
                    scrollPos2 = EditorGUILayout.BeginScrollView(scrollPos2);
                    {
                        if (soundData.GetDataCount() > 0)
                        {
                            EditorGUILayout.BeginVertical();
                            {
                                EditorGUILayout.Separator();

                                SoundClip t_clip = soundData.soundClips[selection];

                                EditorGUILayout.LabelField("ID", selection.ToString(), GUILayout.Width(uiWidthLarge));
                                soundData.names[selection] = EditorGUILayout.TextField("Name", soundData.names[selection], GUILayout.Width(uiWidthLarge));
                                t_clip.playType = (ESoundPlayType)EditorGUILayout.EnumPopup("Play Type", t_clip.playType, GUILayout.Width(uiWidthLarge));
                                t_clip.maxVolume = EditorGUILayout.FloatField("Max Volume", t_clip.maxVolume, GUILayout.Width(uiWidthLarge));
                                t_clip.isLoop = EditorGUILayout.Toggle("Loop", t_clip.isLoop, GUILayout.Width(uiWidthLarge));

                                EditorGUILayout.Separator();

                                if (soundSource == null && t_clip.clipName != string.Empty)
                                    soundSource = Resources.Load(t_clip.clipPath + t_clip.clipName) as AudioClip;

                                soundSource = EditorGUILayout.ObjectField("Audio Clip", soundSource, typeof(AudioClip), false, GUILayout.Width(uiWidthLarge)) as AudioClip;
                                if (soundSource != null)
                                {
                                    t_clip.clipPath = EditorHelper.GetPath(soundSource);
                                    t_clip.clipName = soundSource.name;
                                    t_clip.pitch = EditorGUILayout.Slider("Pitch", t_clip.pitch, -3.0f, 3.0f, GUILayout.Width(uiWidthLarge));
                                    t_clip.dopplerLevel = EditorGUILayout.Slider("Doppler Level", t_clip.dopplerLevel, 0.0f, 5.0f, GUILayout.Width(uiWidthLarge));
                                    t_clip.rollOffMode = (AudioRolloffMode)EditorGUILayout.EnumPopup("Volume Rolloff", t_clip.rollOffMode, GUILayout.Width(uiWidthLarge));
                                    t_clip.minDist = EditorGUILayout.FloatField("Min Distance", t_clip.minDist, GUILayout.Width(uiWidthLarge));
                                    t_clip.maxDist = EditorGUILayout.FloatField("Max Distance", t_clip.maxDist, GUILayout.Width(uiWidthLarge));
                                    t_clip.spatialBlend = EditorGUILayout.Slider("Pan Level", t_clip.spatialBlend, 0.0f, 1.0f, GUILayout.Width(uiWidthLarge));
                                }
                                else
                                {
                                    t_clip.clipPath = string.Empty;
                                    t_clip.clipName = string.Empty;
                                }

                                EditorGUILayout.Separator();

                                if (GUILayout.Button("Add Loop", GUILayout.Width(uiWidthMiddle)))
                                    t_clip.AddLoop();

                                for (int i = 0; i < t_clip.checkTime.Length; i++)
                                {
                                    EditorGUILayout.BeginVertical();
                                    {
                                        GUILayout.Label("Loop Step " + i, EditorStyles.boldLabel);
                                        if (GUILayout.Button("Remove", GUILayout.Width(uiWidthMiddle)))
                                        {
                                            t_clip.RemoveLoop(i);
                                            EditorGUILayout.EndVertical();
                                            continue;
                                        }
                                        t_clip.checkTime[i] = EditorGUILayout.FloatField("Check Time", t_clip.checkTime[i], GUILayout.Width(uiWidthMiddle));
                                        t_clip.setTime[i] = EditorGUILayout.FloatField("Set Time", t_clip.setTime[i], GUILayout.Width(uiWidthMiddle));
                                    }
                                    EditorGUILayout.EndVertical();
                                }
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
                soundData = CreateInstance<SoundData>();
                soundData.LoadData();
                selection = 0;
                soundSource = null;
            }
            if (GUILayout.Button("Save Settings"))
            {
                soundData.SaveData();
                CreateEnumStructure();
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    public void CreateEnumStructure()
    {
        string t_enumName = "SoundList";
        StringBuilder t_builder = new StringBuilder();

        t_builder.AppendLine();

        int t_length = soundData.names != null ? soundData.names.Length : 0;
        for (int i = 0; i < t_length; i++)
        {
            if (soundData.names[i] != string.Empty)
            {
                string t_name = soundData.names[i];
                t_name = string.Concat(t_name.Where(t_char => !char.IsWhiteSpace(t_char)));
                t_builder.AppendLine("    " + t_name + " = " + i + ",");
            }
        }
        EditorHelper.CreateEnumStructure(t_enumName, t_builder);
    }
}
