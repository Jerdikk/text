
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
using System.Xml.Serialization;

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
        public List<DictWord> mainWords;
        public MainDict()
        {
            listWordRange = new List<WordRange>();
            mainWords = new List<DictWord>();
        }

    }


    [Serializable]
    public class AllDict
    {
        public List<WordRange> listWordRange;
        public List<DictWord> dictWords;
        public AllDict()
        {
            listWordRange = null;
            dictWords = new List<DictWord>();
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
        public AllDict allWordDict;
        public MainDict mainDictionary;
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
            List<string> sentTokenList = new List<string>();
            List<string> sentTokenListFirFile = new List<string>();
            List<int> countToken = new List<int>();
            List<int> countSentToken = new List<int>();
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
                    sentTokenList = new List<string>();
                    countSentToken = new List<int>();
                    tokens = sentense.Split(' ');
                    foreach (string token in tokens)
                    {
                        string tempTok = token.Trim();
                        if (tempTok.Length > 0)
                        {

                            tokenList.Add(tempTok);
                            sentTokenList.Add(tempTok);

                            bool found1 = false;
                            foreach (WordRange wordRange in allWordDict.listWordRange)
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
                                                        if (allWordDict.dictWords[i].word == tempTok)
                                                        {
                                                            countToken.Add(i);
                                                            countSentToken.Add(i);
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
                                            if (allWordDict.dictWords[i].word == tempTok)
                                            {
                                                countToken.Add(i);
                                                countSentToken.Add(i);
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
                            {
                                countToken.Add(-1);
                                countSentToken.Add(-1);
                            }
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
                    string hh = "";
                    for (int j = 0; j < countSentToken.Count; j++)
                    {
                        sentTokenList[j] += ";";
                        sentTokenList[j] += countSentToken[j].ToString();
                        hh += sentTokenList[j];
                    }
                    sentTokenListFirFile.Add(hh);

                }
                for (int j = 0; j < countToken.Count; j++)
                {
                    tokenList[j] += ";";
                    tokenList[j] += countToken[j].ToString();
                }
                File.WriteAllLines("2.txt", sentList);
                File.WriteAllLines("4.txt", sentTokenListFirFile);
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

            allWordDict = new AllDict();
            mainDictionary = new MainDict();

            // AllDict mainDict = new AllDict();

            ////                             t = myDict.tree.FindIndex(x => x.Letter == line.Substring(0, 1));
            //   int cc = 0;
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
            // int currentListRangeIndex = 0;
            // int currentLevel2ListRangeIndex = 0;

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
                #region MyRegion
                /*
        if (allWordDict.listWordRange == null)
        {
            currentListRangeIndex = 0;
            allWordDict.listWordRange = new List<WordRange>();
            WordRange wordRange = new WordRange();
            wordRange.letter = line.Substring(0, 1);
            if (lenn > 1)
            {
                if (wordRange.wordRanges == null)
                {
                    wordRange.wordRanges = new List<WordRange>();
                    WordRange wordRange1 = new WordRange();
                    wordRange1.letter = line.Substring(1, 1);
                    wordRange1.startID = allWordDict.dictWords.Count;
                    wordRange1.endID = allWordDict.dictWords.Count;
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
            wordRange.startID = allWordDict.dictWords.Count;
            wordRange.endID = allWordDict.dictWords.Count;
            allWordDict.listWordRange.Add(wordRange);
        }
        else
        {
            if (allWordDict.listWordRange[currentListRangeIndex].letter != line.Substring(0, 1))
            {
                allWordDict.listWordRange[currentListRangeIndex].endID = allWordDict.dictWords.Count;
                if (allWordDict.listWordRange.Count > 1)
                {
                    bool foundIndex = false;
                    for (int h = 0; h < allWordDict.listWordRange.Count; h++)
                    {
                        if (allWordDict.listWordRange[h].letter == line.Substring(0, 1))
                        {
                            currentListRangeIndex = h;
                            foundIndex = true;
                            break;
                        }
                    }

                    if (foundIndex)
                    {
                        WordRange wordRange = allWordDict.listWordRange[currentListRangeIndex];
                        // wordRange.letter = line.Substring(0, 1);
                        //wordRange.startID = allWordDict.dicts.Count;
                        wordRange.endID = allWordDict.dictWords.Count;
                        //allWordDict.listWordRange.Add(wordRange);
                        if (lenn > 1)
                        {
                            if (wordRange.wordRanges == null)
                            {
                                wordRange.wordRanges = new List<WordRange>();
                                WordRange wordRange1 = new WordRange();
                                wordRange1.letter = line.Substring(1, 1);
                                wordRange1.startID = allWordDict.dictWords.Count;
                                wordRange1.endID = allWordDict.dictWords.Count;
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
                                    wordRange.wordRanges[indexx].endID = allWordDict.dictWords.Count;
                                    currentLevel2ListRangeIndex = indexx;
                                }
                                else
                                {
                                    WordRange wordRange1 = new WordRange();
                                    wordRange1.letter = line.Substring(1, 1);
                                    wordRange1.startID = allWordDict.dictWords.Count;
                                    wordRange1.endID = allWordDict.dictWords.Count;
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
                        wordRange.startID = allWordDict.dictWords.Count;
                        wordRange.endID = allWordDict.dictWords.Count;
                        allWordDict.listWordRange.Add(wordRange);
                        currentListRangeIndex = allWordDict.listWordRange.Count - 1;
                        if (lenn > 1)
                        {
                            if (wordRange.wordRanges == null)
                            {
                                wordRange.wordRanges = new List<WordRange>();
                                WordRange wordRange1 = new WordRange();
                                wordRange1.letter = line.Substring(1, 1);
                                wordRange1.startID = allWordDict.dictWords.Count;
                                wordRange1.endID = allWordDict.dictWords.Count;
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
                    wordRange.startID = allWordDict.dictWords.Count;
                    wordRange.endID = allWordDict.dictWords.Count;
                    allWordDict.listWordRange.Add(wordRange);
                    currentListRangeIndex = allWordDict.listWordRange.Count - 1;
                    if (lenn > 1)
                    {
                        if (wordRange.wordRanges == null)
                        {
                            wordRange.wordRanges = new List<WordRange>();
                            WordRange wordRange1 = new WordRange();
                            wordRange1.letter = line.Substring(1, 1);
                            wordRange1.startID = allWordDict.dictWords.Count;
                            wordRange1.endID = allWordDict.dictWords.Count;
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
                    if (allWordDict.listWordRange[currentListRangeIndex].wordRanges != null)
                    {
                        if (allWordDict.listWordRange[currentListRangeIndex].wordRanges[currentLevel2ListRangeIndex].letter != line.Substring(1, 1))
                        {
                            WordRange wordRange = allWordDict.listWordRange[currentListRangeIndex];

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
                                wordRange.wordRanges[indexx].endID = allWordDict.dictWords.Count;
                                currentLevel2ListRangeIndex = indexx;
                            }
                            else
                            {
                                WordRange wordRange1 = new WordRange();
                                wordRange1.letter = line.Substring(1, 1);
                                wordRange1.startID = allWordDict.dictWords.Count;
                                wordRange1.endID = allWordDict.dictWords.Count;
                                wordRange.wordRanges.Add(wordRange1);
                                currentLevel2ListRangeIndex = wordRange.wordRanges.Count - 1;
                            }                                                                      
                        }
                        else
                        {
                            allWordDict.listWordRange[currentListRangeIndex].wordRanges[currentLevel2ListRangeIndex].endID = allWordDict.dictWords.Count;
                        }
                    }
                    else
                    {
                        WordRange wordRange = allWordDict.listWordRange[currentListRangeIndex];
                        if (wordRange.wordRanges == null)
                        {
                            wordRange.wordRanges = new List<WordRange>();
                            WordRange wordRange1 = new WordRange();
                            wordRange1.letter = line.Substring(1, 1);
                            wordRange1.startID = allWordDict.dictWords.Count;
                            wordRange1.endID = allWordDict.dictWords.Count;
                            wordRange.wordRanges.Add(wordRange1);
                            currentLevel2ListRangeIndex = 0;
                        }
                        else
                        {
                            wordRange.wordRanges[currentLevel2ListRangeIndex].endID = allWordDict.dictWords.Count;
                        }

                    }
                }
                allWordDict.listWordRange[currentListRangeIndex].endID = allWordDict.dictWords.Count;
            }

        }*/
                #endregion


                t = -1;
                if (allWordDict.dictWords.Count >= 1)
                {

                    sstart = allWordDict.dictWords.Count - 1000;
                    if (sstart < 0)
                        sstart = 0;

                    for (int i = sstart; i < allWordDict.dictWords.Count; i++)
                    {
                        if (allWordDict.dictWords[i].word == line)
                        {
                            t = i;
                            break;
                        }
                    }
                }
                else
                    t = -1;


                if (t == -1)
                {
                    DictWord dictWord = new DictWord();
                    dictWord.id = localCounter;
                    dictWord.word = line;
                    dictWord.isMainWord = true;
                    dictWord.type = tokens[1].ToLower();
                    allWordDict.dictWords.Add(dictWord);
                    mainDictionary.mainWords.Add(dictWord);

                    if (tokens.Length > 2)
                    {
                        for (int i = 2; i < tokens.Length; i++)
                        {
                            if (tokens[i].Length <= 0)
                                continue;

                            t = -1;
                            for (int ki = sstart; ki < allWordDict.dictWords.Count; ki++)
                            {
                                if (allWordDict.dictWords[ki].word == tokens[i].ToLower())
                                {
                                    t = ki;
                                    break;
                                }
                            }
                            if (t == -1)
                            {
                                DictWord dictWord1 = new DictWord();
                                dictWord1.id = localCounter;
                                dictWord1.isMainWord = false;
                                dictWord1.word = tokens[i].ToLower();
                                dictWord1.type = tokens[1].ToLower();
                                allWordDict.dictWords.Add(dictWord1);
                            }
                        }
                    }
                    localCounter++;
                }
                else
                {
                    DictWord dictWord = allWordDict.dictWords[t];
                    if (tokens.Length > 2)
                    {
                        for (int i = 2; i < tokens.Length; i++)
                        {
                            if (tokens[i].Length <= 0)
                                continue;

                            t = -1;
                            for (int ki = sstart; ki < allWordDict.dictWords.Count; ki++)
                            {
                                if (allWordDict.dictWords[ki].word == tokens[i].ToLower())
                                {
                                    t = ki;
                                    break;
                                }
                            }
                            if (t == -1)
                            {
                                DictWord dictWord1 = new DictWord();
                                dictWord1.id = dictWord.id;
                                dictWord1.isMainWord = false;
                                dictWord1.word = tokens[i].ToLower();
                                dictWord1.type = tokens[1].ToLower();
                                allWordDict.dictWords.Add(dictWord1);
                            }
                        }
                    }

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
           /* BinaryFormatter binFormat = new BinaryFormatter();
            // Сохранить объект в локальном файле.
            using (Stream fStream = new FileStream("allworddict.dat", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                binFormat.Serialize(fStream, allWordDict);
            }*/

//            XmlSerializer xmlSerializer = new XmlSerializer(typeof(AllDict));

            try
            {
                //Open the File
                StreamWriter sw = new StreamWriter("allword.txt", true, Encoding.UTF8);

                foreach (DictWord dictWord in allWordDict.dictWords)
                {
                    string temp = dictWord.word + ";" + dictWord.id + ";" + (dictWord.isMainWord ? "1" : "0") + ";" + dictWord.type;
                    sw.WriteLine(temp);
                }

                //close the file
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }


            try
            {
                //Open the File
                StreamWriter sw = new StreamWriter("mainword.txt", true, Encoding.UTF8);

                foreach (DictWord dictWord in mainDictionary.mainWords)
                {
                    string temp = dictWord.word + ";" + dictWord.id + ";" + (dictWord.isMainWord ? "1" : "0") + ";" + dictWord.type;
                    sw.WriteLine(temp);
                }

                //close the file
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }


            // получаем поток, куда будем записывать сериализованный объект
            /*using (FileStream fs = new FileStream("allworddict.xml", FileMode.OpenOrCreate))
            {
                xmlSerializer.Serialize(fs, allWordDict);

                
            }*/

            /* XmlSerializer xmlSerializer1 = new XmlSerializer(typeof(MainDict));

             using (FileStream fs1 = new FileStream("mainworddict.xml", FileMode.OpenOrCreate))
             {
                 xmlSerializer1.Serialize(fs1, mainDictionary);

             }*/


            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    myDataContext.strings.Add("Object has been serialized");
                }
                );

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

            using (Stream fStream = File.OpenRead("allworddict.dat"))
            {
                allWordDict = (AllDict)binFormat.Deserialize(fStream);
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

        private void loadDictText_Click(object sender, RoutedEventArgs e)
        {
            string[] allText;
            string[] tokens;
            char[] chars = new char[3];
            chars[0] = ';';

            //chars[1] = '!';
            //chars[2] = '?';

            string tempStr;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                if (allWordDict == null)
                    allWordDict = new AllDict();
                if (allWordDict.dictWords == null)
                    allWordDict.dictWords = new List<DictWord>();

                int cc = 0;
                //allText = File.ReadAllLines(openFileDialog.FileName, Encoding.GetEncoding(1251));
                allText = File.ReadAllLines(openFileDialog.FileName, Encoding.GetEncoding("utf-8"));
                foreach (string line in allText)
                {
                    tempStr = "";
                    
                    tokens = line.Split(chars);
                    int lenSents = tokens.Length;
                    if (lenSents < 4)
                        continue;
                    DictWord dictWord = new DictWord();
                    dictWord.word = tokens[0].Trim(); 
                    dictWord.id = int.Parse(tokens[1].Trim());
                    dictWord.isMainWord = tokens[2] == "1";
                    dictWord.type = tokens[3].Trim();
                    allWordDict.dictWords.Add(dictWord);    
                }
            }
            int yyy = 1;
            openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                if (mainDictionary == null)
                    mainDictionary = new MainDict();
                if (mainDictionary.mainWords == null)
                    mainDictionary.mainWords = new List<DictWord>();

                int cc = 0;
                //allText = File.ReadAllLines(openFileDialog.FileName, Encoding.GetEncoding(1251));
                allText = File.ReadAllLines(openFileDialog.FileName, Encoding.GetEncoding("utf-8"));
                foreach (string line in allText)
                {
                    tempStr = "";

                    tokens = line.Split(chars);
                    int lenSents = tokens.Length;
                    if (lenSents < 4)
                        continue;
                    DictWord dictWord = new DictWord();
                    dictWord.word = tokens[0].Trim();
                    dictWord.id = int.Parse(tokens[1].Trim());
                    dictWord.isMainWord = tokens[2] == "1";
                    dictWord.type = tokens[3].Trim();
                    mainDictionary.mainWords.Add(dictWord);
                }
            }
            yyy = 2;
        }

        private void sortDict_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
