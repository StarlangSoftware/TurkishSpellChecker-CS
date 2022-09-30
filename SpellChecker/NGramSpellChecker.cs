using Corpus;
using Dictionary.Dictionary;
using MorphologicalAnalysis;
using NGram;

namespace SpellChecker
{
    public class NGramSpellChecker : SimpleSpellChecker
    {
        private readonly NGram<string> _nGram;
        private bool _rootNgram;
        private double _threshold;

        /**
         * <summary>A constructor of {@link NGramSpellChecker} class which takes a {@link FsmMorphologicalAnalyzer} and an {@link NGram}
         * as inputs. Then, calls its super class {@link SimpleSpellChecker} with given {@link FsmMorphologicalAnalyzer} and
         * assigns given {@link NGram} to the nGram variable.</summary>
         *
         * <param name="fsm">  {@link FsmMorphologicalAnalyzer} type input.</param>
         * <param name="nGram">{@link NGram} type input.</param>
         */
        public NGramSpellChecker(FsmMorphologicalAnalyzer fsm, NGram<string> nGram, bool rootNgram) : base(fsm)
        {
            this._nGram = nGram;
            this._rootNgram = rootNgram;
        }

        /**
        * <summary>Checks the morphological analysis of the given word in the given index. If there is no misspelling, it returns
        * the longest root word of the possible analyses.</summary>
        * <param name="sentence"> Sentence to be analyzed.</param>
        * <param name="index"> Index of the word</param>
        * <returns> If the word is misspelled, null; otherwise the longest root word of the possible analyses.</returns>
        */
        private Word CheckAnalysisAndSetRootForWordAtIndex(Sentence sentence, int index)
        {
            if (index < sentence.WordCount())
            {
                var fsmParses = fsm.MorphologicalAnalysis(sentence.GetWord(index).GetName());
                if (fsmParses.Size() != 0)
                {
                    if (_rootNgram)
                    {
                        return fsmParses.GetParseWithLongestRootWord().GetWord();
                    }

                    return sentence.GetWord(index);
                }
            }

            return null;
        }

        private Word CheckAnalysisAndSetRoot(string word)
        {
            var fsmParses = fsm.MorphologicalAnalysis(word);
            if (fsmParses.Size() != 0)
            {
                if (_rootNgram)
                {
                    return fsmParses.GetParseWithLongestRootWord().GetWord();
                }
                else
                {
                    return new Word(word);
                }
            }

            return null;
        }

        public void SetThreshold(double threshold)
        {
            _threshold = threshold;
        }

        private double GetProbability(string word1, string word2)
        {
            return _nGram.GetProbability(word1, word2);
        }

        /**
         * <summary>The spellCheck method takes a {@link Sentence} as an input and loops i times where i ranges from 0 to size of words in given sentence.
         * Then, it calls morphologicalAnalysis method with each word and assigns it to the {@link FsmParseList}, if the size of
         * {@link FsmParseList} is equal to the 0, it adds current word to the candidateList and assigns it to the candidates {@link ArrayList}.
         * <p/>
         * Later on, it loops through candidates {@link ArrayList} and calls morphologicalAnalysis method with each word and
         * assigns it to the {@link FsmParseList}. Then, it gets the root from {@link FsmParseList}. For the first time, it defines a previousRoot
         * by calling getProbability method with root, and for the following times it calls getProbability method with previousRoot and root.
         * Then, it finds out the best probability and the corresponding candidate as best candidate and adds it to the result {@link Sentence}.
         * <p/>
         * If the size of {@link FsmParseList} is not equal to 0, it directly adds the current word to the result {@link Sentence} and finds
         * the previousRoot directly from the {@link FsmParseList}.</summary>
         *
         * <param name="sentence">{@link Sentence} type input.</param>
         * <returns>Sentence result.</returns>
         */
        public new Sentence SpellCheck(Sentence sentence)
        {
            Word previousRoot = null, root, nextRoot;
            double previousProbability, nextProbability, bestProbability;
            var result = new Sentence();
            root = CheckAnalysisAndSetRootForWordAtIndex(sentence, 0);
            nextRoot = CheckAnalysisAndSetRootForWordAtIndex(sentence, 1);
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

                if (ForcedMisspellCheck(word, result)){
                    previousRoot = CheckAnalysisAndSetRootForWordAtIndex(result, result.WordCount() - 1);
                    root = nextRoot;
                    nextRoot = CheckAnalysisAndSetRootForWordAtIndex(sentence, i + 2);
                    continue;
                }
                if (ForcedBackwardMergeCheck(word, result, previousWord)){
                    previousRoot = CheckAnalysisAndSetRootForWordAtIndex(result, result.WordCount() - 1);
                    root = CheckAnalysisAndSetRootForWordAtIndex(sentence, i + 1);
                    nextRoot = CheckAnalysisAndSetRootForWordAtIndex(sentence, i + 2);
                    continue;
                }
                if (ForcedForwardMergeCheck(word, result, nextWord)){
                    i++;
                    previousRoot = CheckAnalysisAndSetRootForWordAtIndex(result, result.WordCount() - 1);
                    root = CheckAnalysisAndSetRootForWordAtIndex(sentence, i + 1);
                    nextRoot = CheckAnalysisAndSetRootForWordAtIndex(sentence, i + 2);
                    continue;
                }
                if (ForcedSplitCheck(word, result) || ForcedShortcutCheck(word, result)){
                    previousRoot = CheckAnalysisAndSetRootForWordAtIndex(result, result.WordCount() - 1);
                    root = nextRoot;
                    nextRoot = CheckAnalysisAndSetRootForWordAtIndex(sentence, i + 2);
                    continue;
                }

                if (root == null)
                {
                    var candidates = CandidateList(word);
                    candidates.AddRange(MergedCandidatesList(previousWord, word, nextWord));
                    candidates.AddRange(SplitCandidatesList(word));
                    var bestCandidate = new Candidate(word.GetName(), Operator.NO_CHANGE);
                    var bestRoot = word;
                    bestProbability = _threshold;
                    foreach (var candidate in candidates)
                    {
                        if (candidate.GetOperator() == Operator.SPELL_CHECK ||
                            candidate.GetOperator() == Operator.MISSPELLED_REPLACE)
                        {
                            root = CheckAnalysisAndSetRoot(candidate.GetName());
                        }

                        if (candidate.GetOperator() == Operator.BACKWARD_MERGE && previousWord != null &&
                            previousPreviousWord != null)
                        {
                            root = CheckAnalysisAndSetRoot(previousWord.GetName() + word.GetName());
                            previousRoot = CheckAnalysisAndSetRoot(previousPreviousWord.GetName());
                        }

                        if (candidate.GetOperator() == Operator.FORWARD_MERGE && nextWord != null &&
                            nextNextWord != null)
                        {
                            root = CheckAnalysisAndSetRoot(word.GetName() + nextWord.GetName());
                            nextRoot = CheckAnalysisAndSetRoot(nextNextWord.GetName());
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

                        if (System.Math.Max(previousProbability, nextProbability) > bestProbability)
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
                        result.ReplaceWord(i - 1, new Word(bestCandidate.GetName()));
                    }
                    else
                    {
                        if (bestCandidate.GetOperator() == Operator.SPLIT){
                            AddSplitWords(bestCandidate.GetName(), result);
                        } else {
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