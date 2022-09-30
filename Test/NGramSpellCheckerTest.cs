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
                new Sentence(
                    "son derece kısıtlı kelimeler çerçevesinde kendilerini uzun cümlelerle ifade edebiliyorlar"),
                new Sentence("minibüs durağı"),
                new Sentence("noter belgesi"),
                new Sentence("")
            };
            Sentence[] modified =
            {
                new Sentence("demokratik cumhüriyet rn kımetli varlıgımızdır"),
                new Sentence("bu tblodaki değerler zedelenmeyecektir"),
                new Sentence("milliyet'in geeneksel yılin spoşcusu ankşti 43. yeşını doldürdu"),
                new Sentence("demokrasinin icşdı buf ayrmıı bulandürdı"),
                new Sentence("dışişleri mütseşarı Öymen'in 1997'nin iljk aylğrında Bağdat'a gitmesi öngörülüyor"),
                new Sentence("büyüdü , palazandı , devltei eöe geçridi"),
                new Sentence("her makenin cültte aklma sürdsi farlkıdır"),
                new Sentence("yılın sno ayında 10 gazteci gözlatına alündı"),
                new Sentence("iki piotun kulçandığı uçkata üir hotes görçv alyıor"),
                new Sentence("son deece kısütlı keilmeler çeçevesinde kendülerini uzuü cümllerle ifüde edbeiliyorlar"),
                new Sentence("minibü durağı"),
                new Sentence("ntoer belgesi"),
                new Sentence("")
            };
            var fsm = new FsmMorphologicalAnalyzer();
            var nGram = new NGram<string>("../../../ngram.txt");
            nGram.CalculateNGramProbabilities(new NoSmoothing<string>());
            var nGramSpellChecker = new NGramSpellChecker(fsm, nGram, true);
            for (var i = 0; i < modified.Length; i++)
            {
                Assert.AreEqual(original[i].ToString(), nGramSpellChecker.SpellCheck(modified[i]).ToString());
            }
        }

        [Test]
        public void TestSpellCheck2()
        {
            Sentence[] original =
            {
                new Sentence("yeni sezon başladı"),
                new Sentence("sırtıkara adındaki canlı , bir balıktır"),
                new Sentence("siyah ayı , ayıgiller familyasına ait bir ayı türüdür"),
                new Sentence("yeni sezon başladı gibi"),
                new Sentence("alışveriş için markete gitti"),
                new Sentence("küçük bir yalıçapkını geçti"),
                new Sentence("meslek odaları birliği yeniden toplandı"),
                new Sentence("yeni yılın sonrasında vakalarda artış oldu"),
                new Sentence("atomik saatin 10 mhz sinyali kalibrasyon hizmetlerinde referans olarak kullanılmaktadır"),
                new Sentence("rehberimiz bu bölgedeki çıngıraklı yılan varlığı hakkında konuştu"),
                new Sentence("bu son model cihaz 24 inç ekran büyüklüğünde ve 9 kg ağırlıktadır")
            };
            Sentence[] modified =
            {
                new Sentence("yenisezon başladı"),
                new Sentence("sırtı kara adındaki canlı , bir balıktır"),
                new Sentence("siyahayı , ayıgiller familyasına ait bir ayı türüdür"),
                new Sentence("yeni se zon başladı gibs"),
                new Sentence("alis veriş için markete gitit"),
                new Sentence("kucuk bri yalı çapkını gecti"),
                new Sentence("mes lek odaları birliği yendien toplandı"),
                new Sentence("yeniyılın sonrasında vakalarda artış oldu"),
                new Sentence("atomik saatin 10mhz sinyali kalibrasyon hizmetlerinde referans olarka kullanılmaktadır"),
                new Sentence("rehperimiz buı bölgedeki çıngıraklıyılan varlıgı hakkınd konustu"),
                new Sentence("bu sno model ciha 24inç ekran büyüklüğünde ve 9kg ağırlıktadır")
            };
            var fsm = new FsmMorphologicalAnalyzer();
            var nGram = new NGram<string>("../../../ngram.txt");
            nGram.CalculateNGramProbabilities(new NoSmoothing<string>());
            var nGramSpellChecker = new NGramSpellChecker(fsm, nGram, true);
            for (var i = 0; i < modified.Length; i++)
            {
                Assert.AreEqual(original[i].ToString(), nGramSpellChecker.SpellCheck(modified[i]).ToString());
            }
        }

        [Test]
        public void TestSpellCheckSurfaceForm()
        {
            var fsm = new FsmMorphologicalAnalyzer();
            var nGram = new NGram<string>("../../../ngram.txt");
            nGram.CalculateNGramProbabilities(new NoSmoothing<string>());
            var nGramSpellChecker = new NGramSpellChecker(fsm, nGram, false);
            Assert.AreEqual("noter hakkında", nGramSpellChecker.SpellCheck(new Sentence("noter hakkınad")).ToString());
            Assert.AreEqual("arçelik'in çamaşır",
                nGramSpellChecker.SpellCheck(new Sentence("arçelik'in çamşaır")).ToString());
            Assert.AreEqual("ruhsat yanında", nGramSpellChecker.SpellCheck(new Sentence("ruhset yanında")).ToString());
        }
    }
}