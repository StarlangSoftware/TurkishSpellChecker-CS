namespace SpellChecker
{
    public class SpellCheckerParameter
    {
        private double _threshold = 0.0;
        private bool _deMiCheck = true;
        private bool _rootNGram = true;
        
        public void SetThreshold(double threshold)
        {
            _threshold = threshold;
        }
        public void SetDeMiCheck(bool deMiCheck)
        {
            _deMiCheck = deMiCheck;
        }
        public void SetRootNGram(bool rootNGram)
        {
            _rootNGram = rootNGram;
        }
        public double GetThreshold()
        {
            return _threshold;
        }
        public bool DeMiCheck()
        {
            return _deMiCheck;
        }
        public bool IsRootNGram()
        {
            return _rootNGram;
        }
    }
    
}