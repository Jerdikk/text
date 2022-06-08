namespace WpfApp1
{
    public class NeuralNet
    {
        public int numInput;
        public int numHidden;
        public int numOutput;
        public Matrix weightsInput2Hidden;
        public Matrix weightsHidden2Output;

        public NeuralNet(int numInput, int numHidden, int numOutput)
        {
            this.numInput = numInput;
            this.numHidden = numHidden;
            this.numOutput = numOutput;
        }
    }

}
