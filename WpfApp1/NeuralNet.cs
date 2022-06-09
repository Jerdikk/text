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

        public Matrix CalcNet(Matrix input)
        {
            Matrix res = new Matrix(1,1);
            Matrix inputs = input.T();
            Matrix hidden_inputs = this.weightsInput2Hidden * inputs;
            Matrix hidden_outputs = hidden_inputs.Sigmoid();
  /*          inputs = numpy.array(inputs_list, ndmin = 2).T
        hidden_inputs = numpy.dot(self.wih, inputs)
        hidden_outputs = self.activation_function(hidden_inputs)
        final_inputs = numpy.dot(self.who, hidden_outputs)
        final_outputs = self.activation_function(final_inputs)
*/
            return res;
        }



    }

}
