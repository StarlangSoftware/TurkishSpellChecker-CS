namespace SpellChecker
{
    public class SpellCheckerParameter
    {
        private double _threshold = 0.0;
        private bool _suffixCheck = true;
        private bool _rootNGram = true;
        private int _minWordLength = 4;
        private string _domain = null;
        
        /// <summary>
        /// Constructs a SpellCheckerParameter object with default values.
        /// The default threshold is 0.0, the suffix check is enabled, the root ngram is enabled,
        /// the minimum word length is 4, and domain name value is null.
        /// </summary>
        public SpellCheckerParameter()
        {
        }
        
        /// <summary>
        /// Sets the threshold value used in calculating the n-gram probabilities.
        /// </summary>
        /// <param name="threshold">the threshold for the spell checker</param>
        public void SetThreshold(double threshold)
        {
            _threshold = threshold;
        }
        
        /// <summary>
        /// Enables or disables suffix check for the spell checker.
        /// </summary>
        /// <seealso cref="SimpleSpellChecker.ForcedDeDaSplitCheck(Word, Sentence)"/>
        /// <seealso cref="SimpleSpellChecker.ForcedQuestionSuffixSplitCheck(Word, Sentence)"/>
        /// <seealso cref="SimpleSpellChecker.ForcedSuffixSplitCheck(Word, Sentence)"/>
        /// <param name="suffixCheck">a boolean indicating whether the suffix check should be enabled (true) or disabled (false)</param>
        public void SetSuffixCheck(bool suffixCheck)
        {
            _suffixCheck = suffixCheck;
        }
        
        /// <summary>
        /// Enables or disables the root n-gram for the spell checker.
        /// </summary>
        /// <param name="rootNGram">a boolean indicating whether the root n-gram should be enabled (true) or disabled (false)</param>
        public void SetRootNGram(bool rootNGram)
        {
            _rootNGram = rootNGram;
        }
        
        /// <summary>
        /// Sets the minimum length of words viable for spell checking.
        /// </summary>
        /// <param name="minWordLength">the minimum word length for the spell checker</param>
        public void SetMinWordLength(int minWordLength)
        {
            _minWordLength = minWordLength;
        }
        
        /// <summary>
        /// Sets the domain name to the specified value.
        /// </summary>
        /// <param name="domain">the new domain name to set for this object</param>
        public void SetDomain(string domain)
        {
            _domain = domain;
        }
        
        /// <summary>
        /// Returns the threshold value used in calculating the n-gram probabilities.
        /// </summary>
        /// <returns>the threshold for the spell checker</returns>
        public double GetThreshold()
        {
            return _threshold;
        }
        
        /// <summary>
        /// Returns whether suffix check is enabled for the spell checker.
        /// </summary>
        /// <seealso cref="SimpleSpellChecker.ForcedDeDaSplitCheck(Word, Sentence)"/>
        /// <seealso cref="SimpleSpellChecker.ForcedQuestionSuffixSplitCheck(Word, Sentence)"/>
        /// <seealso cref="SimpleSpellChecker.ForcedSuffixSplitCheck(Word, Sentence)"/>
        /// <returns>a boolean indicating whether suffix check is enabled for the spell checker</returns>
        public bool SuffixCheck()
        {
            return _suffixCheck;
        }
        
        /// <summary>
        /// Returns whether the root n-gram is enabled for the spell checker.
        /// </summary>
        /// <returns>a boolean indicating whether the root n-gram is enabled for the spell checker</returns>
        public bool IsRootNGram()
        {
            return _rootNGram;
        }
        
        /// <summary>
        /// Returns the minimum length of words viable for spell checking.
        /// </summary>
        /// <returns>the minimum word length for the spell checker</returns>
        public int GetMinWordLength()
        {
            return _minWordLength;
        }
        
        /// <summary>
        /// Returns the domain name.
        /// </summary>
        /// <returns>the domain name</returns>
        public string GetDomain()
        {
            return _domain;
        }
    }
    
}