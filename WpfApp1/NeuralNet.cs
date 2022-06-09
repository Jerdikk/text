using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class NeuralNet
    {
        public int numInput;
        public int numHidden;
        public int numOutput;

        public double learningRate;

        public Matrix weightsInput2Hidden;
        public Matrix weightsHidden2Output;

        public NeuralNet(int numInput, int numHidden, int numOutput)
        {
            this.numInput = numInput;
            this.numHidden = numHidden;
            this.numOutput = numOutput;
            weightsInput2Hidden = new Matrix(numInput,numHidden);
            var rand = new Random();
            double drRnd;
            for (int i = 0; i < numInput; i++)
                for (int j = 0; j < numHidden; j++)
                {
                    drRnd = (double)rand.Next(-100, 100) / 100.0;
                    if (drRnd == 0.0) drRnd = 0.01;
                    weightsInput2Hidden.elements[i,j] = drRnd;
                }
            weightsHidden2Output = new Matrix(numHidden,numOutput);
            for (int i = 0; i < numHidden; i++)
                for (int j = 0; j < numOutput; j++)
                {
                    drRnd = (double)rand.Next(-100, 100) / 100.0;
                    if (drRnd == 0.0) drRnd = 0.01;
                    weightsHidden2Output.elements[i, j] = drRnd;
                }

        }



    }

}
