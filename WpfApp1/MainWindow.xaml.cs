
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

    public class SentenceIDs
    {
       public List<int> ids = new List<int>();
    }

    public class TextWithSents
    {
        public List<SentenceIDs> sentenceIDs = new List<SentenceIDs>();
    }

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

    public class ListDictWords
    {
        public List<DictWord> dictWords;

        public List<ListDictWords> listWords;

        public ListDictWords()
        {
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
        private DispatcherTimer dispatcherTimer;
        private double index001;
        private double counter001;

        public MainWindow()
        {

            InitializeComponent();

            Binding binding = new Binding();
            binding.Source = myDataContext;
            binding.Path = new PropertyPath("strings");
            binding.Mode = BindingMode.TwoWay;

            lbTest.SetBinding(ListBox.ItemsSourceProperty, binding);

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer(DispatcherPriority.Render);
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(1000);
            dispatcherTimer.Start();
            // myDict = new MyDict();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            progr1.Value = GlobalData.Instance.progress01;
            progr2.Value = GlobalData.Instance.progress02;
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

        public void loadText()
        {
            string[] allText;
            string[] tokens;

            string tempLine = "";
            string tempStr = "";

            char[] chars = new char[5];
            chars[0] = '.';

            chars[1] = '!';
            chars[2] = '?';
            chars[3] = '(';
            chars[4] = ')';


            List<string> sentList = new List<string>();
            List<string> tokenList = new List<string>();
            List<string> sentTokenList = new List<string>();
            List<string> sentTokenListFirFile = new List<string>();
            List<int> countToken = new List<int>();
            List<int> listIDToken = new List<int>();
            List<int> countSentToken = new List<int>();

            TextWithSents textWithSents = new TextWithSents();

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
          (ThreadStart)delegate ()
          {
              myDataContext.strings.Add("start loading text!" + DateTime.Now.ToString());
          }
          );

            int cc = 0;
            //allText = File.ReadAllLines(openFileDialog.FileName, Encoding.GetEncoding(1251));
            allText = File.ReadAllLines(globalFile, Encoding.GetEncoding("utf-8"));
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
          (ThreadStart)delegate ()
          {
              myDataContext.strings.Add("start loading sentlist!" + DateTime.Now.ToString());
          }
          );
            index001 = 0;
            counter001 = allText.Length;
            foreach (string line in allText)
            {
                GlobalData.Instance.progress02 = (double)index001 / (double)counter001 * 100.0;
                index001++;
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
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
          (ThreadStart)delegate ()
          {
              myDataContext.strings.Add("start parsing sents!" + DateTime.Now.ToString());
          }
          );
            index001 = 0;
            counter001 = sentList.Count;
            foreach (string sentense in sentList)
            {
                GlobalData.Instance.progress02 = (double)index001 / (double)counter001 * 100.0;
                index001++;

                SentenceIDs sentenceIDs = new SentenceIDs();
                sentenceIDs.ids = new List<int>();
                
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
                                                        listIDToken.Add(allWordDict.dictWords[i].id);
                                                        sentenceIDs.ids.Add(allWordDict.dictWords[i].id);
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
                                            listIDToken.Add(allWordDict.dictWords[i].id);
                                            sentenceIDs.ids.Add(allWordDict.dictWords[i].id);
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
                            listIDToken.Add(-1);
                            countSentToken.Add(-1);
                        }                       
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
                if (sentenceIDs.ids.Count>0)
                    textWithSents.sentenceIDs.Add(sentenceIDs);
            }
            List<string> strings = new List<string>();
            List<int> indexes = new List<int>();

            bool find;

            for (int j = 0; j < listIDToken.Count; j++)
            {
                if (listIDToken[j] >= 0)
                {
                    find = false;
                    for (int k = 0; k < indexes.Count; k++)
                    {
                        if (indexes[k] == listIDToken[j])
                        {
                            find = true;
                            break;
                        }
                    }
                    if (!find)
                        indexes.Add(listIDToken[j]);
                }
            }

            for (int j = 0; j < countToken.Count; j++)
            {
                if (countToken[j] < 0)
                {
                    string tttt = tokenList[j];
                    tttt += ";";
                    tttt += countToken[j].ToString();
                    strings.Add(tttt);
                }
            }
            if (File.Exists("2.txt"))
                File.Delete("2.txt");
            if (File.Exists("4.txt"))
                File.Delete("4.txt");
            if (File.Exists("3.csv"))
                File.Delete("3.csv");

            File.WriteAllLines("2.txt", sentList);
            File.WriteAllLines("4.txt", sentTokenListFirFile);
            File.WriteAllLines("3.csv", strings);
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
          (ThreadStart)delegate ()
          {
              myDataContext.strings.Add("end loading text!" + DateTime.Now.ToString());
          }
          );
        }

        private void load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                globalFile = openFileDialog.FileName;
                Thread t = new Thread(new ThreadStart(loadText));
                t.Start();
                //MessageBox.Show("ВСЕ!");
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
                  myDataContext.strings.Add("Start!" + DateTime.Now.ToString());
              }
              );

            allText = File.ReadAllLines(globalFile, Encoding.GetEncoding(1251));

            int sstart = 0;
            // int currentListRangeIndex = 0;
            // int currentLevel2ListRangeIndex = 0;

            index001 = 0;
            counter001 = allText.Length;

            foreach (string line1 in allText)
            {
                GlobalData.Instance.progress02 = (double)index001 / (double)counter001 * 100.0;
                index001++;
                /*if (localCounter % 10000 == 0)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                      (ThreadStart)delegate ()
                      {
                          myDataContext.strings.Add(localCounter.ToString() + " " + DateTime.Now.ToString());
                      }
                      );

                }*/

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
                    myDataContext.strings.Add("end!" + DateTime.Now.ToString());
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
            if (File.Exists("allword.txt"))
                File.Delete("allword.txt");

            try
            {
                //Open the File
                StreamWriter sw = new StreamWriter("allword.txt", true, Encoding.UTF8);
                double index002 = 0;
                double counter002 = allWordDict.dictWords.Count;

                foreach (DictWord dictWord in allWordDict.dictWords)
                {                    
                    GlobalData.Instance.progress01 = (double)index002 / (double)counter002 * 100.0;
                    index002++;
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

            if (File.Exists("mainword.txt"))
                File.Delete("mainword.txt");

            try
            {
                //Open the File
                StreamWriter sw = new StreamWriter("mainword.txt", true, Encoding.UTF8);
                double index002 = 0;
                double counter002 = mainDictionary.mainWords.Count;

                foreach (DictWord dictWord in mainDictionary.mainWords)
                {
                    GlobalData.Instance.progress01 = (double)index002 / (double)counter002 * 100.0;
                    index002++;

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
                        myDataContext.strings.Add("start loading dict!" + DateTime.Now.ToString());
                    }
                );

            /*  BinaryFormatter binFormat = new BinaryFormatter();

              using (Stream fStream = File.OpenRead("allworddict.dat"))
              {
                  allWordDict = (AllDict)binFormat.Deserialize(fStream);
              }*/
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    myDataContext.strings.Add("end loading dict!" + DateTime.Now.ToString());
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
                    myDataContext.strings.Add("Start!" + DateTime.Now.ToString());
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
                                    myDataContext.strings.Add("Start to train dataset!" + DateTime.Now.ToString());
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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                globalFile = openFileDialog.FileName;
                Thread t = new Thread(new ThreadStart(LoadAndSortAllWordDict));
                t.Start();

            }
            int yyy = 1;
            /*openFileDialog = new OpenFileDialog();
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
            }*/
            yyy = 2;



        }

        public void LoadAndSortAllWordDict()
        {
            string[] allText;
            string[] tokens;
            char[] chars = new char[3];
            chars[0] = ';';

            //chars[1] = '!';
            //chars[2] = '?';

            string tempStr;



            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    myDataContext.strings.Add("Start loading dict " + DateTime.Now.ToString());
                }
                );


            //SortedDictionary<string, DictWord> tempToSort =
            // new SortedDictionary<string, DictWord>();
            List<DictWord> dictWordsList = new List<DictWord>();

            int cc = 0;
            //allText = File.ReadAllLines(openFileDialog.FileName, Encoding.GetEncoding(1251));
            allText = File.ReadAllLines(globalFile, Encoding.GetEncoding("utf-8"));
            int excepCount = 0;
            double index002 = 0;
            double counter002 = allText.Length;

            foreach (string line in allText)
            {
                GlobalData.Instance.progress01 = (double)index002 / (double)counter002 * 100.0;
                index002++;

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
                try
                {
                    dictWordsList.Add(dictWord);
                }
                catch (Exception ex)
                {
                    excepCount++;
                }
                //   allWordDict.dictWords.Add(dictWord);    
            }
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    myDataContext.strings.Add("Start sorting dict " + DateTime.Now.ToString());
                }
                );
            allText = null;

            List<ListDictWords> listDictWords = new List<ListDictWords>();
            for (int u = 0; u < 33; u++)
            {
                listDictWords.Add(new ListDictWords());
            }
            foreach (ListDictWords listDictWords2 in listDictWords)
            {
                listDictWords2.listWords = new List<ListDictWords>();
                for (int u = 0; u < 33; u++)
                {
                    listDictWords2.listWords.Add(new ListDictWords());
                }

            }
            index001 = 0;
            counter001 = dictWordsList.Count;
            foreach (DictWord dictWord1 in dictWordsList)
            {
                GlobalData.Instance.progress02 = (double)index001 / (double)counter001 * 100.0;
                index001++;

                string temp = dictWord1.word.Substring(0, 1);
                char[] tt = temp.ToCharArray();
                int b = (int)tt[0];
                b -= 1072;
                if ((b < 0) || (b > 32))
                    continue;//throw new Exception();

                int len = dictWord1.word.Length;
                if (len > 0)
                {
                    if (len == 1)
                        listDictWords[b].dictWords.Add(dictWord1);
                    else
                    {
                        temp = dictWord1.word.Substring(1, 1);
                        tt = temp.ToCharArray();
                        int c = (int)tt[0];
                        c -= 1072;
                        if ((c < 0) || (c > 32))
                            continue;//throw new Exception();
                        listDictWords[b].listWords[c].dictWords.Add(dictWord1);
                    }
                }
            }

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    myDataContext.strings.Add("end sorting dict " + DateTime.Now.ToString());
                }
                );

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    myDataContext.strings.Add("start writing dict " + DateTime.Now.ToString());
                }
                );
            if (File.Exists(globalFile))
                File.Delete(globalFile);
            try
            {
                //Open the File
                StreamWriter sw = new StreamWriter(globalFile, true, Encoding.UTF8);

                foreach (ListDictWords listDictWords1 in listDictWords)
                {
                    foreach (DictWord dictWord in listDictWords1.dictWords)
                    {
                        string temp = dictWord.word + ";" + dictWord.id + ";" + (dictWord.isMainWord ? "1" : "0") + ";" + dictWord.type;
                        sw.WriteLine(temp);
                    }
                    foreach (ListDictWords listDictWords22 in listDictWords1.listWords)
                    {
                        foreach (DictWord dictWord in listDictWords22.dictWords)
                        {
                            string temp = dictWord.word + ";" + dictWord.id + ";" + (dictWord.isMainWord ? "1" : "0") + ";" + dictWord.type;
                            sw.WriteLine(temp);
                        }
                    }
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
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate () { myDataContext.strings.Add("end writing dict " + DateTime.Now.ToString()); });

        }

        private void indexDict_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                globalFile = openFileDialog.FileName;
                Thread t = new Thread(new ThreadStart(IndexDict));
                t.Start();

            }
            int yyy = 1;
        }

        public void IndexDict()
        {
            if (allWordDict == null)
            {
                allWordDict = new AllDict();
            }
            else
            {

            }
            if (allWordDict.dictWords == null)
                allWordDict.dictWords = new List<DictWord>();
            else
                allWordDict.dictWords.Clear();

            if (allWordDict.listWordRange == null)
                allWordDict.listWordRange = new List<WordRange>();
            else
                allWordDict.listWordRange.Clear();

            string[] allText;
            string[] tokens;
            char[] chars = new char[3];
            chars[0] = ';';

            string tempStr;
            string currentStr;
            string currentStr2;


            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    myDataContext.strings.Add("Start loading dict " + DateTime.Now.ToString());
                }
                );

            // List<DictWord> dictWordsList = new List<DictWord>();

            int cc = 0;
            allText = File.ReadAllLines(globalFile, Encoding.GetEncoding("utf-8"));
            int excepCount = 0;
            index001 = 0;
            counter001 = allText.Length;
            foreach (string line in allText)
            {
                GlobalData.Instance.progress02 = (double)index001 / (double)counter001 * 100.0;
                index001++;

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
                try
                {
                    allWordDict.dictWords.Add(dictWord);
                }
                catch (Exception ex)
                {
                    excepCount++;
                }
            }

            allText = null;


            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
    (ThreadStart)delegate ()
    {
        myDataContext.strings.Add("Start indexing dict " + DateTime.Now.ToString());
    }
    );

            if (allWordDict.listWordRange == null)
                allWordDict.listWordRange = new List<WordRange>();
            else
                allWordDict.listWordRange.Clear();

            currentStr = "";
            currentStr2 = "";
            int counter = 0;
            int len = 0;
            WordRange wordRange = null;
            WordRange wordRange1 = null;
            index001 = 0;
            counter001 = allWordDict.dictWords.Count;
            foreach (DictWord dictWord in allWordDict.dictWords)
            {
                GlobalData.Instance.progress02 = (double)index001 / (double)counter001 * 100.0;
                index001++;


                tempStr = dictWord.word.Substring(0, 1);
                len = dictWord.word.Length;
                if (tempStr != currentStr)
                {
                    if (allWordDict.listWordRange != null)
                    {
                        bool found = false;
                        foreach (WordRange wordRange2 in allWordDict.listWordRange)
                        {
                            if (wordRange2.letter == tempStr)
                            {
                                found = true;
                                wordRange2.endID = counter;
                                wordRange = wordRange2;
                                currentStr = tempStr;
                                if (len > 1)
                                {
                                    tempStr = dictWord.word.Substring(1, 1);
                                    bool found1 = false;
                                    if (wordRange.wordRanges != null)
                                        foreach (WordRange wordRange3 in wordRange.wordRanges)
                                        {
                                            if (wordRange3.letter == tempStr)
                                            {
                                                currentStr2 = tempStr;
                                                found1 = true;
                                                wordRange3.endID = counter;
                                                wordRange1 = wordRange3;
                                                break;
                                            }
                                        }
                                    if (!found1)
                                    {
                                        currentStr2 = tempStr;
                                        wordRange1 = new WordRange();
                                        wordRange1.letter = tempStr;
                                        wordRange1.startID = counter;
                                        wordRange1.endID = counter;
                                        if (wordRange.wordRanges == null)
                                            wordRange.wordRanges = new List<WordRange>();
                                        wordRange.wordRanges.Add(wordRange1);
                                    }
                                    //}

                                }
                                break;
                            }
                        }
                        if (!found)
                        {
                            currentStr = tempStr;
                            wordRange = new WordRange();
                            wordRange.letter = tempStr;
                            wordRange.startID = counter;
                            wordRange.endID = counter;
                            if (len > 1)
                            {
                                tempStr = dictWord.word.Substring(1, 1);
                                currentStr2 = tempStr;
                                wordRange1 = new WordRange();
                                wordRange1.letter = tempStr;
                                wordRange1.startID = counter;
                                wordRange1.endID = counter;
                                wordRange.wordRanges = new List<WordRange>();
                                wordRange.wordRanges.Add(wordRange1);
                            }

                            allWordDict.listWordRange.Add(wordRange);

                        }
                    }
                    else
                    {

                    }
                }
                else //if (tempStr!=currentStr)
                {
                    wordRange.endID = counter;
                    if (len > 1)
                    {
                        tempStr = dictWord.word.Substring(1, 1);
                        if (tempStr == currentStr2)
                        {
                            wordRange1.endID = counter;
                        }
                        else
                        {
                            currentStr2 = tempStr;
                            bool found = false;
                            if (wordRange.wordRanges != null)
                                foreach (WordRange wordRange2 in wordRange.wordRanges)
                                {
                                    if (wordRange2.letter == tempStr)
                                    {
                                        found = true;
                                        currentStr2 = tempStr;
                                        wordRange2.endID = counter;
                                        wordRange1 = wordRange2;
                                        break;
                                    }
                                }
                            if (!found)
                            {
                                currentStr2 = tempStr;
                                wordRange1 = new WordRange();
                                wordRange1.letter = tempStr;
                                wordRange1.startID = counter;
                                wordRange1.endID = counter;
                                if (wordRange.wordRanges == null)
                                    wordRange.wordRanges = new List<WordRange>();
                                wordRange.wordRanges.Add(wordRange1);

                            }
                        }
                    }
                    else//if (len>1)
                    {

                    }

                }
                counter++;
            }


            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    myDataContext.strings.Add("End indexing dict " + DateTime.Now.ToString());
                }
                );
            allText = null;

        }

    }
}
