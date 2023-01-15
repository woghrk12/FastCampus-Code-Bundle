using System;
using System.IO;
using System.Xml;
using UnityEngine;

public class SoundData : BaseData
{
    public SoundClip[] soundClips = new SoundClip[0];

    private string clipPath = "Sounds/";
    private string xmlFilePath = "";
    private string xmlFileName = "soundData.xml";
    private string dataPath = "Data/soundData";

    private const string SOUND = "sound";
    private const string CLIP = "clip";

    public SoundData() { }

    public void LoadData()
    {
        xmlFilePath = Application.dataPath + dataDirectory;
        TextAsset t_asset = (TextAsset)ResourceManager.Load(dataPath);
        if (t_asset == null || t_asset.text == null)
        {
            AddData("New Sound");
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
                            soundClips = new SoundClip[t_length];
                            break;

                        case "id":
                            t_curID = int.Parse(t_reader.ReadString());
                            soundClips[t_curID] = new SoundClip();
                            soundClips[t_curID].clipId = t_curID;
                            break;

                        case "name":
                            names[t_curID] = t_reader.ReadString();
                            break;

                        case "loops":
                            int t_cnt = int.Parse(t_reader.ReadString());
                            soundClips[t_curID].checkTime = new float[t_cnt];
                            soundClips[t_curID].setTime = new float[t_cnt];
                            break;

                        case "maxVol":
                            soundClips[t_curID].maxVolume = float.Parse(t_reader.ReadString());
                            break;

                        case "pitch":
                            soundClips[t_curID].pitch = float.Parse(t_reader.ReadString());
                            break;

                        case "dopplerLevel":
                            soundClips[t_curID].dopplerLevel = float.Parse(t_reader.ReadString());
                            break;

                        case "rollOffMode":
                            soundClips[t_curID].rollOffMode = (AudioRolloffMode)Enum.Parse(typeof(AudioRolloffMode), t_reader.ReadString());
                            break;

                        case "minDist":
                            soundClips[t_curID].minDist = float.Parse(t_reader.ReadString());
                            break;

                        case "maxDist":
                            soundClips[t_curID].maxDist = float.Parse(t_reader.ReadString());
                            break;

                        case "spatialBlend":
                            soundClips[t_curID].spatialBlend = float.Parse(t_reader.ReadString());
                            break;

                        case "loop":
                            soundClips[t_curID].isLoop = true;
                            break;

                        case "clipPath":
                            soundClips[t_curID].clipPath = t_reader.ReadString();
                            break;

                        case "clipName":
                            soundClips[t_curID].clipName = t_reader.ReadString();
                            break;

                        case "checkTime":
                            SetLoopTime(true, soundClips[t_curID], t_reader.ReadString());
                            break;

                        case "setTime":
                            SetLoopTime(false, soundClips[t_curID], t_reader.ReadString());
                            break;

                        case "type":
                            soundClips[t_curID].playType = (ESoundPlayType)Enum.Parse(typeof(ESoundPlayType), t_reader.ReadString());
                            break;
                    }
                }
            }
        }

        foreach (SoundClip t_clip in soundClips) t_clip.PreLoad();
    }

    private void SetLoopTime(bool p_isCheck, SoundClip p_clip, string p_timeString)
    {
        string[] t_time = p_timeString.Split('/');
        for (int i = 0; i < t_time.Length; i++)
        {
            if (t_time[i] != string.Empty)
            {
                if (p_isCheck) p_clip.checkTime[i] = float.Parse(t_time[i]);
                else p_clip.setTime[i] = float.Parse(t_time[i]);
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
            t_writer.WriteStartElement(SOUND);
            t_writer.WriteElementString("length", GetDataCount().ToString());
            t_writer.WriteWhitespace("\n");

            int t_length = names != null ? names.Length : 0;

            for (int i = 0; i < t_length; i++)
            {
                SoundClip t_clip = soundClips[i];
                t_writer.WriteStartElement(CLIP);
                t_writer.WriteElementString("id", i.ToString());
                t_writer.WriteElementString("name", names[i]);
                t_writer.WriteElementString("loops", t_clip.checkTime.Length.ToString());
                t_writer.WriteElementString("maxVol", t_clip.maxVolume.ToString());
                t_writer.WriteElementString("pitch", t_clip.pitch.ToString());
                t_writer.WriteElementString("dopplerLevel", t_clip.dopplerLevel.ToString());
                t_writer.WriteElementString("rollOffMode", t_clip.rollOffMode.ToString());
                t_writer.WriteElementString("minDist", t_clip.minDist.ToString());
                t_writer.WriteElementString("maxDist", t_clip.maxDist.ToString());
                t_writer.WriteElementString("spatialBlend", t_clip.spatialBlend.ToString());
                if(t_clip.isLoop) t_writer.WriteElementString("loop", "true");
                t_writer.WriteElementString("clipPath", t_clip.clipPath);
                t_writer.WriteElementString("clipName", t_clip.clipName);
                
                string t_str = "";
                foreach (float t_checkTime in t_clip.checkTime) t_str += t_checkTime.ToString() + "/";
                t_writer.WriteElementString("checkTime", t_str);

                t_str = "";
                foreach (float t_setTime in t_clip.setTime) t_str += t_setTime.ToString() + "/";
                t_writer.WriteElementString("setTime", t_str);

                t_writer.WriteElementString("type", t_clip.playType.ToString());

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
            soundClips = new SoundClip[] { new SoundClip() };
        }
        else
        {
            names = ArrayHelper.Add(p_newName, names);
            soundClips = ArrayHelper.Add(new SoundClip(), soundClips);
        }
    }

    public override void RemoveData(int p_idx)
    {
        names = ArrayHelper.Remove(p_idx, names);
        soundClips = ArrayHelper.Remove(p_idx, soundClips);

        if (names.Length <= 0) names = null;
        if (soundClips.Length <= 0) soundClips = null;
    }

    public override void ClearData()
    {
        foreach (SoundClip t_clip in soundClips) t_clip.ReleaseClip();

        soundClips = null;
        names = null;
    }

    public SoundClip GetCopyClip(int p_idx)
    {
        if (p_idx < 0 || p_idx >= soundClips.Length) return null;

        SoundClip t_origin = soundClips[p_idx];
        SoundClip t_clip = new SoundClip();

        t_clip.clipId = p_idx;
        t_clip.clipPath = t_origin.clipPath;
        t_clip.clipName = t_origin.clipName;
        t_clip.maxVolume = t_origin.maxVolume;
        t_clip.pitch = t_origin.pitch;
        t_clip.dopplerLevel = t_origin.dopplerLevel;
        t_clip.rollOffMode = t_origin.rollOffMode;
        t_clip.minDist = t_origin.minDist;
        t_clip.maxDist = t_origin.maxDist;
        t_clip.spatialBlend = t_origin.spatialBlend;
        t_clip.isLoop = t_origin.isLoop;
        
        t_clip.checkTime = new float[t_origin.checkTime.Length];
        t_clip.setTime = new float[t_origin.setTime.Length];

        for (int i = 0; i < t_clip.checkTime.Length; i++)
        {
            t_clip.checkTime[i] = t_origin.checkTime[i];
            t_clip.setTime[i] = t_origin.setTime[i];
        }

        t_clip.playType = t_origin.playType;

        t_clip.PreLoad();
        return t_clip;
    }

    public SoundClip GetClip(int p_idx)
    {
        if (p_idx < 0 || p_idx >= soundClips.Length) return null;

        soundClips[p_idx].PreLoad();
        return soundClips[p_idx];
    }

    public override void CopyData(int p_idx)
    {
        names = ArrayHelper.Add(names[p_idx], names);
        soundClips = ArrayHelper.Add(GetCopyClip(p_idx), soundClips);
    }

}
