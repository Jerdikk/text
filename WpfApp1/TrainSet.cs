using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace WpfApp1
{
    [System.Serializable]
    public class TrainSet
    {
        public int dimTrainSet;
        public int dimInputSet;
        public List<Matrix> mTrainSet;
        public List<Matrix> mInputSet;
        //public List<int> controlDigit;

        public TrainSet(int dimTrainSet, int dimInputSet)
        {
            this.dimTrainSet = dimTrainSet;
            this.dimInputSet = dimInputSet;
            this.mInputSet = new List<Matrix>();
            this.mTrainSet = new List<Matrix>();
            //this.controlDigit = new List<int>();
        }
        public void ClearTrainSet()
        {
            this.mTrainSet.Clear();
            this.mInputSet.Clear();
            //this.controlDigit.Clear();
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

        public static TrainSet Load(string fname)
        {
            if (File.Exists(fname))
            {
                TrainSet trainSet;
                BinaryFormatter binFormat = new BinaryFormatter();

                using (Stream fStream = File.OpenRead(fname))
                {
                    trainSet = (TrainSet)binFormat.Deserialize(fStream);
                }
                return trainSet;
            }
            else
                return null;
        }

    }
}
