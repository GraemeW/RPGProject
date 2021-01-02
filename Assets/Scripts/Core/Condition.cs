using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    [System.Serializable]
    public class Condition
    {
        [SerializeField] string predicate;
        [SerializeField] string[] parameters;

        public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
        {
            if (string.IsNullOrWhiteSpace(predicate)) { return true; }

            foreach (IPredicateEvaluator evaluator in evaluators)
            {
                bool? result = evaluator.Evaluate(predicate, parameters);

                if (result == null) { continue; }
                if (result == false) { return false; }
            }

            return true;
        }
    }

}
