using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LeaderboardUIController : MonoBehaviour
{
    public LeaderboardHandler leaderboardHandler;
    public TMP_Text outputText;

    private enum ETopScoreElement { USERNAME = 1, TIMESTAMP = 2, SCORE = 3 };

    public int maxRetrievableScores = 100;
    public RectTransform scoreContentContainer;
    public GameObject scorePrefab;

    private List<GameObject> scoreObjects = new List<GameObject>();

    private void Start()
    {
        StartCoroutine(CreateTopscorePrefabs());
    }

    private void OnEnable()
    {
        leaderboardHandler.OnAddedScore += OnAddedUserScore;
        leaderboardHandler.OnUpdatedScore += OnUpdateUserScore;
        leaderboardHandler.OnRetrivedScore += OnRetrievedUserScore;
        leaderboardHandler.OnUpdatedLeaderboard += OnUpdatedLeaderboard;
    }

    private void OnDisable()
    {
        leaderboardHandler.OnAddedScore -= OnAddedUserScore;
        leaderboardHandler.OnUpdatedScore -= OnUpdateUserScore;
        leaderboardHandler.OnRetrivedScore -= OnRetrievedUserScore;
        leaderboardHandler.OnUpdatedLeaderboard -= OnUpdatedLeaderboard;
    }

    private void OnAddedUserScore(object p_sender, UserScoreArgs p_scoreArgs)
    {
        outputText.text = p_scoreArgs.message;
    }
    private void OnUpdateUserScore(object p_sender, UserScoreArgs p_scoreArgs)
    {
        outputText.text = p_scoreArgs.message;
    }
    private void OnRetrievedUserScore(object p_sender, UserScoreArgs p_scoreArgs)
    {
        outputText.text = p_scoreArgs.message;
    }
    private void OnUpdatedLeaderboard(object p_sender, LeaderboardArgs p_leaderboardArgs)
    {
        var t_scores = p_leaderboardArgs.scores;
        for (int i = 0; i < Mathf.Min(t_scores.Count, scoreObjects.Count); i++)
        {
            var t_score = t_scores[i];
            var t_scoreObject = scoreObjects[i];
            t_scoreObject.SetActive(true);

            var t_textElements = t_scoreObject.GetComponentsInChildren<TMP_Text>();
            t_textElements[(int)ETopScoreElement.USERNAME].text = string.IsNullOrEmpty(t_score.userName) ? t_score.userId : t_score.userName;
            t_textElements[(int)ETopScoreElement.TIMESTAMP].text = t_score.ShortDateString;
            t_textElements[(int)ETopScoreElement.SCORE].text = t_score.score.ToString();
        }

        for (int i = t_scores.Count; i < scoreObjects.Count; i++)
            scoreObjects[i].SetActive(false);
    }

    private IEnumerator CreateTopscorePrefabs()
    {
        var t_textElements = scorePrefab.GetComponentsInChildren<TMP_Text>();
        var t_topScoreElementValues = Enum.GetValues(typeof(ETopScoreElement));
        var lastTopScoreElementValue = (int)t_topScoreElementValues.GetValue(t_topScoreElementValues.Length - 1);
        if (t_textElements.Length < lastTopScoreElementValue)
        {
            throw new InvalidOperationException(String.Format(
                "At least {0} Text components must be present on TopScorePrefab. Found {1}",
                lastTopScoreElementValue,
                t_textElements.Length));
        }

        for (int i = 0; i < maxRetrievableScores; i++)
        {
            GameObject scoreObject = Instantiate(scorePrefab, scoreContentContainer.transform);
            scoreObject.GetComponentInChildren<TMP_Text>().text = (i + 1).ToString();
            scoreObject.name = "Leaderboard Score Record " + i;
            scoreObject.SetActive(false);

            scoreObjects.Add(scoreObject);

            yield return null;
        }
    }
}
