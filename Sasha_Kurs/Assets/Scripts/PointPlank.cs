using System;
using UnityEngine;

[Serializable]
public class PointPlank
{
    #region Fields

    [SerializeField] public int Points;
    [SerializeField] public string RewardText;
    [SerializeField] public string OutcomeText;
    [SerializeField] public EGameOutcome Outcome;
    [SerializeField] public bool AbsolutePlank;

    #endregion
}