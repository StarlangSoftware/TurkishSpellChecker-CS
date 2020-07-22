using System.IO;
using Corpus;
using MorphologicalAnalysis;
using NUnit.Framework;
using SpellChecker;

namespace Test
{
    public class SimpleSpellCheckerTest
    {

        [Test]
        public void TestSpellCheck()
        {
            var fsm = new FsmMorphologicalAnalyzer();
            var simpleSpellChecker = new SimpleSpellChecker(fsm);
            var input = new StreamReader("../../../misspellings.txt");
            var line = input.ReadLine();
            while (line != null){
                var items = line.Split(" ");
                var misspelled = items[0];
                var corrected = items[1];
                Assert.AreEqual(corrected, simpleSpellChecker.SpellCheck(new Sentence(misspelled)).ToString());
                line = input.ReadLine();
            }
            input.Close();
        }
    }
}