using Dictionary.Dictionary;

namespace SpellChecker
{
    public class Candidate : Word
    {
        private Operator _operator;
        
        /// <summary>
        /// Constructs a new Candidate object with the specified candidate and operator.
        /// </summary>
        /// <param name="candidate">The word candidate to be checked for spelling.</param>
        /// <param name="operator">The operator to be applied to the candidate in the spell checking process.</param>
        public Candidate(string candidate, Operator _operator) : base(candidate){
            this._operator = _operator;
        }
        
        /// <summary>
        /// Returns the operator associated with this candidate.
        /// </summary>
        /// <returns>The operator associated with this candidate.</returns>
        public Operator GetOperator() {
            return _operator;
        }
    }
}