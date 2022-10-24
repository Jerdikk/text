
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp1
{
    [Serializable]
    public class WordRange
    {
        public string letter;
        public int startID;
        public int endID;
        public List<WordRange> wordRanges;
    }

    [Serializable]
    public class DictWord
    {
        public int id;
        public string word;
        public string type;
        public bool isMainWord;
    }
    [Serializable]
    public class Node
    {
        // public int temp1;
        public string Letter;
        //  public string wholeWord;
        // public string type;
        public int id;

        //  public List<string> Variants;
        public List<Node> Nodes;
        public List<DictWord> dictWords;

        public Node()
        {
        }

    }
    [Serializable]
    public class MyDict
    {
        public int globalID;
        public List<Node> tree;

        public MyDict()
        {
            globalID = 0;
            tree = new List<Node>();
        }
    }

    [Serializable]
    public class MainDict
    {
        public List<WordRange> listWordRange;
        public List<DictWord> dicts;
        public MainDict()
        {
            listWordRange = null;
            dicts = new List<DictWord>();
        }
    }
    public class MyDataContext
    {

        public ObservableCollection<string> strings { get; set; }

        public MyDataContext()
        {
            strings = new ObservableCollection<string>();

        }
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int GlobalCounter = 0;
        //public MyDict myDict;
        public MainDict loadedMyDict;
        public MyDataContext myDataContext = new MyDataContext();
        public TrainSet trainSet;
        public string globalFile;

        public MainWindow()
        {

            InitializeComponent();

            Binding binding = new Binding();
            binding.Source = myDataContext;
            binding.Path = new PropertyPath("strings");
            binding.Mode = BindingMode.TwoWay;

            lbTest.SetBinding(ListBox.ItemsSourceProperty, binding);

            // myDict = new MyDict();
        }

        private string ChangeUTF8Space(string targetStr)
        {
            try
            {
                string currentStr = string.Empty;
                byte[] utf8Space = new byte[] { 0xc2, 0xa0 };
                string tempSpace = Encoding.GetEncoding("UTF-8").GetString(utf8Space);
                currentStr = targetStr.Replace(tempSpace, " ");
                return currentStr;
            }
            catch (Exception ex)
            {
                return targetStr;
            }
        }

        private void load_Click(object sender, RoutedEventArgs e)
        {
            string[] allText;
            string[] tokens;

            string tempLine = "";
            string tempStr = "";

            char[] chars = new char[3];
            chars[0] = '.';

            chars[1] = '!';
            chars[2] = '?';

            List<string> sentList = new List<string>();
            List<string> tokenList = new List<string>();
            List<int> countToken = new List<int>();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                int cc = 0;
                //allText = File.ReadAllLines(openFileDialog.FileName, Encoding.GetEncoding(1251));
                allText = File.ReadAllLines(openFileDialog.FileName, Encoding.GetEncoding("utf-8"));
                foreach (string line in allText)
                {
                    tempStr = "";
                    tempLine += line;
                    tokens = tempLine.Split(chars);
                    int lenSents = tokens.Length;
                    if (lenSents > 1)
                    {
                        for (int i = 0; i < (lenSents - 1); i++)
                        {
                            tempStr = tokens[i].Replace("—", "");
                            tempStr = tempStr.Replace(",", "");
                            tempStr = tempStr.Replace("…", "");
                            tempStr = tempStr.Replace(";", "");
                            tempStr = tempStr.Replace(":", "");
                            tempStr = ChangeUTF8Space(tempStr);
                            tempStr = tempStr.ToLower();
                            sentList.Add(tempStr);
                        }
                        tempLine = tokens[lenSents - 1];
                    }
                }
                sentList.Add(tempLine);

                foreach (string sentense in sentList)
                {
                    tokens = sentense.Split(' ');
                    foreach (string token in tokens)
                    {
                        string tempTok = token.Trim();
                        if (tempTok.Length > 0)
                        {

                            tokenList.Add(tempTok);


                            bool found1 = false;
                            foreach (WordRange wordRange in loadedMyDict.listWordRange)
                            {
                                if (wordRange.letter == tempTok.Substring(0, 1))
                                {
                                    if (tempTok.Length > 1)
                                    {
                                        if (wordRange.wordRanges.Count > 0)
                                        {
                                            foreach (WordRange wordRange1 in wordRange.wordRanges)
                                            {
                                                if (wordRange1.letter == tempTok.Substring(1, 1))
                                                {
                                                    bool found = false;
                                                    for (int i = wordRange1.startID; i <= wordRange1.endID; i++)
                                                    {
                                                        if (loadedMyDict.dicts[i].word == tempTok)
                                                        {
                                                            countToken.Add(i);
                                                            found = true;
                                                            found1 = true;
                                                            break;
                                                        }
                                                    }
                                                    if (found)
                                                        break;
                                                }
                                            }
                                            if (found1)
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        for (int i = wordRange.startID; i <= wordRange.endID; i++)
                                        {
                                            if (loadedMyDict.dicts[i].word == tempTok)
                                            {
                                                countToken.Add(i);

                                                found1 = true;
                                                break;
                                            }
                                        }
                                        if (found1)
                                            break;

                                    }
                                }
                            }
                            if (!found1)
                                countToken.Add(-1);
                            /*
                            int t = tokenList.FindIndex(x => x == token);
                            if (t == -1)
                            {
                                tokenList.Add(token.Trim());
                                countToken.Add(1);
                            }
                            else
                                countToken[t] += 1;*/
                        }

                    }
                }
                for (int j = 0; j < countToken.Count; j++)
                {
                    tokenList[j] += ";";
                    tokenList[j] += countToken[j].ToString();
                }
                File.WriteAllLines("2.txt", sentList);
                File.WriteAllLines("3.csv", tokenList);
                MessageBox.Show("ВСЕ!");
            }
        }

        public void GetMainDict()
        {
            string[] allText;
            string[] tokens;

            char[] chars = new char[3];
            chars[0] = '.';

            chars[1] = '!';
            chars[2] = '?';

            List<string> sentList = new List<string>();
            List<string> tokenList = new List<string>();
            List<int> countToken = new List<int>();

            int localCounter = 0;

            MainDict mainDict = new MainDict();

            ////                             t = myDict.tree.FindIndex(x => x.Letter == line.Substring(0, 1));
            int cc = 0;
            int t;
            int u;
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
              (ThreadStart)delegate ()
              {
                  myDataContext.strings.Add("Start!");
              }
              );

            allText = File.ReadAllLines(globalFile, Encoding.GetEncoding(1251));

            int sstart = 0;
            int currentListRangeIndex = 0;
            int currentLevel2ListRangeIndex = 0;

            foreach (string line1 in allText)
            {
                if (localCounter % 100 == 0)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                      (ThreadStart)delegate ()
                      {
                          myDataContext.strings.Add(localCounter.ToString());
                      }
                      );

                }

                tokens = line1.Split(',');

                int lenn = tokens[0].Length;

                string line = tokens[0].ToLower();

                if (mainDict.listWordRange == null)
                {
                    currentListRangeIndex = 0;
                    mainDict.listWordRange = new List<WordRange>();
                    WordRange wordRange = new WordRange();
                    wordRange.letter = line.Substring(0, 1);
                    if (lenn > 1)
                    {
                        if (wordRange.wordRanges == null)
                        {
                            wordRange.wordRanges = new List<WordRange>();
                            WordRange wordRange1 = new WordRange();
                            wordRange1.letter = line.Substring(1, 1);
                            wordRange1.startID = mainDict.dicts.Count;
                            wordRange1.endID = mainDict.dicts.Count;
                            currentLevel2ListRangeIndex = 0;
                            wordRange.wordRanges.Add(wordRange1);
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        wordRange.wordRanges = null;
                    }
                    wordRange.startID = mainDict.dicts.Count;
                    wordRange.endID = mainDict.dicts.Count;
                    mainDict.listWordRange.Add(wordRange);
                }
                else
                {
                    if (mainDict.listWordRange[/*mainDict.listWordRange.Count - 1*/currentListRangeIndex].letter != line.Substring(0, 1))
                    {
                        mainDict.listWordRange[/*mainDict.listWordRange.Count - 1*/currentListRangeIndex].endID = mainDict.dicts.Count;
                        if (mainDict.listWordRange.Count > 1)
                        {
                            bool foundIndex = false;
                            for (int h = 0; h < mainDict.listWordRange.Count; h++)
                            {
                                if (mainDict.listWordRange[h].letter == line.Substring(0, 1))
                                {
                                    currentListRangeIndex = h;
                                    foundIndex = true;
                                    break;
                                }
                            }

                            if (foundIndex)
                            {
                                WordRange wordRange = mainDict.listWordRange[currentListRangeIndex];
                                // wordRange.letter = line.Substring(0, 1);
                                //wordRange.startID = mainDict.dicts.Count;
                                wordRange.endID = mainDict.dicts.Count;
                                //mainDict.listWordRange.Add(wordRange);
                                if (lenn > 1)
                                {
                                    if (wordRange.wordRanges == null)
                                    {
                                        wordRange.wordRanges = new List<WordRange>();
                                        WordRange wordRange1 = new WordRange();
                                        wordRange1.letter = line.Substring(1, 1);
                                        wordRange1.startID = mainDict.dicts.Count;
                                        wordRange1.endID = mainDict.dicts.Count;
                                        currentLevel2ListRangeIndex = 0;
                                        wordRange.wordRanges.Add(wordRange1);
                                    }
                                    else
                                    {
                                        bool foundIndex1 = false;
                                        int indexx = 0;
                                        for (int l = 0; l < wordRange.wordRanges.Count; l++)
                                        {
                                            if (wordRange.wordRanges[l].letter == line.Substring(1, 1))
                                            {
                                                foundIndex1 = true;
                                                indexx = l;
                                                break;
                                            }
                                            else
                                            {

                                            }
                                        }
                                        if (foundIndex1)
                                        {
                                            wordRange.wordRanges[indexx].endID = mainDict.dicts.Count;
                                            currentLevel2ListRangeIndex = indexx;
                                        }
                                        else
                                        {
                                            WordRange wordRange1 = new WordRange();
                                            wordRange1.letter = line.Substring(1, 1);
                                            wordRange1.startID = mainDict.dicts.Count;
                                            wordRange1.endID = mainDict.dicts.Count;
                                            wordRange.wordRanges.Add(wordRange1);
                                            currentLevel2ListRangeIndex = wordRange.wordRanges.Count - 1;

                                        }
                                    }
                                }
                                else
                                    wordRange.wordRanges = null;

                            }
                            else
                            {
                                WordRange wordRange = new WordRange();
                                wordRange.letter = line.Substring(0, 1);
                                wordRange.startID = mainDict.dicts.Count;
                                wordRange.endID = mainDict.dicts.Count;
                                mainDict.listWordRange.Add(wordRange);
                                currentListRangeIndex = mainDict.listWordRange.Count - 1;
                                if (lenn > 1)
                                {
                                    if (wordRange.wordRanges == null)
                                    {
                                        wordRange.wordRanges = new List<WordRange>();
                                        WordRange wordRange1 = new WordRange();
                                        wordRange1.letter = line.Substring(1, 1);
                                        wordRange1.startID = mainDict.dicts.Count;
                                        wordRange1.endID = mainDict.dicts.Count;
                                        wordRange.wordRanges.Add(wordRange1);
                                        currentLevel2ListRangeIndex = 0;
                                    }
                                    else
                                    {

                                    }
                                }
                                else
                                    wordRange.wordRanges = null;

                            }

                        }
                        else
                        {
                            WordRange wordRange = new WordRange();
                            wordRange.letter = line.Substring(0, 1);
                            wordRange.startID = mainDict.dicts.Count;
                            wordRange.endID = mainDict.dicts.Count;
                            mainDict.listWordRange.Add(wordRange);
                            currentListRangeIndex = mainDict.listWordRange.Count - 1;
                            if (lenn > 1)
                            {
                                if (wordRange.wordRanges == null)
                                {
                                    wordRange.wordRanges = new List<WordRange>();
                                    WordRange wordRange1 = new WordRange();
                                    wordRange1.letter = line.Substring(1, 1);
                                    wordRange1.startID = mainDict.dicts.Count;
                                    wordRange1.endID = mainDict.dicts.Count;
                                    wordRange.wordRanges.Add(wordRange1);
                                    currentLevel2ListRangeIndex = 0;
                                }
                                else
                                {

                                }
                            }
                            else
                                wordRange.wordRanges = null;
                        }
                    }
                    else
                    {
                        if (lenn > 1)
                        {
                            if (mainDict.listWordRange[currentListRangeIndex].wordRanges != null)
                            {
                                if (mainDict.listWordRange[currentListRangeIndex].wordRanges[currentLevel2ListRangeIndex].letter != line.Substring(1, 1))
                                {
                                    WordRange wordRange = mainDict.listWordRange[currentListRangeIndex];

                                    bool foundIndex1 = false;
                                    int indexx = 0;
                                    for (int l = 0; l < wordRange.wordRanges.Count; l++)
                                    {
                                        if (wordRange.wordRanges[l].letter == line.Substring(1, 1))
                                        {
                                            foundIndex1 = true;
                                            indexx = l;
                                            break;
                                        }
                                        else
                                        {

                                        }
                                    }
                                    if (foundIndex1)
                                    {
                                        wordRange.wordRanges[indexx].endID = mainDict.dicts.Count;
                                        currentLevel2ListRangeIndex = indexx;
                                    }
                                    else
                                    {
                                        WordRange wordRange1 = new WordRange();
                                        wordRange1.letter = line.Substring(1, 1);
                                        wordRange1.startID = mainDict.dicts.Count;
                                        wordRange1.endID = mainDict.dicts.Count;
                                        wordRange.wordRanges.Add(wordRange1);
                                        currentLevel2ListRangeIndex = wordRange.wordRanges.Count - 1;
                                    }


                                    /*
                                    WordRange wordRange1 = new WordRange();
                                    wordRange1.letter = line.Substring(1, 1);
                                    wordRange1.startID = mainDict.dicts.Count;
                                    wordRange1.endID = mainDict.dicts.Count;
                                    wordRange.wordRanges.Add(wordRange1);
                                    currentLevel2ListRangeIndex = 0;*/
                                }
                                else
                                {
                                    mainDict.listWordRange[currentListRangeIndex].wordRanges[currentLevel2ListRangeIndex].endID = mainDict.dicts.Count;
                                }
                            }
                            else
                            {
                                WordRange wordRange = mainDict.listWordRange[currentListRangeIndex];
                                if (wordRange.wordRanges == null)
                                {
                                    wordRange.wordRanges = new List<WordRange>();
                                    WordRange wordRange1 = new WordRange();
                                    wordRange1.letter = line.Substring(1, 1);
                                    wordRange1.startID = mainDict.dicts.Count;
                                    wordRange1.endID = mainDict.dicts.Count;
                                    wordRange.wordRanges.Add(wordRange1);
                                    currentLevel2ListRangeIndex = 0;
                                }
                                else
                                {
                                    wordRange.wordRanges[currentLevel2ListRangeIndex].endID = mainDict.dicts.Count;
                                }

                            }
                        }
                        mainDict.listWordRange[currentListRangeIndex].endID = mainDict.dicts.Count;
                    }

                }
                t = -1;
                if (mainDict.dicts.Count >= 1)
                {
                    /* sstart = -1;
                     eend=-1;
                     for(int i = 0; i < listWordRange.Count; i++)
                     {
                         if(listWordRange[i].letter == line.Substring(0, 1))
                         {
                             sstart = listWordRange[i].startID;
                             eend = listWordRange[i].endID;
                             break;
                         }
                     }
                     if((sstart != -1)&&(eend!=-1))
                     {*/
                    sstart = mainDict.dicts.Count - 1000;
                    if (sstart < 0)
                        sstart = 0;

                    for (int i = sstart; i < mainDict.dicts.Count; i++)
                    {
                        if (mainDict.dicts[i].word == line)
                        {
                            t = i;
                            break;
                        }
                    }
                    /*  }
                      else
                      {
                          throw new Exception("wefwefwfe");
                      }*/
                }
                else
                    t = -1;

                //t = mainDict.dicts.FindIndex(x => x.word == line);

                if (t == -1)
                {
                    DictWord dictWord = new DictWord();
                    dictWord.id = localCounter;
                    dictWord.word = line;
                    dictWord.isMainWord = true;
                    dictWord.type = tokens[1].ToLower();
                    mainDict.dicts.Add(dictWord);
                    mainDict.listWordRange[mainDict.listWordRange.Count - 1].endID = mainDict.dicts.Count;
                    if (tokens.Length > 2)
                    {
                        for (int i = 2; i < tokens.Length; i++)
                        {
                            if (tokens[i].Length <= 0)
                                continue;

                            t = -1;

                            // if ((sstart != -1) && (eend != -1))
                            // {                            

                            for (int ki = sstart; ki < mainDict.dicts.Count; ki++)
                            {
                                if (mainDict.dicts[ki].word == tokens[i].ToLower())
                                {
                                    t = ki;
                                    break;
                                }
                            }
                            //}
                            /*else
                            {
                                throw new Exception("wefwefwfe");
                            }*/


                            //t = mainDict.dicts.FindIndex(x => x.word == tokens[i].ToLower());
                            if (t == -1)
                            {
                                DictWord dictWord1 = new DictWord();
                                dictWord1.id = localCounter;
                                dictWord1.isMainWord = false;
                                //localCounter++;
                                dictWord1.word = tokens[i].ToLower();
                                dictWord1.type = tokens[1].ToLower();
                                mainDict.dicts.Add(dictWord1);
                                mainDict.listWordRange[mainDict.listWordRange.Count - 1].endID = mainDict.dicts.Count;
                            }
                        }
                    }
                    localCounter++;
                }

            }

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
  (ThreadStart)delegate ()
  {
      myDataContext.strings.Add("end!");
  }
  );

            // BinaryFormatter сохраняет данные в двоичном формате. Чтобы получить доступ к BinaryFormatter, понадобится
            // импортировать System.Runtime.Serialization.Formatters.Binary
            BinaryFormatter binFormat = new BinaryFormatter();
            // Сохранить объект в локальном файле.
            using (Stream fStream = new FileStream("maindict.dat", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                binFormat.Serialize(fStream, mainDict);
            }
        }

        private void loadDict_Click(object sender, RoutedEventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = path;
            if (openFileDialog.ShowDialog() == true)
            {
                globalFile = openFileDialog.FileName;
                Thread t = new Thread(new ThreadStart(GetMainDict));
                t.Start();
                //MessageBox.Show("ВСЕ!");
            }
        }

        public void loadUserDict()
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                    {
                        myDataContext.strings.Add("start loading dict!");
                    }
                );

            BinaryFormatter binFormat = new BinaryFormatter();

            using (Stream fStream = File.OpenRead("maindict.dat"))
            {
                loadedMyDict = (MainDict)binFormat.Deserialize(fStream);
            }
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    myDataContext.strings.Add("end loading dict!");
                }
                );
        }

        private void loadDictXML_Click(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(new ThreadStart(loadUserDict));
            t.Start();            
        }

        private void testNet_Click(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(new ThreadStart(TestForMNIST));

            t.Start();
        }


        public void TestForMNIST()
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    myDataContext.strings.Add("Start!");
                }
                );

            NeuralNet myNNet;

            try
            {
                myNNet = NeuralNet.Load("weights");
                if (myNNet == null)
                {
                    myNNet = new NeuralNet(784, 200, 10, 7, 0.1f);
                    trainSet = TrainSet.Load("MNIST");
                    if (trainSet == null)
                    {
                        trainSet = new TrainSet(10, 784);
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                             (ThreadStart)delegate ()
                                {
                                    myDataContext.strings.Add("Start to train dataset!");
                                }
                             );

                        if (File.Exists("mnist_train.csv"))
                        {
                            foreach (string line in File.ReadLines("mnist_train.csv", Encoding.GetEncoding(1251)))
                            {
                                string[] tokens;
                                Matrix mTrain = new Matrix(1, trainSet.dimTrainSet);
                                Matrix mInput = new Matrix(1, trainSet.dimInputSet);
                                tokens = line.Split(',');
                                int lenSents = tokens.Length;
                                int control_digit;
                                bool res = Int32.TryParse(tokens[0], out control_digit);

                                // trainSet.controlDigit.Add(control_digit);

                                for (int i = 0; i < trainSet.dimTrainSet; i++)
                                {
                                    if (i == control_digit)
                                        mTrain.elements[0, i] = 0.9999f;
                                    else
                                        mTrain.elements[0, i] = 0.000f;
                                }

                                trainSet.mTrainSet.Add(mTrain);

                                for (int i = 1; i < lenSents; i++)
                                {
                                    int t = Convert.ToInt32(tokens[i]);
                                    if (t == 0)
                                        mInput.elements[0, i - 1] = 0.01f;
                                    else
                                    {
                                        if (t == 255)
                                            mInput.elements[0, i - 1] = 0.9999f;
                                        else
                                            mInput.elements[0, i - 1] = (t / 256.0f * 0.99f) + 0.01f;
                                    }
                                }

                                trainSet.mInputSet.Add(mInput);

                            }
                            trainSet.Save("MNIST");
                        }
                        else
                        {
                            MessageBox.Show("File not found for train dataset!");
                            return;
                        }
                    }
                    else
                    {

                    }
                    for (int epo = 0; epo < myNNet.numEpoch; epo++)
                    {
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                            (ThreadStart)delegate ()
                            {
                                myDataContext.strings.Add(" Start epoch: " + epo.ToString() + " Time: " + DateTime.Now.ToString("mm:ss:ffff"));
                            }
                            );
                        //int tempNumStr = 0;
                        for (int index = 0; index < trainSet.mInputSet.Count; index++)
                        {
                            //tempNumStr++;

                            myNNet.TrainNet(trainSet.mInputSet[index], trainSet.mTrainSet[index]);

                        }
                    }

                    myNNet.Save("weights");
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        (ThreadStart)delegate ()
                        {
                            myDataContext.strings.Add("Finish train dataset! Time: " + DateTime.Now.ToString("mm:ss:ffff"));
                            myDataContext.strings.Add("Start to test dataset!");
                        }
                        );

                }
                else
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        (ThreadStart)delegate ()
                        {
                            myDataContext.strings.Add("Weights loaded! Time: " + DateTime.Now.ToString("mm:ss:ffff"));
                            myDataContext.strings.Add("Start to test dataset!");
                        }
                        );

                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            try
            {
                if (File.Exists("mnist_test.csv"))
                {
                    string[] tokens1;
                    string[] allText1;

                    int countAll = 0;
                    int countRight = 0;

                    allText1 = File.ReadAllLines("mnist_test.csv", Encoding.GetEncoding(1251));
                    foreach (string line in allText1)
                    {
                        countAll++;
                        Matrix input = new Matrix(1, 784);

                        tokens1 = line.Split(',');
                        int lenSents = tokens1.Length;
                        int control_digit;
                        bool res = Int32.TryParse(tokens1[0], out control_digit);


                        for (int i = 1; i < lenSents; i++)
                        {
                            int t = Convert.ToInt32(tokens1[i]);
                            if (t == 0)
                                input.elements[0, i - 1] = 0.01f;
                            else
                            {
                                if (t == 255)
                                    input.elements[0, i - 1] = 0.9999f;
                                else
                                    input.elements[0, i - 1] = (t / 256.0f * 0.99f) + 0.01f;
                            }
                        }

                        Matrix testt = myNNet.CalcNet(input);

                        float value = -1.0f;
                        int calcDigit = -1;

                        for (int i = 1; i < testt.Cols; i++)
                        {
                            if (value < testt.elements[0, i])
                            {
                                calcDigit = i;
                                value = testt.elements[0, i];
                            }
                        }

                        if (calcDigit == control_digit)
                            countRight++;

                        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                            (ThreadStart)delegate ()
                            {
                                myDataContext.strings.Add("Control digit: " + control_digit.ToString() + " NN digit: " + calcDigit.ToString());
                            }
                            );

                    }
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        (ThreadStart)delegate ()
                        {
                            myDataContext.strings.Add("All count: " + countAll.ToString() + " right count: " + countRight.ToString());
                            myDataContext.strings.Add("That's all!");
                        }
                        );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }



        }
    }
}
