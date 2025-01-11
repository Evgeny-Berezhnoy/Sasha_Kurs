using System;
using UnityEngine;

[Serializable]
public class Reward
{
    #region Fields

    [SerializeField] private Sprite _icon;
    [SerializeField] private int _points;
    [SerializeField] private string _pointsText;
    [SerializeField] private string _rewardText;
    [SerializeField] private bool _absoluteReward;

    #endregion

    #region Properties

    public Sprite Icon => _icon;
    public int Points => _points;
    public string PointsText => _pointsText;
    public string RewardText => _rewardText;
    public bool AbsoluteReward => _absoluteReward;

    #endregion
}