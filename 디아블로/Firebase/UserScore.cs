using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;

[Serializable]
public class UserScore
{
    public static string userIdPath = "user_id";
    public static string userNamePath = "user_name";
    public static string scorePath = "score";
    public static string timestampPath = "timestamp";
    public static string otherDataPath = "data";

    public string userId;
    public string userName;
    public long score;
    public long timestamp;
    public Dictionary<string, object> otherData;

    public string ShortDateString
    {
        get 
        {
            var t_scoreData = new DateTimeOffset(new DateTime(timestamp * TimeSpan.TicksPerSecond, DateTimeKind.Utc)).LocalDateTime;
            return t_scoreData.ToShortDateString() + " " + t_scoreData.ToShortTimeString();
        }
    }

    // Data from unity editor
    public UserScore(string p_userId, string p_userName, long p_score, long p_timestamp, Dictionary<string, object> p_otherData = null)
    {
        userId = p_userId;
        userName = p_userName;
        score = p_score;
        timestamp = p_timestamp;
        otherData = p_otherData;
    }

    // Data from firebase
    public UserScore(DataSnapshot p_record)
    {
        userId = p_record.Child(userIdPath).Value.ToString();
        if (p_record.Child(userNamePath).Exists)
        {
            userName = p_record.Child(userNamePath).Value.ToString();
        }

        long t_score = Int64.MinValue;
        if (Int64.TryParse(p_record.Child(scorePath).Value.ToString(), out t_score))
            score = t_score;

        long t_timestamp = Int64.MinValue;
        if (Int64.TryParse(p_record.Child(timestampPath).Value.ToString(), out t_timestamp))
            timestamp = t_timestamp;

        if (p_record.Child(otherDataPath).Exists && p_record.Child(otherDataPath).HasChildren)
        {
            otherData = new Dictionary<string, object>();

            foreach (var t_keyValue in p_record.Child(otherDataPath).Children)
                otherData[t_keyValue.Key] = t_keyValue.Value;
        }
    }

    public static UserScore CreateScoreFromRecord(DataSnapshot p_record)
    {
        if (p_record == null) 
        {
            Debug.LogWarning("Null DataSnapshot record in UserScore.CreateScoreFromRecord");
            return null;
        }

        if (p_record.Child(userIdPath).Exists && p_record.Child(scorePath).Exists && p_record.Child(timestampPath).Exists)
        {
            return new UserScore(p_record);
        }

        Debug.LogWarning("Invalid record format in UserScore.CreateScoreFromRecord");
        return null;
    }

    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>() {
            { userIdPath, userId },
            { userNamePath, userName },
            { scorePath, score },
            { timestampPath, timestamp },
            { otherDataPath, otherData }
        };
    }
}