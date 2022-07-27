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
        public int numEpoch;

        public Matrix weightsInput2Hidden;
        public Matrix weightsHidden2Output;

        public NeuralNet(int numInput, int numHidden, int numOutput, int numEpoch)
        {
            this.numInput = numInput;
            this.numHidden = numHidden;
            this.numOutput = numOutput;
            this.learningRate = 0.1;
            weightsInput2Hidden = new Matrix(numInput, numHidden);
            var rand = new Random();
            double drRnd;
            for (int i = 0; i < numInput; i++)
                for (int j = 0; j < numHidden; j++)
                {
                    drRnd = (double)rand.Next(-100, 100) / 100.0;
                    if (drRnd == 0.0) drRnd = 0.01;
                    weightsInput2Hidden.elements[i, j] = drRnd;
                }
            weightsHidden2Output = new Matrix(numHidden, numOutput);
            for (int i = 0; i < numHidden; i++)
                for (int j = 0; j < numOutput; j++)
                {
                    drRnd = (double)rand.Next(-100, 100) / 100.0;
                    if (drRnd == 0.0) drRnd = 0.01;
                    weightsHidden2Output.elements[i, j] = drRnd;
                }

            this.numEpoch = numEpoch;
        }

        public Matrix CalcNet(Matrix input)
        {
            Matrix inputs = input.T();
            Matrix hidden_inputs = inputs * this.weightsInput2Hidden;
            Matrix hidden_outputs = hidden_inputs.Sigmoid();
            Matrix output_input = hidden_outputs*this.weightsHidden2Output ;
            Matrix output = output_input.Sigmoid();

            return output;
        }

        public void TrainNet(Matrix input, Matrix target)
        {
            Matrix inputs = input.T();
            Matrix targets = target.T();

            Matrix hidden_inputs = inputs * this.weightsInput2Hidden;
            Matrix hidden_outputs = hidden_inputs.Sigmoid();
            Matrix output_input = hidden_outputs * this.weightsHidden2Output;
            Matrix final_outputs = output_input.Sigmoid();
            Matrix output_errors = targets - final_outputs;

            Matrix hidden_errors = output_errors*this.weightsHidden2Output.T() ;

            Matrix temp = 1.0 - final_outputs;
            temp = Matrix.mulAdamar(final_outputs, temp);
            temp= Matrix.mulAdamar(output_errors , temp);
            temp = temp.T() * hidden_outputs;
            temp = this.learningRate * temp;
            this.weightsHidden2Output = this.weightsHidden2Output + temp.T();

            temp = 1.0 - hidden_outputs;
            temp = Matrix.mulAdamar(hidden_outputs, temp);
            temp = Matrix.mulAdamar(hidden_errors, temp);
            temp = temp.T()*inputs;
            temp = this.learningRate * temp;
            this.weightsInput2Hidden = this.weightsInput2Hidden + temp.T();

        }

    }

}
