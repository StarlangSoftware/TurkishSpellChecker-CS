using Corpus;
using MorphologicalAnalysis;
using NGram;
using NUnit.Framework;
using SpellChecker;

namespace Test
{
    public class NGramSpellCheckerTest
    {
        [Test]
        public void TestSpellCheck()
        {
            Sentence[] original =
            {
                new Sentence("demokratik cumhuriyet en kıymetli varlığımızdır"),
                new Sentence("bu tablodaki değerler zedelenmeyecektir"),
                new Sentence("milliyet'in geleneksel yılın sporcusu anketi 43. yaşını doldurdu"),
                new Sentence("demokrasinin icadı bu ayrımı bulandırdı"),
                new Sentence("dışişleri müsteşarı Öymen'in 1997'nin ilk aylarında Bağdat'a gitmesi öngörülüyor"),
                new Sentence("büyüdü , palazlandı , devleti ele geçirdi"),
                new Sentence("her maskenin ciltte kalma süresi farklıdır"),
                new Sentence("yılın son ayında 10 gazeteci gözaltına alındı"),
                new Sentence("iki pilotun kullandığı uçakta bir hostes görev alıyor"),
            };
            Sentence[] modified =
            {
                new Sentence("demokratik cumhüriyet en krymetli varlıgımızdır"),
                new Sentence("bu tblodaki değerlğr zedelenmeyecüktir"),
                new Sentence("milliyet'in gileneksel yılın spoşcusu ankşti 43. yaşinı doldürdu"),
                new Sentence("demokrasinin icşdı bu ayrmıı bulandürdı"),
                new Sentence("dışişleri mütseşarı Öymen'in 1997'nin ilk aylğrında Bağdat'a gitmesi öngşrülüyor"),
                new Sentence("büyüdü , palazlandıü , devltei ele geçridi"),
                new Sentence("her müskenin cşltte kalma sürdsi farlkıdır"),
                new Sentence("yılın son ayında 10 gaeteci gözlatına alındı"),
                new Sentence("iki piltun kulçandığı uçkata bir hotes görçv alyıor"),
            };
            var fsm = new FsmMorphologicalAnalyzer();
            var nGram = new NGram<string>("../../../ngram.txt");
            nGram.CalculateNGramProbabilities(new NoSmoothing<string>());
            var nGramSpellChecker = new NGramSpellChecker(fsm, nGram);
            for (var i = 0; i < modified.Length; i++)
            {
                Assert.AreEqual(original[i].ToString(), nGramSpellChecker.SpellCheck(modified[i]).ToString());
            }
        }
    }
}