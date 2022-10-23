using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    [Serializable]
    public class NeuralNet
    {
        public int numInput;
        public int numHidden;
        public int numOutput;

        public float learningRate;
        public int numEpoch;

        public Matrix weightsInput2Hidden;
        public Matrix weightsHidden2Output;



        public NeuralNet(int numInput, int numHidden, int numOutput, int numEpoch, float learningRate)
        {
            this.numInput = numInput;
            this.numHidden = numHidden;
            this.numOutput = numOutput;
            this.learningRate = learningRate;
            weightsInput2Hidden = new Matrix(numInput, numHidden);
            var rand = new Random();
            float drRnd;
            for (int i = 0; i < numInput; i++)
                for (int j = 0; j < numHidden; j++)
                {
                    drRnd = (float)rand.Next(-99, 99) / 100.0f;
                    if (Math.Abs(drRnd) < 0.001) drRnd = 0.01f;
                    weightsInput2Hidden.elements[i, j] = drRnd;
                }
            weightsHidden2Output = new Matrix(numHidden, numOutput);
            for (int i = 0; i < numHidden; i++)
                for (int j = 0; j < numOutput; j++)
                {
                    drRnd = (float)rand.Next(-99, 99) / 100.0f;
                    if (Math.Abs(drRnd) < 0.001) drRnd = 0.01f;
                    weightsHidden2Output.elements[i, j] = drRnd;
                }

            this.numEpoch = numEpoch;
        }

        public Matrix CalcNet(Matrix inputs)
        {
            Matrix hidden_outputs;
            Matrix output;

            hidden_outputs = inputs * this.weightsInput2Hidden;
            hidden_outputs.Sigmoid();
            output = hidden_outputs * this.weightsHidden2Output;
            output.Sigmoid();

            return output;
        }

        Matrix OptimizeErrorFunc1(Matrix a, Matrix b)
        {
            float temp;
            Matrix res = new Matrix(a.Rows, a.Cols);

            for (int i = 0; i < a.Rows; i++)
                for (int j = 0; j < a.Cols; j++)
                {
                    temp = a.elements[i, j];
                    res.elements[i, j] = b.elements[i, j] * temp * (1.0f - temp);
                }
            return res;
        }

        Matrix OptimizeErrorFunc2(Matrix a, Matrix b)
        {
            float temp;
            Matrix res = new Matrix(a.Rows, a.Cols);

            for (int i = 0; i < a.Rows; i++)
                for (int j = 0; j < a.Cols; j++)
                {
                    temp = a.elements[i, j];
                    res.elements[i, j] = b.elements[j, i] * temp * (1.0f - temp);
                }
            return res;
        }


        public void TrainNet(Matrix inputs, Matrix targets)
        {
            Matrix hidden_outputs;

            Matrix final_outputs;
            Matrix output_errors;

            Matrix hidden_errors;

            hidden_outputs = inputs * this.weightsInput2Hidden;
            hidden_outputs.Sigmoid();
            final_outputs = hidden_outputs * this.weightsHidden2Output;
            final_outputs.Sigmoid();

            output_errors = targets - final_outputs;

            hidden_errors = this.weightsHidden2Output * output_errors.Transpose();

            this.weightsHidden2Output.AddWithMulLR(hidden_outputs.Transpose() * OptimizeErrorFunc1(final_outputs, output_errors), this.learningRate);
            this.weightsInput2Hidden.AddWithMulLR(inputs.Transpose() * OptimizeErrorFunc2(hidden_outputs, hidden_errors/*.Transpose()*/), this.learningRate);

        }

        public void Save(string fname)
        {
            // BinaryFormatter сохраняет данные в двоичном формате. Чтобы получить доступ к BinaryFormatter, понадобится
            // импортировать System.Runtime.Serialization.Formatters.Binary
            BinaryFormatter binFormat = new BinaryFormatter();
            // Сохранить объект в локальном файле.
            using (Stream fStream = new FileStream(fname,
               FileMode.Create, FileAccess.Write, FileShare.None))
            {
                binFormat.Serialize(fStream, this);
            }
        }

        public static NeuralNet Load(string fname)
        {
            //NeuralNet neuralNet;
            if (File.Exists(fname))
            {

                BinaryFormatter binFormat = new BinaryFormatter();

                using (Stream fStream = File.OpenRead(fname))
                {
                    return (NeuralNet)binFormat.Deserialize(fStream);
                }
            }
            else
                return null;
            //return neuralNet;
        }

    }


   
}
