using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class QuestionBoard : MonoBehaviour
{
    #region Fields

    [Header("Data")]
    [SerializeField] public List<Question> Questions;
    [SerializeField] public List<PointPlank> Planks;
    [SerializeField] public List<Reward> PossibleRewards;

    private LinkedList<Question> QuestionChain;
    private LinkedListNode<Question> CurrentQuestion;
        
    [Header("UI")]
    [SerializeField] public TextMeshProUGUI PointsText;
    
    private int Points;
    
    [Header("UI - Screens")]
    [SerializeField] public RectTransform GreetingScreen;
    [SerializeField] public RectTransform QuestionScreen;
    [SerializeField] public RectTransform RewardScreen;
    [SerializeField] public RectTransform VictoryScreen;
    [SerializeField] public RectTransform DefeatScreen;

    [Header("UI - Greeting screen")]
    [SerializeField] public TextMeshProUGUI GreetingScreenText;
    [SerializeField] public Button ToQuestionScreenButton;

    [Header("UI - Question screen")]
    [SerializeField] public TextMeshProUGUI QuestionText;
    
    [SerializeField] public Button FirstAnswerButton;
    [SerializeField] public TextMeshProUGUI FirstAnswerText;

    [SerializeField] public Button SecondAnswerButton;
    [SerializeField] public TextMeshProUGUI SecondAnswerText;

    [SerializeField] public Button ThirdAnswerButton;
    [SerializeField] public TextMeshProUGUI ThirdAnswerText;

    [Header("UI - Reward screen")]
    [SerializeField] public TextMeshProUGUI RewardText;

    [SerializeField] public RectTransform FirstRewardPanel;
    [SerializeField] public Button FirstRewardButton;
    [SerializeField] public Image FirstRewardImage;
    [SerializeField] public TextMeshProUGUI FirstRewardPointsText;

    [SerializeField] public RectTransform SecondRewardPanel;
    [SerializeField] public Button SecondRewardButton;
    [SerializeField] public Image SecondRewardImage;
    [SerializeField] public TextMeshProUGUI SecondRewardPointsText;

    [SerializeField] public RectTransform ThirdRewardPanel;
    [SerializeField] public Button ThirdRewardButton;
    [SerializeField] public Image ThirdRewardImage;
    [SerializeField] public TextMeshProUGUI ThirdRewardPointsText;

    private List<Reward> _availableRewards;

    [Header("UI - Victory screen")]
    [SerializeField] public TextMeshProUGUI VictoryText;
    [SerializeField] public Image VictoryRewardImage;
    
    [Header("UI - Defeat screen")]
    [SerializeField] public TextMeshProUGUI DefeatText;

    [Header("UI - Other settings")]
    [SerializeField] private Color RightAnswerColor;
    [SerializeField] private Color WrongAnswerColor;
    [SerializeField] private Color DefaultAnswerColor;

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

        FirstAnswerButton.onClick.AddListener(() => AnswerQuestion(0, FirstAnswerButton));
        SecondAnswerButton.onClick.AddListener(() => AnswerQuestion(1, SecondAnswerButton));
        ThirdAnswerButton.onClick.AddListener(() => AnswerQuestion(2, ThirdAnswerButton));

        FirstRewardButton.onClick.AddListener(() => ChooseReward(0));
        SecondRewardButton.onClick.AddListener(() => ChooseReward(1));
        ThirdRewardButton.onClick.AddListener(() => ChooseReward(2));
    }

    public void OnDestroy()
    {
        ToQuestionScreenButton.onClick.RemoveAllListeners();

        FirstAnswerButton.onClick.RemoveAllListeners();
        SecondAnswerButton.onClick.RemoveAllListeners();
        ThirdAnswerButton.onClick.RemoveAllListeners();

        FirstRewardButton.onClick.RemoveAllListeners();
        SecondRewardButton.onClick.RemoveAllListeners();
        ThirdRewardButton.onClick.RemoveAllListeners();
    }

    #endregion

    #region Methods

    private void SetNewQuestion()
    {
        if(CurrentQuestion == null)
        {
            CurrentQuestion = QuestionChain.First;

            SetAnswersTexts();
        }
        else if(CurrentQuestion.Next == null)
        {
            PointPlank ConqueredPlank =
                new PointPlank()
                {
                    Points = -1,
                    Outcome = EGameOutcome.Defeat
                }; 

            foreach(var plank in Planks)
            {
                if(Points >= plank.Points &&
                    ConqueredPlank.Points < plank.Points)
                {
                    ConqueredPlank = plank;
                };
            };
            
            if (ConqueredPlank.Outcome == EGameOutcome.Victory)
            {
                RewardText.text = ConqueredPlank.RewardText;
                
                _availableRewards = PossibleRewards.FindAll(x => x.Points <= Points);

                if (ConqueredPlank.AbsolutePlank)
                {
                    _availableRewards = _availableRewards.FindAll(x => x.AbsoluteReward);
                };
                
                if(_availableRewards.Count >= 1)
                {
                    FirstRewardPanel.gameObject.SetActive(true);

                    FirstRewardImage.sprite     = _availableRewards[0].Icon;
                    FirstRewardPointsText.text  = _availableRewards[0].PointsText;
                }
                else
                {
                    FirstRewardPanel.gameObject.SetActive(false);
                };

                if (_availableRewards.Count >= 2)
                {
                    SecondRewardPanel.gameObject.SetActive(true);

                    SecondRewardImage.sprite    = _availableRewards[1].Icon;
                    SecondRewardPointsText.text = _availableRewards[1].PointsText;
                }
                else
                {
                    SecondRewardPanel.gameObject.SetActive(false);
                };

                if (_availableRewards.Count >= 3)
                {
                    ThirdRewardPanel.gameObject.SetActive(true);

                    ThirdRewardImage.sprite     = _availableRewards[2].Icon;
                    ThirdRewardPointsText.text  = _availableRewards[2].PointsText;
                }
                else
                {
                    ThirdRewardPanel.gameObject.SetActive(false);
                };

                VictoryText.text = ConqueredPlank.OutcomeText;
                
                ShowScreen(rewardScreen : true);
            }
            else
            {
                DefeatText.text = ConqueredPlank.OutcomeText;

                ShowScreen(defeatScreen : true);
            };
        }
        else
        {
            CurrentQuestion = CurrentQuestion.Next;

            SetAnswersTexts();
        };
    }

    private async void AnswerQuestion(int answerIndex, Button button)
    {
        var buttonImage = button.gameObject.GetComponent<Image>();

        Color setColor;

        if (CurrentQuestion.Value.Answers[answerIndex].Correct)
        {
            Points += CurrentQuestion.Value.Reward;

            PointsText.text = Points.ToString();

            setColor = RightAnswerColor;
        }
        else
        {
            setColor = WrongAnswerColor;
        };

        FirstAnswerButton.enabled   = false;
        SecondAnswerButton.enabled  = false;
        ThirdAnswerButton.enabled   = false;
        
        for (int i = 0; i < 6; i++)
        {
            if(buttonImage.color == setColor)
            {
                buttonImage.color = DefaultAnswerColor;
            }
            else
            {
                buttonImage.color = setColor;
            };

            await Task.Delay(500);
        };

        buttonImage.color = DefaultAnswerColor;
        
        FirstAnswerButton.enabled   = true;
        SecondAnswerButton.enabled  = true;
        ThirdAnswerButton.enabled   = true;
        
        SetNewQuestion();
    }

    private void ChooseReward(int rewardIndex)
    {
        VictoryRewardImage.sprite = _availableRewards[rewardIndex].Icon;

        ShowScreen(victoryScreen : true);
    }

    private void SetAnswersTexts()
    {
        QuestionText.text       = CurrentQuestion.Value.Text;

        FirstAnswerText.text    = CurrentQuestion.Value.Answers[0].Text;
        SecondAnswerText.text   = CurrentQuestion.Value.Answers[1].Text;
        ThirdAnswerText.text    = CurrentQuestion.Value.Answers[2].Text;
    }

    private void ShowScreen(
        bool greetingScreen = false,
        bool questionScreen = false,
        bool rewardScreen = false,
        bool victoryScreen = false,
        bool defeatScreen = false)
    {
        GreetingScreen.gameObject.SetActive(greetingScreen);
        QuestionScreen.gameObject.SetActive(questionScreen);
        RewardScreen.gameObject.SetActive(rewardScreen);
        VictoryScreen.gameObject.SetActive(victoryScreen);
        DefeatScreen.gameObject.SetActive(defeatScreen);
    }

    #endregion
}
