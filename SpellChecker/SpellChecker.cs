using Corpus;

namespace SpellChecker
{
    public interface SpellChecker
    {
        /**
         * <summary>The spellCheck method which takes a {@link Sentence} as an input.</summary>
         *
         * <param name="sentence">{@link Sentence} type input.</param>
         * <returns>Sentence result.</returns>
         */
        Sentence SpellCheck(Sentence sentence);
        
    }
}