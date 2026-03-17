Turkish Spell Checker
============

This tool is a spelling checker for Modern Turkish. It detects spelling errors and corrects them appropriately, through its list of misspellings and matching to the Turkish dictionary.

Simple Web Interface
============

[Link 1](http://104.247.163.162/nlptoolkit/turkish-spell-checker.html) [Link 2](https://starlangsoftware.github.io/nlptoolkit-web-simple/turkish-spell-checker.html)

Video Lectures
============

[<img src="https://github.com/StarlangSoftware/TurkishSpellChecker/blob/master/video.jpg" width="50%">](https://youtu.be/wKwTKv6Jlo0)

For Developers
============

You can also see [Java](https://github.com/starlangsoftware/TurkishSpellChecker), [Python](https://github.com/starlangsoftware/TurkishSpellChecker-Py), [Cython](https://github.com/starlangsoftware/TurkishSpellChecker-Cy), [Swift](https://github.com/starlangsoftware/TurkishSpellChecker-Swift), [Js](https://github.com/starlangsoftware/TurkishSpellChecker-Js), [C](https://github.com/starlangsoftware/TurkishSpellChecker-C), or [C++](https://github.com/starlangsoftware/TurkishSpellChecker-CPP) repository.

## Requirements

* C# Editor
* [Git](#git)

### Git

Install the [latest version of Git](https://git-scm.com/book/en/v2/Getting-Started-Installing-Git).

## Download Code

In order to work on code, create a fork from GitHub page. 
Use Git for cloning the code to your local or below line for Ubuntu:

	git clone <your-fork-git-link>

A directory called TurkishSpellChecker-CS will be created. Or you can use below link for exploring the code:

	git clone https://github.com/starlangsoftware/TurkishSpellChecker-CS.git

## Open project with Rider IDE

To import projects from Git with version control:

* Open Rider IDE, select Get From Version Control.

* In the Import window, click URL tab and paste github URL.

* Click open as Project.

Result: The imported project is listed in the Project Explorer view and files are loaded.


## Compile

**From IDE**

After being done with the downloading and opening project, select **Build Solution** option from **Build** menu. After compilation process, user can run TurkishSpellChecker-CS.

Detailed Description
============

+ [Creating SpellChecker](#creating-spellchecker)
+ [Spell Correction](#spell-correction)

## Creating SpellChecker

SpellChecker finds spelling errors and corrects them in Turkish. There are two types of spell checker available:

* `SimpleSpellChecker`
    
    * To instantiate this, a `FsmMorphologicalAnalyzer` is needed. 
        
            FsmMorphologicalAnalyzer fsm = new FsmMorphologicalAnalyzer();
            SpellChecker spellChecker = new SimpleSpellChecker(fsm);   
     
* `NGramSpellChecker`,
    
    * To create an instance of this, both a `FsmMorphologicalAnalyzer` and a `NGram` is required. 
    
    * `FsmMorphologicalAnalyzer` can be instantiated as follows:
        
            FsmMorphologicalAnalyzer fsm = new FsmMorphologicalAnalyzer();
    
    * `NGram` can be either trained from scratch or loaded from an existing model.
        
        * Training from scratch:
                
                Corpus corpus = new Corpus("corpus.txt"); 
                NGram ngram = new NGram(corpus.GetAllWordsAsArrayList(), 1);
                ngram.CalculateNGramProbabilities(LaplaceSmoothing());
                
        *There are many smoothing methods available. For other smoothing methods, check [here](https://github.com/olcaytaner/NGram).*       
        * Loading from an existing model:
     
                NGram ngram = new NGram("ngram.txt");

	*For further details, please check [here](https://github.com/StarlangSoftware/NGram).*        
            
    * Afterwards, `NGramSpellChecker` can be created as below:
        
            SpellChecker spellChecker = new NGramSpellChecker(fsm, ngram);
     

## Spell Correction

Spell correction can be done as follows:

    Sentence sentence = new Sentence("Dıktor olaç yazdı");
    Sentence corrected = spellChecker.SpellCheck(sentence);
    Console.WriteLine(corrected);
    
Output:

    Doktor ilaç yazdı

For Contibutors
============

### Resources
1. Add resources to the project directory. Do not forget to choose 'EmbeddedRecource' in 'Build Action' and 'Copy always' in 'Copy to output directory' in File Properties dialog. 
   
### C# files
1. Do not forget to comment each function.
```
	/**
	* <summary>Returns the first literal's name.</summary>
	*
	* <returns>the first literal's name.</returns>
	*/
	public string Representative()
	{
		return GetSynonym().GetLiteral(0).GetName();
	}
```
2. Function names should follow pascal caml case.
```
	public string GetLongDefinition()
```
3. Write ToString methods, if necessary.
4. Use var type as a standard type.
```
	public override bool Equals(object second)
	{
		var relation = (Relation) second;
```
5. Use standard naming for private and protected class variables. Use _ for private and capital for protected class members.
```
    public class SynSet
    {
        private string _id;
		protected string Name;
```
6. Use NUnit for writing test classes. Use test setup if necessary.
```
   public class WordNetTest
    {
        WordNet.WordNet turkish;

        [SetUp]
        public void Setup()
        {
            turkish = new WordNet.WordNet();
        }

        [Test]
        public void TestSynSetList()
        {
            var literalCount = 0;
            foreach (var synSet in turkish.SynSetList()){
                literalCount += synSet.GetSynonym().LiteralSize();
            }
            Assert.AreEqual(110259, literalCount);
        }
```
