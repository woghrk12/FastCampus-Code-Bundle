using System;
using System.IO;
using System.Xml;
using UnityEngine;

/// <summary>
/// The class has as an effect clip list, effect file name and path.
/// The class has the ability to read and write files
/// </summary>
public class EffectData : BaseData
{
    public EffectClip[] effectClips = new EffectClip[0];

    public string clipPath = "Effects/";
    private string xmlFilePath = "";
    private string xmlFileName = "effectData.xml";
    private string dataPath = "Data/effectData";

    // XML delimiter
    private const string EFFECT = "effect";
    private const string CLIP = "clip";

    private EffectData() { }

    public void LoadData()
    {
        xmlFilePath = Application.dataPath + dataDirectory;
        TextAsset t_asset = (TextAsset)ResourceManager.Load(dataPath);
        if (t_asset == null || t_asset.text == null)
        {
            AddData("New Effect");
            return;
        }

        using (XmlReader t_reader = XmlReader.Create(new StringReader(t_asset.text)))
        {
            int t_curID = 0;
            while (t_reader.Read())
            {
                if (t_reader.IsStartElement())
                {
                    switch (t_reader.Name)
                    {
                        case "length":
                            int t_length = int.Parse(t_reader.ReadString());
                            names = new string[t_length];
                            effectClips = new EffectClip[t_length];
                            break;

                        case "id":
                            t_curID = int.Parse(t_reader.ReadString());
                            effectClips[t_curID] = new EffectClip();
                            effectClips[t_curID].clipId = t_curID;
                            break;

                        case "name":
                            names[t_curID] = t_reader.ReadString();
                            break;

                        case "effectType":
                            effectClips[t_curID].effectType = (EEffectType)Enum.Parse(typeof(EEffectType), t_reader.ReadString());
                            break;

                        case "effectName":
                            effectClips[t_curID].effectName = t_reader.ReadString();
                            break;

                        case "effectPath":
                            effectClips[t_curID].effectPath = t_reader.ReadString();
                            break;
                    }
                }
            }
        }
    }

    public void SaveData()
    {
        XmlWriterSettings t_settings = new XmlWriterSettings();
        t_settings.Encoding = System.Text.Encoding.Unicode;

        using (XmlWriter t_writer = XmlWriter.Create(xmlFilePath + xmlFileName, t_settings))
        {
            t_writer.WriteStartDocument();
            t_writer.WriteStartElement(EFFECT);
            t_writer.WriteElementString("length", GetDataCount().ToString());
            t_writer.WriteWhitespace("\n");

            int t_length = names != null ? names.Length : 0;

            for (int i = 0; i < t_length; i++)
            {
                EffectClip t_clip = effectClips[i];
                t_writer.WriteStartElement(CLIP);

                t_writer.WriteElementString("id", i.ToString());
                t_writer.WriteElementString("name", names[i]);
                t_writer.WriteElementString("effectType", t_clip.effectType.ToString());
                t_writer.WriteElementString("effectName", t_clip.effectName);
                t_writer.WriteElementString("effectPath", t_clip.effectPath);

                t_writer.WriteEndElement();
                t_writer.WriteWhitespace("\n");
            }

            t_writer.WriteEndElement();
            t_writer.WriteEndDocument();
        }
    }

    public override void AddData(string p_newName)
    {
        if (names == null)
        {
            names = new string[] { p_newName };
            effectClips = new EffectClip[] { new EffectClip() };
        }
        else 
        {
            names = ArrayHelper.Add(p_newName, names);
            effectClips = ArrayHelper.Add(new EffectClip(), effectClips);
        }
    }

    public override void RemoveData(int p_idx)
    {
        names = ArrayHelper.Remove(p_idx, names);
        effectClips = ArrayHelper.Remove(p_idx, effectClips);

        if (names.Length <= 0) names = null;
        if (effectClips.Length <= 0) effectClips = null;
    }

    public override void ClearData()
    {
        foreach (EffectClip t_clip in effectClips) t_clip.ReleaseClip();

        effectClips = null;
        names = null;
    }

    public EffectClip GetCopyClip(int p_idx)
    {
        if (p_idx < 0 || p_idx >= effectClips.Length) return null;

        EffectClip t_origin = effectClips[p_idx];
        EffectClip t_clip = new EffectClip();

        t_clip.clipId = effectClips.Length;
        t_clip.effectType = t_origin.effectType;
        t_clip.effectName = t_origin.effectName;
        t_clip.effectPath = t_origin.effectPath;

        t_clip.PreLoad();
        return t_clip;
    }

    /// <summary>
    /// return the clip with preloading
    /// </summary>
    public EffectClip GetClip(int p_idx)
    {
        if (p_idx < 0 || p_idx >= effectClips.Length) return null;

        effectClips[p_idx].PreLoad();
        return effectClips[p_idx];
    }

    public override void CopyData(int p_idx)
    {
        names = ArrayHelper.Add(names[p_idx], names);
        effectClips = ArrayHelper.Add(GetCopyClip(p_idx), effectClips);
    }
}
