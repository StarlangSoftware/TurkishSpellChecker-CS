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
         * <summary>A constructor of {@link NGramSpellChecker} class which takes a {@link FsmMorphologicalAnalyzer} and an {@link NGram}
         * as inputs. Then, calls its super class {@link SimpleSpellChecker} with given {@link FsmMorphologicalAnalyzer} and
         * assigns given {@link NGram} to the nGram variable.</summary>
         *
         * <param name="fsm">  {@link FsmMorphologicalAnalyzer} type input.</param>
         * <param name="nGram">{@link NGram} type input.</param>
         */
        public NGramSpellChecker(FsmMorphologicalAnalyzer fsm, NGram<string> nGram) : base(fsm)
        {
            this._nGram = nGram;
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
            Word previousRoot = null;
            var result = new Sentence();
            for (var i = 0; i < sentence.WordCount(); i++)
            {
                var word = sentence.GetWord(i);
                var fsmParses = fsm.MorphologicalAnalysis(word.GetName());
                if (fsmParses.Size() == 0)
                {
                    var candidates = CandidateList(word);
                    var bestCandidate = word.GetName();
                    var bestRoot = word;
                    double bestProbability = 0;
                    foreach (var candidate in candidates) {
                        fsmParses = fsm.MorphologicalAnalysis(candidate);
                        var root = fsmParses.GetFsmParse(0).GetWord();
                        double probability;
                        if (previousRoot != null)
                        {
                            probability = _nGram.GetProbability(previousRoot.GetName(), root.GetName());
                        }
                        else
                        {
                            probability = _nGram.GetProbability(root.GetName());
                        }

                        if (probability > bestProbability)
                        {
                            bestCandidate = candidate;
                            bestRoot = root;
                            bestProbability = probability;
                        }
                    }
                    previousRoot = bestRoot;
                    result.AddWord(new Word(bestCandidate));
                }
                else
                {
                    result.AddWord(word);
                    previousRoot = fsmParses.GetParseWithLongestRootWord().GetWord();
                }
            }

            return result;
        }
    }
}