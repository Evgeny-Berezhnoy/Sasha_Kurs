using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Question
{
    #region Fields

    [SerializeField] public string Text;
    [SerializeField] public List<Answer> Answers = new List<Answer>();
    [SerializeField] public int Reward;

    #endregion
}