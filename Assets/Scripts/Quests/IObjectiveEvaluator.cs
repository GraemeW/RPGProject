using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public interface IObjectiveEvaluator
    {
        bool Evaluate();
    }
}
