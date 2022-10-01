using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Corpus;
using Dictionary.Dictionary;
using Dictionary.Language;
using MorphologicalAnalysis;

namespace SpellChecker
{
    public class SimpleSpellChecker : SpellChecker
    {
        protected FsmMorphologicalAnalyzer fsm;
        private Dictionary<string, string> mergedWords = new Dictionary<string, string>();
        private Dictionary<string, string> splitWords = new Dictionary<string, string>();

        private static List<string> shortcuts = new List<string>()
        {
            "cc", "cm2", "cm", "gb", "ghz", "gr", "gram", "hz", "inc", "inch", "inç",
            "kg", "kw", "kva", "litre", "lt", "m2", "m3", "mah", "mb", "metre", "mg", "mhz", "ml", "mm", "mp", "ms",
            "mt", "mv", "tb", "tl", "va", "volt", "watt", "ah", "hp"
        };

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
        private List<Candidate> GenerateCandidateList(string word)
        {
            var s = TurkishLanguage.LOWERCASE_LETTERS;
            var candidates = new List<Candidate>();
            for (var i = 0; i < word.Length; i++)
            {
                if (i < word.Length - 1)
                {
                    var swapped = word.Substring(0, i) + word[i + 1] + word[i] + word.Substring(i + 2);
                    candidates.Add(new Candidate(swapped, Operator.SPELL_CHECK));
                }

                if (TurkishLanguage.LETTERS.Contains("" + word[i]) || "qwx".Contains("" + word[i]))
                {
                    var deleted = word.Substring(0, i) + word.Substring(i + 1);
                    candidates.Add(new Candidate(deleted, Operator.SPELL_CHECK));
                    foreach (var t in s)
                    {
                        var replaced = word.Substring(0, i) + t + word.Substring(i + 1);
                        candidates.Add(new Candidate(replaced, Operator.SPELL_CHECK));
                    }

                    foreach (var t in s)
                    {
                        var added = word.Substring(0, i) + t + word.Substring(i);
                        candidates.Add(new Candidate(added, Operator.SPELL_CHECK));
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
        protected List<Candidate> CandidateList(Word word)
        {
            var candidates = GenerateCandidateList(word.GetName());
            for (var i = 0; i < candidates.Count; i++)
            {
                var fsmParseList = fsm.MorphologicalAnalysis(candidates[i].GetName());
                if (fsmParseList.Size() == 0)
                {
                    var newCandidate = fsm.GetDictionary().GetCorrectForm(candidates[i].GetName());
                    if (newCandidate != null && fsm.MorphologicalAnalysis(newCandidate).Size() > 0)
                    {
                        candidates[i] = new Candidate(newCandidate, Operator.MISSPELLED_REPLACE);
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
            LoadDictionaries();
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
                Word nextWord = null;
                Word previousWord = null;
                if (i > 0)
                {
                    previousWord = sentence.GetWord(i - 1);
                }

                if (i < sentence.WordCount() - 1)
                {
                    nextWord = sentence.GetWord(i + 1);
                }

                if (ForcedMisspellCheck(word, result) || ForcedBackwardMergeCheck(word, result, previousWord))
                {
                    continue;
                }

                if (ForcedForwardMergeCheck(word, result, nextWord))
                {
                    i++;
                    continue;
                }

                if (ForcedSplitCheck(word, result) || ForcedShortcutCheck(word, result))
                {
                    continue;
                }

                var fsmParseList = fsm.MorphologicalAnalysis(word.GetName());
                Word newWord;
                if (fsmParseList.Size() == 0)
                {
                    var candidates = MergedCandidatesList(previousWord, word, nextWord);
                    if (candidates.Count < 1)
                    {
                        candidates = CandidateList(word);
                    }

                    if (candidates.Count < 1)
                    {
                        candidates.AddRange(SplitCandidatesList(word));
                    }

                    if (candidates.Count > 0)
                    {
                        var randomCandidate = random.Next(candidates.Count);
                        newWord = new Word(candidates[randomCandidate].GetName());
                        if (candidates[randomCandidate].GetOperator() == Operator.BACKWARD_MERGE)
                        {
                            result.ReplaceWord(i - 1, newWord);
                            continue;
                        }

                        if (candidates[randomCandidate].GetOperator() == Operator.FORWARD_MERGE)
                        {
                            i++;
                        }

                        if (candidates[randomCandidate].GetOperator() == Operator.SPLIT)
                        {
                            AddSplitWords(candidates[randomCandidate].GetName(), result);
                            continue;
                        }
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

        protected bool ForcedMisspellCheck(Word word, Sentence result)
        {
            var forcedReplacement = fsm.GetDictionary().GetCorrectForm(word.GetName());
            if (forcedReplacement != null)
            {
                result.AddWord(new Word(forcedReplacement));
                return true;
            }

            return false;
        }

        protected bool ForcedBackwardMergeCheck(Word word, Sentence result, Word previousWord)
        {
            if (previousWord != null)
            {
                var forcedReplacement =
                    GetCorrectForm(result.GetWord(result.WordCount() - 1).GetName() + " " + word.GetName(),
                        mergedWords);
                if (forcedReplacement != null)
                {
                    result.ReplaceWord(result.WordCount() - 1, new Word(forcedReplacement));
                    return true;
                }
            }

            return false;
        }

        protected bool ForcedForwardMergeCheck(Word word, Sentence result, Word nextWord)
        {
            if (nextWord != null)
            {
                var forcedReplacement = GetCorrectForm(word.GetName() + " " + nextWord.GetName(), mergedWords);
                if (forcedReplacement != null)
                {
                    result.AddWord(new Word(forcedReplacement));
                    return true;
                }
            }

            return false;
        }

        protected void AddSplitWords(string multiWord, Sentence result)
        {
            var words = multiWord.Split(" ");
            result.AddWord(new Word(words[0]));
            result.AddWord(new Word(words[1]));
        }

        protected bool ForcedSplitCheck(Word word, Sentence result)
        {
            string forcedReplacement = GetCorrectForm(word.GetName(), splitWords);
            if (forcedReplacement != null)
            {
                AddSplitWords(forcedReplacement, result);
                return true;
            }

            return false;
        }

        protected bool ForcedShortcutCheck(Word word, Sentence result)
        {
            var shortcutRegex = "[0-9]+(" + shortcuts[0];
            for (var i = 1; i < shortcuts.Count; i++)
            {
                shortcutRegex += "|" + shortcuts[i];
            }

            shortcutRegex += ")";

            if (new Regex(shortcutRegex).IsMatch(word.GetName()))
            {
                var pair = GetSplitPair(word);
                result.AddWord(new Word(pair.Item1));
                result.AddWord(new Word(pair.Item2));
                return true;
            }

            return false;
        }

        protected List<Candidate> MergedCandidatesList(Word previousWord, Word word, Word nextWord)
        {
            var mergedCandidates = new List<Candidate>();
            Candidate backwardMergeCandidate = null;
            Candidate forwardMergeCandidate;
            if (previousWord != null)
            {
                backwardMergeCandidate =
                    new Candidate(previousWord.GetName() + word.GetName(), Operator.BACKWARD_MERGE);
                var fsmParseList = fsm.MorphologicalAnalysis(backwardMergeCandidate.GetName());
                if (fsmParseList.Size() != 0)
                {
                    mergedCandidates.Add(backwardMergeCandidate);
                }
            }

            if (nextWord != null)
            {
                forwardMergeCandidate = new Candidate(word.GetName() + nextWord.GetName(), Operator.FORWARD_MERGE);
                if (backwardMergeCandidate == null ||
                    backwardMergeCandidate.GetName() != forwardMergeCandidate.GetName())
                {
                    FsmParseList fsmParseList = fsm.MorphologicalAnalysis(forwardMergeCandidate.GetName());
                    if (fsmParseList.Size() != 0)
                    {
                        mergedCandidates.Add(forwardMergeCandidate);
                    }
                }
            }

            return mergedCandidates;
        }

        protected List<Candidate> SplitCandidatesList(Word word)
        {
            var splitCandidates = new List<Candidate>();
            for (var i = 4; i < word.GetName().Length - 3; i++)
            {
                var firstPart = word.GetName().Substring(0, i);
                var secondPart = word.GetName().Substring(i);
                var fsmParseListFirst = fsm.MorphologicalAnalysis(firstPart);
                var fsmParseListSecond = fsm.MorphologicalAnalysis(secondPart);
                if (fsmParseListFirst.Size() > 0 && fsmParseListSecond.Size() > 0)
                {
                    splitCandidates.Add(new Candidate(firstPart + " " + secondPart, Operator.SPLIT));
                }
            }

            return splitCandidates;
        }

        private void LoadDictionaries()
        {
            string line;
            string[] list;
            var assembly = typeof(SpellChecker).Assembly;
            var stream = assembly.GetManifestResourceStream("SpellChecker.merged.txt");
            var streamReader = new StreamReader(stream);
            line = streamReader.ReadLine();
            while (line != null)
            {
                list = line.Split(" ");
                mergedWords[list[0] + " " + list[1]] = list[2];
                line = streamReader.ReadLine();
            }

            stream = assembly.GetManifestResourceStream("SpellChecker.split.txt");
            streamReader = new StreamReader(stream);
            line = streamReader.ReadLine();
            while (line != null)
            {
                list = line.Split(" ");
                splitWords[list[0]] = list[1] + " " + list[2];
                line = streamReader.ReadLine();
            }
        }

        protected string GetCorrectForm(string wordName, Dictionary<string, string> dictionary)
        {
            if (dictionary.ContainsKey(wordName))
            {
                return dictionary[wordName];
            }

            return null;
        }

        private Tuple<string, string> GetSplitPair(Word word)
        {
            var key = "";
            int j;
            for (j = 0; j < word.GetName().Length; j++)
            {
                if (word.GetName()[j] >= '0' && word.GetName()[j] <= '9')
                {
                    key += word.GetName()[j];
                }
                else
                {
                    break;
                }
            }

            var value = word.GetName().Substring(j);
            return new Tuple<string, string>(key, value);
        }
    }
}