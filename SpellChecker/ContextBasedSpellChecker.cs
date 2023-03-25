using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Corpus;
using Dictionary.Dictionary;
using MorphologicalAnalysis;
using NGram;

namespace SpellChecker
{
    public class ContextBasedSpellChecker : NGramSpellChecker
    {
        private Dictionary<string, List<string>> _contextList;
        
        /// <summary>
        /// A constructor of <see cref="ContextBasedSpellChecker"/> class which takes an <see cref="FsmMorphologicalAnalyzer"/>, an <see cref="NGram"/>
        /// and a <see cref="SpellCheckerParameter"/> as inputs. Then, calls its super class <see cref="NGramSpellChecker"/> with given inputs.
        /// </summary>
        /// <param name="fsm">Input of type <see cref="FsmMorphologicalAnalyzer"/>.</param>
        /// <param name="nGram">Input of type <see cref="NGram"/>.</param>
        /// <param name="parameter">Input of type <see cref="SpellCheckerParameter"/>.</param>
        public ContextBasedSpellChecker(FsmMorphologicalAnalyzer fsm, NGram<string> nGram,
            SpellCheckerParameter parameter)
            : base(fsm, nGram, parameter)
        {
        }
        
        /// <summary>
        /// Another constructor of <see cref="ContextBasedSpellChecker"/> class which takes an <see cref="FsmMorphologicalAnalyzer"/>
        /// and an<see cref="NGram"/> as inputs. Then, calls its super class <see cref="NGramSpellChecker"/> with given inputs.
        /// </summary>
        /// <param name="fsm">Input of type <see cref="FsmMorphologicalAnalyzer"/>.</param>
        /// <param name="nGram">Input of type <see cref="NGram"/>.</param>
        public ContextBasedSpellChecker(FsmMorphologicalAnalyzer fsm, NGram<string> nGram)
            : base(fsm, nGram)
        {
        }
        
        /// <inheritdoc/>
        /// This method also loads context information from a file.
        protected override void LoadDictionaries()
        {
            base.LoadDictionaries();
            List<string> contextListWords;
            _contextList = new Dictionary<string, List<string>>();
            string line;
            StreamReader contextListReader;
            try
            {
                contextListReader = GetReader("context_list.txt");
                line = contextListReader.ReadLine();
                while (line != null)
                {
                    var word = line.Split("\t")[0];
                    var otherWords = line.Split("\t")[1].Split(" ");
                    contextListWords = new List<string>(otherWords);
                    _contextList.Add(word, contextListWords);
                    line = contextListReader.ReadLine();
                }
                contextListReader.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        
        /// <summary>
        /// Uses context information to generate candidates for a misspelled word.
        /// The candidates are the words that are in the context of the neighbouring words of the misspelled word.
        /// Uses the <see cref="DamerauLevenshteinDistance(string, string)"/> method to calculate the distance between the misspelled word and
        /// the candidates and to determine whether the candidates are valid.
        /// </summary>
        /// <param name="word">The misspelled word.</param>
        /// <param name="sentence">The sentence containing the misspelled word.</param>
        /// <returns>An ArrayList of valid candidates for the misspelled word.</returns>
        protected override List<Candidate> CandidateList(Word word, Sentence sentence)
        {
            var words = new List<Word>(sentence.GetWords());
            var validCandidates = new List<Candidate>();
            var candidates = new HashSet<Candidate>();
            words.Remove(word);
            foreach (var w in words)
            {
                var parses = Fsm.MorphologicalAnalysis(w.GetName());
                if (parses.Size() > 0)
                {
                    var root = parses.GetParseWithLongestRootWord().GetWord().GetName();
                    if (_contextList.ContainsKey(root))
                    {
                        foreach (var s in _contextList[root])
                        {
                            candidates.Add(new Candidate(s,Operator.CONTEXT_BASED));
                        }
                    }
                }
            }
            foreach (var candidate in candidates)
            {
                int distance;
                if (candidate.GetName().Length < 5)
                {
                    distance = 1;
                }
                else
                {
                    if (candidate.GetName().Length < 7)
                    {
                        distance = 2;
                    }
                    else
                    {
                        distance = 3;
                    }
                }
                if (DamerauLevenshteinDistance(word.GetName(), candidate.GetName()) <= distance)
                {
                    validCandidates.Add(candidate);
                }
            }
            return validCandidates;
        }
        
        /// <summary>
        /// Calculates the Damerau-Levenshtein distance between two strings.
        /// This method also allows for the transposition of adjacent characters,
        /// which is not possible in a standard Levenshtein distance calculation.
        /// </summary>
        /// <param name="first">the first string</param>
        /// <param name="second">the second string</param>
        /// <returns>the Damerau-Levenshtein distance between the two strings</returns>
        private int DamerauLevenshteinDistance(string first, string second)
        {
            if (string.IsNullOrEmpty(first))
            {
                return string.IsNullOrEmpty(second) ? 0 : second.Length;
            }
            if (string.IsNullOrEmpty(second))
            {
                return first.Length;
            }
            var firstLength = first.Length;
            var secondLength = second.Length;
            var distanceMatrix = new int[firstLength + 1, secondLength + 1];

            for (var firstIndex = 0; firstIndex <= firstLength; firstIndex++)
            {
                distanceMatrix[firstIndex, 0] = firstIndex;
            }
            for (var secondIndex = 0; secondIndex <= secondLength; secondIndex++)
            {
                distanceMatrix[0, secondIndex] = secondIndex;
            }
            for (var firstIndex = 1; firstIndex <= firstLength; firstIndex++)
            {
                for (var secondIndex = 1; secondIndex <= secondLength; secondIndex++)
                {
                    var cost = (first[firstIndex - 1] == second[secondIndex - 1]) ? 0 : 1;
                    distanceMatrix[firstIndex, secondIndex] = System.Math.Min(System.Math.Min(distanceMatrix[firstIndex - 1, secondIndex] + 1,
                        distanceMatrix[firstIndex, secondIndex - 1] + 1), distanceMatrix[firstIndex - 1, secondIndex - 1] + cost);
                    if (firstIndex == 1 || secondIndex == 1)
                    {
                        continue;
                    }
                    if (first[firstIndex - 1] == second[secondIndex - 2] && first[firstIndex - 2] == second[secondIndex - 1])
                    {
                        distanceMatrix[firstIndex, secondIndex] = System.Math.Min(distanceMatrix[firstIndex, secondIndex], distanceMatrix[firstIndex - 2, secondIndex - 2] + cost);
                    }
                }
            }
            return distanceMatrix[firstLength, secondLength];
        }
    }
}