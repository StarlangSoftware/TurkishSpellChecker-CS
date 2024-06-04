using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Corpus;
using Dictionary.Dictionary;
using MorphologicalAnalysis;
using NGram;

namespace SpellChecker
{
    public class NGramSpellChecker : SimpleSpellChecker
    {
        private readonly NGram<string> _nGram;

        /**
         * <summary>
         * A constructor of <see cref="NGramSpellChecker"/> class which takes an <see cref="FsmMorphologicalAnalyzer"/>, an <see cref="NGram"/>
         * and a <see cref="SpellCheckerParameter"/> as inputs. Then, calls its super class <see cref="SimpleSpellChecker"/> with given <see cref="FsmMorphologicalAnalyzer"/>
         * and <see cref="SpellCheckerParameter"/>. Finally, it assigns given <see cref="NGram"/> to the nGram variable.
         * </summary>
         *
         * <param name="fsm"><see cref="FsmMorphologicalAnalyzer"/> type input.</param>
         * <param name="nGram"><see cref="NGram"/> type input.</param>    
         * <param name="parameter"><see cref="SpellCheckerParameter"/> type input.</param>
         */
        public NGramSpellChecker(FsmMorphologicalAnalyzer fsm, NGram<string> nGram, SpellCheckerParameter parameter) : base(fsm, parameter)
        {
            _nGram = nGram;
        }
        
        /**
         * <summary>
         * Another constructor of <see cref="NGramSpellChecker"/> class which takes an <see cref="FsmMorphologicalAnalyzer"/> and an <see cref="NGram"/>
         * as inputs. Then, calls its super class <see cref="SimpleSpellChecker"/> with given <see cref="FsmMorphologicalAnalyzer"/> and
         * assigns given <see cref="NGram"/> to the nGram variable.
         * </summary>
         *
         * <param name="fsm"><see cref="FsmMorphologicalAnalyzer"/> type input.</param>
         * <param name="nGram"><see cref="NGram"/> type input.</param>
         */
        public NGramSpellChecker(FsmMorphologicalAnalyzer fsm, NGram<string> nGram) : base(fsm)
        {
            _nGram = nGram;
        }

        /**
        * <summary>
        * Checks the morphological analysis of the given word in the given index. If there is no misspelling, it returns
        * the longest root word of the possible analysis.
        * </summary>
        * <param name="sentence"> Sentence to be analyzed.</param>
        * <param name="index"> Index of the word</param>
        * <returns> If the word is misspelled, null; otherwise the longest root word of the possible analysis.</returns>
        */
        private Word CheckAnalysisAndSetRootForWordAtIndex(Sentence sentence, int index)
        {
            if (index < sentence.WordCount())
            {
                var wordName = sentence.GetWord(index).GetName();
                if (new Regex(".*\\d+.*").IsMatch(wordName) && new Regex(".*[a-zA-ZçöğüşıÇÖĞÜŞİ]+.*").IsMatch(wordName)
                    && !wordName.Contains("'") || wordName.Length < Parameter.GetMinWordLength())
                {
                    return sentence.GetWord(index);
                }
                var fsmParses = Fsm.MorphologicalAnalysis(wordName);
                if (fsmParses.Size() != 0)
                {
                    if (Parameter.IsRootNGram())
                    {
                        return fsmParses.GetParseWithLongestRootWord().GetWord();
                    }
                    return sentence.GetWord(index);
                }
                else
                {
                    var upperCaseWordName = wordName.Substring(0,1).ToUpper(new CultureInfo("tr-TR")) + wordName.Substring(1);
                    var upperCaseFsmParses = Fsm.MorphologicalAnalysis(upperCaseWordName);
                    if (upperCaseFsmParses.Size() != 0)
                    {
                        if (Parameter.IsRootNGram())
                        {
                            return upperCaseFsmParses.GetParseWithLongestRootWord().GetWord();
                        }
                        return sentence.GetWord(index);
                    }
                }
            }
            return null;
        }
        
        /**
        * <summary>
        * Checks the morphological analysis of the given word. If there is no misspelling, it returns
        * the longest root word of the possible analysis.
        * </summary>
        * <param name="word"> Word to be analyzed.</param>
        * <returns> If the word is misspelled, null; otherwise the longest root word of the possible analysis.</returns>
        */
        private Word CheckAnalysisAndSetRoot(string word)
        {
            var fsmParsesOfWord = Fsm.MorphologicalAnalysis(word);
            if (fsmParsesOfWord.Size() != 0)    
            {
                if (Parameter.IsRootNGram())
                {
                    return fsmParsesOfWord.GetParseWithLongestRootWord().GetWord();
                }
                return new Word(word);
            }
            var fsmParsesOfCapitalizedWord = Fsm.MorphologicalAnalysis(word.Substring(0, 1).ToUpper(new CultureInfo("tr-TR")) + word.Substring(1));
            if (fsmParsesOfCapitalizedWord.Size() != 0)  
            {
                if (Parameter.IsRootNGram())
                {
                    return fsmParsesOfCapitalizedWord.GetParseWithLongestRootWord().GetWord();
                }
                return new Word(word);
            }
            return null;
        }

        /// <summary>
        /// Returns the bi-gram probability P(word2 | word1) for the given bigram consisting of two words.
        /// </summary>
        /// <param name="word1">First word in bi-gram</param>
        /// <param name="word2">Second word in bi-gram</param>
        /// <returns>Bi-gram probability P(word2 | word1)</returns>
        private double GetProbability(string word1, string word2)
        {
            return _nGram.GetProbability(word1, word2);
        }

        /**
         * <summary>
         * The spellCheck method takes a {@link Sentence} as an input and loops i times where i ranges from 0 to size of words in given sentence.
         * Then, it calls morphologicalAnalysis method with each word and assigns it to the {@link FsmParseList}, if the size of
         * {@link FsmParseList} is equal to the 0, it adds current word to the candidateList and assigns it to the candidates {@link ArrayList}.
         * <p/>
         * Later on, it loops through candidates {@link ArrayList} and calls morphologicalAnalysis method with each word and
         * assigns it to the {@link FsmParseList}. Then, it gets the root from {@link FsmParseList}. For the first time, it defines a previousRoot
         * by calling getProbability method with root, and for the following times it calls getProbability method with previousRoot and root.
         * Then, it finds out the best probability and the corresponding candidate as best candidate and adds it to the result {@link Sentence}.
         * <p/>
         * If the size of {@link FsmParseList} is not equal to 0, it directly adds the current word to the result {@link Sentence} and finds
         * the previousRoot directly from the {@link FsmParseList}.
         * </summary>
         *
         * <param name="sentence">{@link Sentence} type input.</param>
         * <returns>Sentence result.</returns>
         */
        public new Sentence SpellCheck(Sentence sentence)
        {
            Word previousRoot = null;
            double previousProbability, nextProbability, bestProbability;
            var result = new Sentence();
            var root = CheckAnalysisAndSetRootForWordAtIndex(sentence, 0);
            var nextRoot = CheckAnalysisAndSetRootForWordAtIndex(sentence, 1);
            for (var i = 0; i < sentence.WordCount(); i++)
            {
                Word nextWord = null;
                Word previousWord = null;
                Word nextNextWord = null;
                Word previousPreviousWord = null;
                var word = sentence.GetWord(i);
                if (i > 0)
                {
                    previousWord = sentence.GetWord(i - 1);
                }

                if (i > 1)
                {
                    previousPreviousWord = sentence.GetWord(i - 2);
                }

                if (i < sentence.WordCount() - 1)
                {
                    nextWord = sentence.GetWord(i + 1);
                }

                if (i < sentence.WordCount() - 2)
                {
                    nextNextWord = sentence.GetWord(i + 2);
                }

                if (ForcedMisspellCheck(word, result))
                {
                    previousRoot = CheckAnalysisAndSetRootForWordAtIndex(result, result.WordCount() - 1);
                    root = nextRoot;
                    nextRoot = CheckAnalysisAndSetRootForWordAtIndex(sentence, i + 2);
                    continue;
                }
                if (ForcedBackwardMergeCheck(word, result, previousWord) || ForcedSuffixMergeCheck(word, result, previousWord))
                {
                    previousRoot = CheckAnalysisAndSetRootForWordAtIndex(result, result.WordCount() - 1);
                    root = CheckAnalysisAndSetRootForWordAtIndex(sentence, i + 1);
                    nextRoot = CheckAnalysisAndSetRootForWordAtIndex(sentence, i + 2);
                    continue;
                }
                if (ForcedForwardMergeCheck(word, result, nextWord) || ForcedHyphenMergeCheck(word, result, previousWord, nextWord))
                {
                    i++;
                    previousRoot = CheckAnalysisAndSetRootForWordAtIndex(result, result.WordCount() - 1);
                    root = CheckAnalysisAndSetRootForWordAtIndex(sentence, i + 1);
                    nextRoot = CheckAnalysisAndSetRootForWordAtIndex(sentence, i + 2);
                    continue;
                }
                if (ForcedSplitCheck(word, result) || ForcedShortcutSplitCheck(word, result))
                {
                    previousRoot = CheckAnalysisAndSetRootForWordAtIndex(result, result.WordCount() - 1);
                    root = nextRoot;
                    nextRoot = CheckAnalysisAndSetRootForWordAtIndex(sentence, i + 2);
                    continue;
                }
                if (Parameter.SuffixCheck())
                {
                    if (ForcedDeDaSplitCheck(word, result) || ForcedSuffixSplitCheck(word, result) || ForcedQuestionSuffixSplitCheck(word, result))
                    {
                        previousRoot = CheckAnalysisAndSetRootForWordAtIndex(result, result.WordCount() - 1);
                        root = nextRoot;
                        nextRoot = CheckAnalysisAndSetRootForWordAtIndex(sentence, i + 2);
                        continue;   
                    }
                }

                if (root == null || (word.GetName().Length < Parameter.GetMinWordLength() && Fsm.MorphologicalAnalysis(word.GetName()).Size() == 0))
                {
                    var candidates = new List<Candidate>();
                    if (root == null)
                    {
                        candidates.AddRange(CandidateList(word, sentence));
                        candidates.AddRange(SplitCandidatesList(word));
                    }
                    candidates.AddRange(MergedCandidatesList(previousWord, word, nextWord));
                    var bestCandidate = new Candidate(word.GetName(), Operator.NO_CHANGE);
                    var bestRoot = word;
                    bestProbability = Parameter.GetThreshold();
                    foreach (var candidate in candidates)
                    {
                        if (candidate.GetOperator() == Operator.SPELL_CHECK ||
                            candidate.GetOperator() == Operator.MISSPELLED_REPLACE ||
                            candidate.GetOperator() == Operator.CONTEXT_BASED||
                            candidate.GetOperator() == Operator.TRIE_BASED)
                        {
                            root = CheckAnalysisAndSetRoot(candidate.GetName());
                        }

                        if (candidate.GetOperator() == Operator.BACKWARD_MERGE && previousWord != null)
                        {
                            root = CheckAnalysisAndSetRoot(previousWord.GetName() + word.GetName());
                            if (previousPreviousWord != null)
                            {
                                previousRoot = CheckAnalysisAndSetRoot(previousPreviousWord.GetName());
                            }
                        }

                        if (candidate.GetOperator() == Operator.FORWARD_MERGE && nextWord != null)
                        {
                            root = CheckAnalysisAndSetRoot(word.GetName() + nextWord.GetName());
                            if (nextNextWord != null)
                            {
                                nextRoot = CheckAnalysisAndSetRoot(nextNextWord.GetName());
                            }
                        }

                        if (previousRoot != null)
                        {
                            if (candidate.GetOperator() == Operator.SPLIT)
                            {
                                root = CheckAnalysisAndSetRoot(candidate.GetName().Split(" ")[0]);
                            }

                            previousProbability = GetProbability(previousRoot.GetName(), root.GetName());
                        }
                        else
                        {
                            previousProbability = 0.0;
                        }

                        if (nextRoot != null)
                        {
                            if (candidate.GetOperator() == Operator.SPLIT)
                            {
                                root = CheckAnalysisAndSetRoot(candidate.GetName().Split(" ")[1]);
                            }

                            nextProbability = GetProbability(root.GetName(), nextRoot.GetName());
                        }
                        else
                        {
                            nextProbability = 0.0;
                        }

                        if (System.Math.Max(previousProbability, nextProbability) > bestProbability || candidates.Count == 1)
                        {
                            bestCandidate = candidate;
                            bestRoot = root;
                            bestProbability = System.Math.Max(previousProbability, nextProbability);
                        }
                    }

                    if (bestCandidate.GetOperator() == Operator.FORWARD_MERGE)
                    {
                        i++;
                    }

                    if (bestCandidate.GetOperator() == Operator.BACKWARD_MERGE)
                    {
                        result.ReplaceWord(result.WordCount() - 1, new Word(bestCandidate.GetName()));
                    }
                    else
                    {
                        if (bestCandidate.GetOperator() == Operator.SPLIT)
                        {
                            AddSplitWords(bestCandidate.GetName(), result);
                        } 
                        else 
                        {
                            result.AddWord(new Word(bestCandidate.GetName()));
                        }
                    }

                    root = bestRoot;
                }
                else
                {
                    result.AddWord(word);
                }

                previousRoot = root;
                root = nextRoot;
                nextRoot = CheckAnalysisAndSetRootForWordAtIndex(sentence, i + 2);
            }

            return result;
        }
    }
}