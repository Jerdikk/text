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


        Matrix hidden_outputs;
        Matrix output;
        Matrix final_outputs;
        Matrix output_errors;

        Matrix hidden_errors;


        public NeuralNet(int numInput, int numHidden, int numOutput, int numEpoch, double learningRate)
        {
            this.numInput = numInput;
            this.numHidden = numHidden;
            this.numOutput = numOutput;
            this.learningRate = learningRate;
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

        public Matrix CalcNet(Matrix inputs)
        {
            hidden_outputs = inputs * this.weightsInput2Hidden;
            hidden_outputs.Sigmoid();
            output = hidden_outputs * this.weightsHidden2Output;
            output.Sigmoid();

            return output;
        }

        Matrix OptimizeErrorFunc(Matrix a, Matrix b)
        {
            double temp;
            if ((a.Rows != b.Rows) || (a.Cols != b.Cols))
                return null;
            Matrix res = new Matrix(a.Rows, a.Cols);
            for (int i = 0; i < a.Rows; i++)
                for (int j = 0; j < a.Cols; j++)
                {
                    temp = a.elements[i, j];
                    res.elements[i, j] = b.elements[i, j] * temp * (1.0 - temp);
                }
            return res;
        }

        public void TrainNet(Matrix inputs, Matrix targets)
        {
            final_outputs = CalcNet(inputs);
            output_errors = targets - final_outputs;

            hidden_errors = this.weightsHidden2Output * output_errors.Transpose();

            this.weightsHidden2Output.AddWithMulLR(hidden_outputs.Transpose() * OptimizeErrorFunc(final_outputs, output_errors), this.learningRate);
            this.weightsInput2Hidden.AddWithMulLR(inputs.Transpose() * OptimizeErrorFunc(hidden_outputs, hidden_errors.Transpose()), this.learningRate);

        }

    }

}
