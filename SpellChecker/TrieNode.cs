using System.Collections.Generic;
using System.Text;

namespace SpellChecker
{
    public class TrieNode
    {
        private Dictionary<char,TrieNode> _children;
        private bool _isWord;
        
        /// <summary>
        /// A constructor of <see cref="TrieNode"/> class which constructs a new TrieNode with an empty children Dictionary.
        /// </summary>
        public TrieNode()
        {
           _children = new Dictionary<char, TrieNode>();
        }
        
        /// <summary>
        /// Returns the child TrieNode with the given character as its value.
        /// </summary>
        /// <param name="character">The character value of the child TrieNode.</param>
        /// <returns>TrieNode with the given character value.</returns>
        public TrieNode GetChild(char character)
        {
            if (_children.TryGetValue(character, out var child))
            {
                return child;
            }
            return null;
        }

        /// <summary>
        /// Adds a child TrieNode to the current TrieNode instance.
        /// </summary>
        /// <param name="character">The character key of the child node to be added.</param>
        /// <param name="child">The TrieNode object to be added as a child.</param>
        public void AddChild(char character, TrieNode child) {
            _children.Add(character, child);
        }
        
        /// <summary>
        /// Returns a string representation of the keys of all child TrieNodes of the current TrieNode instance.
        /// </summary>
        /// <returns>A string of characters representing the keys of all child TrieNodes.</returns>
        public string ChildrenToString() {
            var result = new StringBuilder();
            foreach (var child in _children) {
                result.Append(child.Key);
                result.Append(" ");
            }
            return result.ToString();
        }
        
        /// <summary>
        /// Returns whether the current TrieNode represents the end of a word.
        /// </summary>
        /// <returns>true if the current TrieNode represents the end of a word, false otherwise.</returns>
        public bool IsWord() {
            return _isWord;
        }
        
        /// <summary>
        /// Sets whether the current TrieNode represents the end of a word.
        /// </summary>
        /// <param name="isWord">true if the current TrieNode represents the end of a word, false otherwise.</param>
        public void SetIsWord(bool isWord) {
            _isWord = isWord;
        }
    }
}