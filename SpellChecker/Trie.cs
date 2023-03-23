using System.Globalization;

namespace SpellChecker
{
    public class Trie
    {
        private TrieNode _rootNode;
        
        /// <summary>
        /// A constructor of <see cref="Trie"/> class which constructs a new Trie with an empty root node.
        /// </summary>
        public Trie()
        {
            _rootNode = new TrieNode();
        }
        
        /// <summary>
        /// Inserts a new word into the Trie.
        /// </summary>
        /// <param name="word">The word to be inserted.</param>
        public void Insert(string word)
        {
            var currentNode = _rootNode;
            foreach (var character in word)
            {
                if (currentNode.GetChild(character) == null)
                {
                    currentNode.AddChild(character, new TrieNode());
                }
                currentNode = currentNode.GetChild(character);
            }
            currentNode.SetIsWord(true);
        }
        
        /// <summary>
        /// Checks if a word is in the Trie.
        /// </summary>
        /// <param name="word">The word to be searched for.</param>
        /// <returns>true if the word is in the Trie, false otherwise.</returns>
        public bool Search(string word)
        {
            var node = GetTrieNode(word.ToLower(new CultureInfo("tr-TR")));
            if(node == null)
            {
                return false;
            }
            return node.IsWord();
            
        }
        
        /// <summary>
        /// Checks if a given prefix exists in the Trie.
        /// </summary>
        /// <param name="prefix">The prefix to be searched for.</param>
        /// <returns>true if the prefix exists, false otherwise.</returns>
        public bool StartsWith(string prefix)
        {
            if(GetTrieNode(prefix.ToLower(new CultureInfo("tr-TR"))) == null)
            {
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// Returns the TrieNode corresponding to the last character of a given word.
        /// </summary>
        /// <param name="word">The word to be searched for.</param>
        /// <returns>The TrieNode corresponding to the last character of the word.</returns>
        public TrieNode GetTrieNode(string word)
        {
            var currentNode = _rootNode;
            foreach (var character in word)
            {
                if (currentNode.GetChild(character) == null)
                {
                    return null;
                }
                currentNode = currentNode.GetChild(character);
            }
            return currentNode;
        }
    }
}