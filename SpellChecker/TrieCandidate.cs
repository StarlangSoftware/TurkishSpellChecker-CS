namespace SpellChecker
{
    public class TrieCandidate : Candidate
    {
        private int _currentIndex;
        private double _currentPenalty;

        /// <summary>
        /// Constructs a new <see cref="TrieCandidate"/> object with the specified word, current index, and current penalty.
        /// </summary>
        /// <param name="word">the candidate word</param>
        /// <param name="currentIndex">the current index of the candidate word</param>
        /// <param name="currentPenalty">the current penalty associated with the candidate word</param>
        public TrieCandidate(string word, int currentIndex, double currentPenalty) : base(word, Operator.TRIE_BASED)
        {
            _currentIndex = currentIndex;
            _currentPenalty = currentPenalty;
        }
        
        /// <summary>
        /// Returns the current index of the candidate word.
        /// </summary>
        /// <returns>the current index of the candidate word</returns>
        public int GetCurrentIndex()
        {
            return _currentIndex;
        }
        
        /// <summary>
        /// Returns the current penalty value associated with the candidate word.
        /// </summary>
        /// <returns>the current penalty value associated with the candidate word</returns>
        public double GetCurrentPenalty()
        {
            return _currentPenalty;
        }
        
        /// <summary>
        /// Increments the current index of the candidate word by 1.
        /// </summary>
        public void NextIndex()
        {
            _currentIndex += 1;
        }
    }
}