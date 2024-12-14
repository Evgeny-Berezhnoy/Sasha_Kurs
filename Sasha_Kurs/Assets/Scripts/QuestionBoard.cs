using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionBoard : MonoBehaviour
{
    #region Fields

    [Header("Data")]
    [SerializeField] public List<Question> Questions;
    [SerializeField] public int PointsToWin;

    private LinkedList<Question> QuestionChain;
    private LinkedListNode<Question> CurrentQuestion;

    [Header("UI")]
    [SerializeField] public TextMeshProUGUI PointsText;
    
    [Header("UI - Screens")]
    [SerializeField] public RectTransform GreetingScreen;
    [SerializeField] public RectTransform QuestionScreen;
    [SerializeField] public RectTransform VictoryScreen;
    [SerializeField] public RectTransform DefeatScreen;

    [Header("UI - Greeting Board")]
    [SerializeField] public TextMeshProUGUI GreetingScreenText;
    [SerializeField] public Button ToQuestionScreenButton;

    [Header("UI - Main board")]
    [SerializeField] public TextMeshProUGUI QuestionText;
    
    [SerializeField] public Button FirstAnswerButton;
    [SerializeField] public TextMeshProUGUI FirstAnswerText;
    
    [SerializeField] public Button SecondAnswerButton;
    [SerializeField] public TextMeshProUGUI SecondAnswerText;
    
    [SerializeField] public Button ThirdAnswerButton;
    [SerializeField] public TextMeshProUGUI ThirdAnswerText;

    private int Points;

    #endregion

    #region Unity events

    public void Start()
    {
        QuestionChain = new LinkedList<Question>(Questions);

        Points = 0;

        ShowScreen(greetingScreen : true);

        SetNewQuestion();

        PointsText.text = Points.ToString();

        ToQuestionScreenButton.onClick.AddListener(() => ShowScreen(questionScreen : true));
        FirstAnswerButton.onClick.AddListener(() => AnswerQuestion(0));
        SecondAnswerButton.onClick.AddListener(() => AnswerQuestion(1));
        ThirdAnswerButton.onClick.AddListener(() => AnswerQuestion(2));
    }

    public void OnDestroy()
    {
        ToQuestionScreenButton.onClick.RemoveAllListeners();
        FirstAnswerButton.onClick.RemoveAllListeners();
        SecondAnswerButton.onClick.RemoveAllListeners();
        ThirdAnswerButton.onClick.RemoveAllListeners();
    }

    #endregion

    #region Methods

    private void SetNewQuestion()
    {
        if(CurrentQuestion == null)
        {
            CurrentQuestion = QuestionChain.First;
        }
        else if(CurrentQuestion.Next == null)
        {
            if(Points >= PointsToWin)
            {
                ShowScreen(victoryScreen : true);
            }
            else
            {
                ShowScreen(defeatScreen : true);
            };

            return;
        }
        else
        {
            CurrentQuestion = CurrentQuestion.Next;
        };

        QuestionText.text = CurrentQuestion.Value.Text;

        FirstAnswerText.text = CurrentQuestion.Value.Answers[0].Text;
        SecondAnswerText.text = CurrentQuestion.Value.Answers[1].Text;
        ThirdAnswerText.text = CurrentQuestion.Value.Answers[2].Text;
    }

    private void AnswerQuestion(int answerIndex)
    {
        if (CurrentQuestion.Value.Answers[answerIndex].Correct)
        {
            Points += CurrentQuestion.Value.Reward;

            PointsText.text = Points.ToString();
        };
        
        SetNewQuestion();
    }

    private void ShowScreen(bool greetingScreen = false, bool questionScreen = false, bool victoryScreen = false, bool defeatScreen = false)
    {
        GreetingScreen.gameObject.SetActive(greetingScreen);
        QuestionScreen.gameObject.SetActive(questionScreen);
        VictoryScreen.gameObject.SetActive(victoryScreen);
        DefeatScreen.gameObject.SetActive(defeatScreen);
    }

    #endregion
}
