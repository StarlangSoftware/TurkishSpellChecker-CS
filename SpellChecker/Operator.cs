namespace SpellChecker
{
    /// <summary>
    /// Enum class that represents different types of spell checking operators that can be applied to a Word.
    /// </summary>
    public enum Operator
    {
        /// <summary>
        /// No change is made to the Word.
        /// </summary>
        NO_CHANGE, 
        
        /// <summary>
        /// The Word is changed into a Word in the misspellings list.
        /// </summary>
        MISSPELLED_REPLACE, 
        
        /// <summary>
        /// The Word is changed into a Candidate by deleting, adding, replacing a character or swapping two consecutive characters.
        /// </summary>
        SPELL_CHECK,
        
        /// <summary>
        /// The Word is split into multiple Candidates.
        /// </summary>
        SPLIT, 
        
        /// <summary>
        /// The Word and the Word after are merged into one Candidate.
        /// </summary>
        FORWARD_MERGE, 
        
        /// <summary>
        /// The Word and the Word before are merged into one Candidate.
        /// </summary>
        BACKWARD_MERGE, 
        
        /// <summary>
        /// The Word is changed into a Candidate based on the context-based spell checking algorithm.
        /// </summary>
        CONTEXT_BASED, 
        
        /// <summary>
        /// The Word is changed into a Candidate based on the trie-based spell checking algorithm.
        /// </summary>
        TRIE_BASED
    }
}