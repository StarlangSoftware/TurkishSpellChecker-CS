using System;
using System.Collections.Generic;
using Corpus;
using Dictionary.Dictionary;
using Dictionary.Language;
using MorphologicalAnalysis;

namespace SpellChecker
{
    public class SimpleSpellChecker : SpellChecker
    {
        protected FsmMorphologicalAnalyzer fsm;

        /**
         * <summary>The generateCandidateList method takes a string as an input. Firstly, it creates a string consists of lowercase Turkish letters
         * and an {@link List} candidates. Then, it loops i times where i ranges from 0 to the length of given word. It gets substring
         * from 0 to ith index and concatenates it with substring from i+1 to the last index as a new string called deleted. Then, adds
         * this string to the candidates {@link List}. Secondly, it loops j times where j ranges from 0 to length of
         * lowercase letters string and adds the jth character of this string between substring of given word from 0 to ith index
         * and the substring from i+1 to the last index, then adds it to the candidates {@link List}. Thirdly, it loops j
         * times where j ranges from 0 to length of lowercase letters string and adds the jth character of this string between
         * substring of given word from 0 to ith index and the substring from i to the last index, then adds it to the candidates {@link List}.</summary>
         *
         * <param name="word">string input.</param>
         * <returns>List candidates.</returns>
         */
        private List<string> generateCandidateList(string word)
        {
            var s = TurkishLanguage.LOWERCASE_LETTERS;
            var candidates = new List<string>();
            for (var i = 0; i < word.Length; i++)
            {
                if (i < word.Length - 1)
                {
                    var swapped = word.Substring(0, i) + word[i + 1] + word[i] + word.Substring(i + 2);
                    candidates.Add(swapped);
                }

                if (TurkishLanguage.LETTERS.Contains("" + word[i]) || "qwx".Contains("" + word[i]))
                {
                    var deleted = word.Substring(0, i) + word.Substring(i + 1);
                    candidates.Add(deleted);
                    foreach (var t in s)
                    {
                        var replaced = word.Substring(0, i) + t + word.Substring(i + 1);
                        candidates.Add(replaced);
                    }

                    foreach (var t in s)
                    {
                        var added = word.Substring(0, i) + t + word.Substring(i);
                        candidates.Add(added);
                    }
                }
            }

            return candidates;
        }

        /**
         * <summary>The candidateList method takes a {@link Word} as an input and creates a candidates {@link List} by calling generateCandidateList
         * method with given word. Then, it loop i times where i ranges from 0 to size of candidates {@link List} and creates a
         * {@link FsmParseList} by calling morphologicalAnalysis with each item of candidates {@link List}. If the size of
         * {@link FsmParseList} is 0, it then removes the ith item.</summary>
         *
         * <param name="word">Word input.</param>
         * <returns>candidates {@link List}.</returns>
         */
        protected List<string> CandidateList(Word word)
        {
            var candidates = generateCandidateList(word.GetName());
            for (var i = 0; i < candidates.Count; i++)
            {
                var fsmParseList = fsm.MorphologicalAnalysis(candidates[i]);
                if (fsmParseList.Size() == 0)
                {
                    var newCandidate = fsm.GetDictionary().GetCorrectForm(candidates[i]);
                    if (newCandidate != null && fsm.MorphologicalAnalysis(newCandidate).Size() > 0)
                    {
                        candidates[i] = newCandidate;
                    }
                    else
                    {
                        candidates.RemoveAt(i);
                        i--;
                    }
                }
            }

            return candidates;
        }

        /**
         * <summary>A constructor of {@link SimpleSpellChecker} class which takes a {@link FsmMorphologicalAnalyzer} as an input and
         * assigns it to the fsm variable.</summary>
         *
         * <param name="fsm">{@link FsmMorphologicalAnalyzer} type input.</param>
         */
        public SimpleSpellChecker(FsmMorphologicalAnalyzer fsm)
        {
            this.fsm = fsm;
        }


        /**
         * <summary>The spellCheck method takes a {@link Sentence} as an input and loops i times where i ranges from 0 to size of words in given sentence.
         * Then, it calls morphologicalAnalysis method with each word and assigns it to the {@link FsmParseList}, if the size of
         * {@link FsmParseList} is equal to the 0, it adds current word to the candidateList and assigns it to the candidates {@link List}.
         * if the size of candidates greater than 0, it generates a random number and selects an item from candidates {@link List} with
         * this random number and assign it as newWord. If the size of candidates is not greater than 0, it directly assigns the
         * current word as newWord. At the end, it adds the newWord to the result {@link Sentence}.</summary>
         *
         * <param name="sentence">{@link Sentence} type input.</param>
         * <returns>Sentence result.</returns>
         */
        public Sentence SpellCheck(Sentence sentence)
        {
            var random = new Random();
            var result = new Sentence();
            for (var i = 0; i < sentence.WordCount(); i++)
            {
                var word = sentence.GetWord(i);
                var fsmParseList = fsm.MorphologicalAnalysis(word.GetName());
                Word newWord;
                if (fsmParseList.Size() == 0)
                {
                    var candidates = CandidateList(word);
                    if (candidates.Count > 0)
                    {
                        var randomCandidate = random.Next(candidates.Count);
                        newWord = new Word(candidates[randomCandidate]);
                    }
                    else
                    {
                        newWord = word;
                    }
                }
                else
                {
                    newWord = word;
                }

                result.AddWord(newWord);
            }

            return result;
        }
    }
}