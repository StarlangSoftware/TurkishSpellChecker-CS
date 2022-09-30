using Dictionary.Dictionary;

namespace SpellChecker
{
    public class Candidate : Word
    {
        private Operator _operator;

        public Candidate(string candidate, Operator _operator) : base(candidate){
            this._operator = _operator;
        }

        public Operator GetOperator() {
            return _operator;
        }
    }
}