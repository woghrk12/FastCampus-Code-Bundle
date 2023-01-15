using System;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using UnityEngine;
using TMPro;

public class UserScoreArgs : EventArgs
{
    public UserScore score;
    public string message;

    public UserScoreArgs(UserScore p_score, string p_message)
    {
        score = p_score;
        message = p_message;
    }
}

public class LeaderboardArgs : EventArgs
{
    public DateTime startDate;
    public DateTime endDate;

    public List<UserScore> scores;
}

public class LeaderboardHandler : MonoBehaviour
{
    private bool isReadyToInitialize = false;
    private bool isInitiailized = false;
    private bool isAddingUserScore = false;
    private bool isUpdatingUserScore = false;
    private bool isRetrievingUserScore = false;
    private bool isGetUserScoreCallQueued = false;

    private DatabaseReference databaseRef;

    public bool isSendInitializedEvent = false;
    public event EventHandler OnInitialized;

    private bool isSendAddedScoreEvent = false;
    private UserScoreArgs addedScoreArgs;
    public event EventHandler<UserScoreArgs> OnAddedScore;

    private bool isSendUpdatedScoreEvent = false;
    private UserScoreArgs updatedScoreArgs;
    public event EventHandler<UserScoreArgs> OnUpdatedScore;

    private bool isSendRetrievedScoreEvent = false;
    private UserScoreArgs retrivedScoreArgs;
    public event EventHandler<UserScoreArgs> OnRetrivedScore;

    private List<UserScore> topScores = new List<UserScore>();
    public List<UserScore> TopScores => topScores;

    private Dictionary<string, UserScore> userScores = new Dictionary<string, UserScore>();

    private Query currentNewScoreQuery;

    private bool isSendUpdatedLeaderboardEvent = false;
    public event EventHandler<LeaderboardArgs> OnUpdatedLeaderboard;

    private bool isGettingTopScore = false;
    private int scoresToRetrieve = 20;

    
    public TMP_InputField userIdInputField;
    public TMP_InputField userNameInputField;
    public TMP_InputField scoreInputField;
    public TMP_Text outputText;

    public DateTime startDateTime;
    private long StartTimeTicks => startDateTime.Ticks / TimeSpan.TicksPerSecond;
    public DateTime endDateTime;
    private long EndTimeTicks
    {
        get 
        {
            long t_endTimeTicks = endDateTime.Ticks / TimeSpan.TicksPerSecond;
            if (t_endTimeTicks <= 0) t_endTimeTicks = DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond;
            return t_endTimeTicks;
        }
    }

    public bool TaskProcessing { get { return isReadyToInitialize || isGettingTopScore || isRetrievingUserScore || isAddingUserScore; } }

    public string AllScoreDataPath => "all_scores";

    private void Start()
    {
        FirebaseInitializer.Initialize(t_dependencyStatus =>
        {
            if (t_dependencyStatus == Firebase.DependencyStatus.Available)
            {
                isReadyToInitialize = true;
                InitializeDatabase();
            }
        });
    }

    private void Update()
    {
        if (isSendAddedScoreEvent)
        {
            isSendAddedScoreEvent = false;
            OnAddedScore(this, addedScoreArgs);
        }

        if (isSendUpdatedScoreEvent)
        {
            isSendUpdatedScoreEvent = false;
            OnUpdatedScore(this, updatedScoreArgs);
        }

        if (isSendRetrievedScoreEvent)
        {
            isSendRetrievedScoreEvent = false;
            OnRetrivedScore(this, retrivedScoreArgs);
        }

        if (isSendUpdatedLeaderboardEvent)
        {
            isSendUpdatedLeaderboardEvent = false;
            OnUpdatedLeaderboard(this, new LeaderboardArgs { scores = topScores, startDate = startDateTime, endDate = endDateTime }) ;
        }
    }

    private void InitializeDatabase()
    {
        if (isInitiailized) return;

        FirebaseApp t_app = FirebaseApp.DefaultInstance;
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        isInitiailized = true;
        isReadyToInitialize = false;

        OnInitialized(this, null);
    }

    public Task AddScore(string p_userId, string p_userName, int p_score, long p_timestamp = 1L, Dictionary<string, object> p_otherData = null)
    {
        if (p_timestamp <= 0) p_timestamp = DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond;

        var t_userScore = new UserScore(p_userId, p_userName, p_score, p_timestamp, p_otherData);
        return AddScore(t_userScore);
    }

    public Task<UserScore> AddScore(UserScore p_userScore)
    {
        if (isAddingUserScore) 
        {
            Debug.LogError("Running add user score task!");
            return null;
        }

        var t_scoreDictionary = p_userScore.ToDictionary();
        isAddingUserScore = true;

        return Task.Run(() => {
            var t_newEntry = databaseRef.Child(AllScoreDataPath).Push();

            return t_newEntry.SetValueAsync(t_scoreDictionary).ContinueWith(t_task => {
                if (t_task.Exception != null) throw t_task.Exception;
                if (!t_task.IsCompleted) return null;
                
                isAddingUserScore = false;

                addedScoreArgs = new UserScoreArgs(p_userScore, p_userScore.userId + " Added!");
                isSendAddedScoreEvent = true;

                return p_userScore;
            }).Result;
        });
    }

    public Task UpdateScore(string p_userId, string p_userName, int p_score, long p_timestamp = 1L, Dictionary<string, object> p_otherData = null)
    {
        if (p_timestamp <= 0) p_timestamp = DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond;

        var t_userScore = new UserScore(p_userId, p_userName, p_score, p_timestamp, p_otherData);
        return UpdateScore(t_userScore);
    }

    public Task<UserScore> UpdateScore(UserScore p_userScore)
    {
        if (isUpdatingUserScore)
        {
            Debug.LogError("Running update user score task!");
            return null;
        }

        var t_scoreDictionary = p_userScore.ToDictionary();
        isUpdatingUserScore = true;

        return Task.Run(() => {
            var t_newEntry = databaseRef.Child(AllScoreDataPath).Push();

            return t_newEntry.SetValueAsync(t_scoreDictionary).ContinueWith(t_task => {
                if (t_task.Exception != null) throw t_task.Exception;
                if (!t_task.IsCompleted) return null;

                isAddingUserScore = false;

                addedScoreArgs = new UserScoreArgs(p_userScore, p_userScore.userId + " Added!");
                isSendAddedScoreEvent = true;

                return p_userScore;
            }).Result;
        });
    }

    public void GetUserScore(string p_userId)
    {
        if (!isInitiailized && !isGetUserScoreCallQueued)
        {
            isGetUserScoreCallQueued = true;
            StartCoroutine(GetUserScoreWhenInitialized(p_userId));
            return;
        }
        if (isGetUserScoreCallQueued) return;

        isRetrievingUserScore = true;

        databaseRef.Child(AllScoreDataPath).OrderByChild(UserScore.userIdPath).StartAt(p_userId).EndAt(p_userId).GetValueAsync().ContinueWith(t_task => {
            if (t_task.Exception != null) throw t_task.Exception;
            if (!t_task.IsCompleted) return;

            if (t_task.Result.ChildrenCount <= 0)
            {
                retrivedScoreArgs = new UserScoreArgs(null, string.Format("No Scores for User {0}", p_userId));
            }
            else
            {
                var t_scores = ParseValiduserScoreRecords(t_task.Result, -1, -1).ToList();

                if (t_scores.Count <= 0)
                {
                    retrivedScoreArgs = new UserScoreArgs(null, string.Format(
                        "No Scores for User {0} within time range ({1} - {2})", 
                        p_userId, 
                        startDateTime, 
                        endDateTime));
                }
                else
                {
                    var t_orderdScore = t_scores.OrderBy(t_score => t_score.score);
                    var t_userScore = t_orderdScore.Last();

                    retrivedScoreArgs = new UserScoreArgs(t_userScore, t_userScore.userId + " Retrived!");
                }
            }

            isRetrievingUserScore = false;
            isSendAddedScoreEvent = true;
        });
    }

    private List<UserScore> ParseValiduserScoreRecords(DataSnapshot p_snapshot, long p_startTS, long p_endTS)
    {
        return p_snapshot.Children.Select(t_scoreRecord => UserScore.CreateScoreFromRecord(t_scoreRecord))
            .Where(t_score => t_score != null && t_score.timestamp > p_startTS && t_score.timestamp <= p_endTS)
            .Reverse()
            .ToList();
    }

    private IEnumerator GetUserScoreWhenInitialized(string p_userId)
    {
        while (!isInitiailized) yield return null;

        isGetUserScoreCallQueued = false;
        GetUserScore(p_userId);
    }

    private void GetInitialTopScores(long p_batchEnd)
    {
        isGettingTopScore = true;

        var t_query = databaseRef.Child(AllScoreDataPath).OrderByChild(UserScore.scorePath);
        t_query = t_query.LimitToLast(scoresToRetrieve);

        t_query.GetValueAsync().ContinueWith(t_task => {
            if (t_task.Exception != null)
            {
                SetTopScores();
                return;
            }
            if (!t_task.IsCompleted) return;
            if (!t_task.Result.HasChildren)
            {
                SetTopScores();
                return;
            }

            var t_scores = ParseValiduserScoreRecords(t_task.Result, StartTimeTicks, EndTimeTicks);
            foreach (var t_score in t_scores)
            {
                if (!userScores.ContainsKey(t_score.userId)) userScores[t_score.userId] = t_score;
                else
                {
                    var t_bestScore = GetBestScore(userScores[t_score.userId], t_score);
                    userScores[t_score.userId] = t_bestScore;
                }
                if (userScores.Count >= scoresToRetrieve)
                {
                    SetTopScores();
                    return;
                }
            }

            long t_nextEndAt = t_scores.First().score + 1L;
            try
            {
                GetInitialTopScores(t_nextEndAt);
            }
            catch (Exception t_ex)
            {
                Debug.LogError(t_ex);
            }
            finally
            {
                SetTopScores();
            }
        });
    }

    private UserScore GetBestScore(params UserScore[] p_scores)
    {
        if (p_scores.Length <= 0) return null;

        UserScore t_bestScore = null;
        foreach (var t_score in p_scores)
        {
            if (t_bestScore == null) t_bestScore = t_score;
            else if (t_bestScore.score < t_score.score) t_bestScore = t_score;
        }
        return t_bestScore;
    }

    private void SetTopScores()
    {
        topScores.Clear();

        topScores.AddRange(userScores.Values.OrderByDescending(t_score => t_score.score));

        currentNewScoreQuery = databaseRef.Child(AllScoreDataPath).OrderByChild(UserScore.scorePath);

        if (topScores.Count > 0) currentNewScoreQuery = currentNewScoreQuery.EndAt(topScores.Last().score);

        isSendUpdatedLeaderboardEvent = true;
        isGettingTopScore = false;
    }

    public void AddUserScore() => AddScore(userIdInputField.text, userNameInputField.text, int.Parse(scoreInputField.text));
    public void UpdateUserScore() => UpdateScore(userIdInputField.text, userNameInputField.text, int.Parse(scoreInputField.text));
    public void GetUserScore() => GetUserScore(userIdInputField.text);
    public void RefreshScore()
    {
        if (isInitiailized) GetInitialTopScores(Int64.MinValue);
    }
}
