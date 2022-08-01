
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
    public class DictWord
    {
        public string wholeWord;
        public string type;
        public List<string> words;
    }
    [Serializable]
    public class DictTree
    {
        public int temp1;
        public string Letter;
        //  public List<string> Variants;
        public List<DictTree> dictTrees;
        public List<DictWord> dictWords;

        public DictTree()
        {
        }

    }
    [Serializable]
    public class MyDict
    {
        public List<DictTree> tree;

        public MyDict()
        {
            tree = new List<DictTree>();
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

        public MyDict myDict;
        public MyDict loadedMyDict;
        public MyDataContext myDataContext = new MyDataContext();
        public TrainSet trainSet;

        public MainWindow()
        {

            InitializeComponent();

            Binding binding = new Binding();
            binding.Source = myDataContext;
            binding.Path = new PropertyPath("strings");
            binding.Mode = BindingMode.TwoWay;

            lbTest.SetBinding(ListBox.ItemsSourceProperty, binding);

            myDict = new MyDict();
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
                allText = File.ReadAllLines(openFileDialog.FileName, Encoding.GetEncoding(1251));
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
                        if (token.Length > 0)
                        {
                            int t = tokenList.FindIndex(x => x == token);
                            if (t == -1)
                            {
                                tokenList.Add(token.Trim());
                                countToken.Add(1);
                            }
                            else
                                countToken[t] += 1;
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

        private void loadDict_Click(object sender, RoutedEventArgs e)
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

            DictTree dt;
            DictTree dt2;
            DictTree dt3;
            DictTree dt4;


            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                int cc = 0;
                allText = File.ReadAllLines(openFileDialog.FileName, Encoding.GetEncoding(1251));
                foreach (string line1 in allText)
                {
                    tokens = line1.Split(',');

                    int lenn = tokens[0].Length;

                    string line = tokens[0].ToLower();
                    if (lenn > 1)
                    {
                        int t = -1;
                        if (myDict.tree == null)
                            myDict.tree = new List<DictTree>();
                        else
                            t = myDict.tree.FindIndex(x => x.Letter == line.Substring(0, 2));
                        if (t == -1)
                        {
                            if (lenn > 1)
                            {
                                dt = new DictTree();
                                dt.Letter = line.Substring(0, 2);
                                if (lenn > 3)
                                {
                                    dt2 = new DictTree();
                                    dt2.Letter = line.Substring(2, 2);
                                    if (lenn > 5)
                                    {
                                        dt3 = new DictTree();
                                        dt3.Letter = line.Substring(4, 2);
                                        if (lenn > 7)
                                        {
                                            dt4 = new DictTree();
                                            dt4.Letter = line.Substring(6);
                                            dt4.dictWords = new List<DictWord>();
                                            DictWord dictWord = new DictWord();
                                            dictWord.wholeWord = line;
                                            dictWord.type = tokens[1].ToLower();
                                            dictWord.words = new List<string>();
                                            for (int i = 2; i < tokens.Length; i++)
                                            {
                                                dictWord.words.Add(tokens[i]);
                                            }
                                            dt4.dictWords.Add(dictWord);

                                            dt3.dictTrees = new List<DictTree>();
                                            dt3.dictTrees.Add(dt4);
                                        }
                                        else
                                        {
                                            if (lenn != 6)
                                            {
                                                dt4 = new DictTree();
                                                dt4.Letter = line.Substring(6);
                                                dt4.dictWords = new List<DictWord>();
                                                DictWord dictWord = new DictWord();
                                                dictWord.wholeWord = line;
                                                dictWord.type = tokens[1].ToLower();
                                                dictWord.words = new List<string>();
                                                for (int i = 2; i < tokens.Length; i++)
                                                {
                                                    dictWord.words.Add(tokens[i]);
                                                }
                                                dt4.dictWords.Add(dictWord);

                                                dt3.dictTrees = new List<DictTree>();
                                                dt3.dictTrees.Add(dt4);

                                            }
                                            else
                                            {
                                                if (dt3.dictWords == null)
                                                    dt3.dictWords = new List<DictWord>();
                                                DictWord dictWord = new DictWord();
                                                dictWord.wholeWord = line;
                                                dictWord.type = tokens[1].ToLower();
                                                dictWord.words = new List<string>();
                                                for (int i = 2; i < tokens.Length; i++)
                                                {
                                                    dictWord.words.Add(tokens[i]);
                                                }
                                                dt3.dictWords.Add(dictWord);
                                            }
                                        }
                                        if (dt2.dictTrees == null)
                                            dt2.dictTrees = new List<DictTree>();
                                        dt2.dictTrees.Add(dt3);
                                    }
                                    else
                                    {
                                        if (lenn != 4)
                                        {
                                            //if (dt2.Variants == null)
                                            //    dt2.Variants = new List<string>();
                                            //dt2.Variants.Add(line.Substring(4));

                                            dt4 = new DictTree();
                                            dt4.Letter = line.Substring(4);
                                            dt4.dictWords = new List<DictWord>();
                                            DictWord dictWord = new DictWord();
                                            dictWord.wholeWord = line;
                                            dictWord.type = tokens[1].ToLower();
                                            dictWord.words = new List<string>();
                                            for (int i = 2; i < tokens.Length; i++)
                                            {
                                                dictWord.words.Add(tokens[i]);
                                            }
                                            dt4.dictWords.Add(dictWord);

                                            dt2.dictTrees = new List<DictTree>();
                                            dt2.dictTrees.Add(dt4);
                                        }
                                        else
                                        {
                                            dt2.dictWords = new List<DictWord>();
                                            DictWord dictWord = new DictWord();
                                            dictWord.wholeWord = line;
                                            dictWord.type = tokens[1].ToLower();
                                            dictWord.words = new List<string>();
                                            for (int i = 2; i < tokens.Length; i++)
                                            {
                                                dictWord.words.Add(tokens[i]);
                                            }
                                            dt2.dictWords.Add(dictWord);
                                        }
                                    }
                                    if (dt.dictTrees == null)
                                        dt.dictTrees = new List<DictTree>();
                                    dt.dictTrees.Add(dt2);
                                }
                                else
                                {
                                    if (lenn != 2)
                                    {
                                        // if (dt.Variants == null)
                                        //     dt.Variants = new List<string>();
                                        // dt.Variants.Add(line.Substring(2));

                                        dt4 = new DictTree();
                                        dt4.Letter = line.Substring(2);
                                        dt4.dictWords = new List<DictWord>();
                                        DictWord dictWord = new DictWord();
                                        dictWord.wholeWord = line;
                                        dictWord.type = tokens[1].ToLower();
                                        dictWord.words = new List<string>();
                                        for (int i = 2; i < tokens.Length; i++)
                                        {
                                            dictWord.words.Add(tokens[i]);
                                        }
                                        dt4.dictWords.Add(dictWord);

                                        dt.dictTrees = new List<DictTree>();
                                        dt.dictTrees.Add(dt4);

                                    }
                                    else
                                    {
                                        dt.dictWords = new List<DictWord>();
                                        DictWord dictWord = new DictWord();
                                        dictWord.wholeWord = line;
                                        dictWord.type = tokens[1].ToLower();
                                        dictWord.words = new List<string>();
                                        for (int i = 2; i < tokens.Length; i++)
                                        {
                                            dictWord.words.Add(tokens[i]);
                                        }
                                        dt.dictWords.Add(dictWord);

                                    }
                                }
                                if (myDict.tree == null)
                                    myDict.tree = new List<DictTree>();
                                myDict.tree.Add(dt);
                            }
                            else
                            {
                                dt = new DictTree();
                                dt.Letter = line.Substring(0);
                                if (myDict.tree == null)
                                    myDict.tree = new List<DictTree>();
                                myDict.tree.Add(dt);
                            }
                        }
                        else
                        {
                            if (lenn > 3)
                            {
                                int y = -1;
                                if (myDict.tree[t].dictTrees == null)
                                    myDict.tree[t].dictTrees = new List<DictTree>();
                                else
                                    y = myDict.tree[t].dictTrees.FindIndex(x => x.Letter == line.Substring(2, 2));
                                if (y == -1)
                                {
                                    if (lenn > 3)
                                    {
                                        dt2 = new DictTree();
                                        dt2.Letter = line.Substring(2, 2);
                                        if (lenn > 5)
                                        {
                                            dt3 = new DictTree();
                                            dt3.Letter = line.Substring(4, 2);
                                            if (lenn > 7)
                                            {
                                                dt4 = new DictTree();
                                                dt4.Letter = line.Substring(6);
                                                dt4.dictWords = new List<DictWord>();
                                                DictWord dictWord = new DictWord();
                                                dictWord.wholeWord = line;
                                                dictWord.type = tokens[1].ToLower();
                                                dictWord.words = new List<string>();
                                                for (int i = 2; i < tokens.Length; i++)
                                                {
                                                    dictWord.words.Add(tokens[i]);
                                                }
                                                dt4.dictWords.Add(dictWord);

                                                dt3.dictTrees = new List<DictTree>();
                                                dt3.dictTrees.Add(dt4);
                                            }
                                            else
                                            {
                                                if (lenn != 6)
                                                {
                                                    //    if (dt3.Variants == null)
                                                    //        dt3.Variants = new List<string>();
                                                    //    dt3.Variants.Add(line.Substring(6));

                                                    dt4 = new DictTree();
                                                    dt4.Letter = line.Substring(6);
                                                    dt4.dictWords = new List<DictWord>();
                                                    DictWord dictWord = new DictWord();
                                                    dictWord.wholeWord = line;
                                                    dictWord.type = tokens[1].ToLower();
                                                    dictWord.words = new List<string>();
                                                    for (int i = 2; i < tokens.Length; i++)
                                                    {
                                                        dictWord.words.Add(tokens[i]);
                                                    }
                                                    dt4.dictWords.Add(dictWord);

                                                    dt3.dictTrees = new List<DictTree>();
                                                    dt3.dictTrees.Add(dt4);

                                                }
                                                else
                                                {
                                                    dt3.dictWords = new List<DictWord>();
                                                    DictWord dictWord = new DictWord();
                                                    dictWord.wholeWord = line;
                                                    dictWord.type = tokens[1].ToLower();
                                                    dictWord.words = new List<string>();
                                                    for (int i = 2; i < tokens.Length; i++)
                                                    {
                                                        dictWord.words.Add(tokens[i]);
                                                    }
                                                    dt3.dictWords.Add(dictWord);
                                                }
                                            }
                                            //if (dt2.dictTrees == null)
                                            dt2.dictTrees = new List<DictTree>();
                                            dt2.dictTrees.Add(dt3);
                                        }
                                        else
                                        {
                                            if (lenn != 4)
                                            {
                                                // if (dt2.Variants == null)
                                                //    dt2.Variants = new List<string>();
                                                //dt2.Variants.Add(line.Substring(4));

                                                dt3 = new DictTree();
                                                dt3.Letter = line.Substring(4);
                                                dt3.dictWords = new List<DictWord>();
                                                DictWord dictWord = new DictWord();
                                                dictWord.wholeWord = line;
                                                dictWord.type = tokens[1].ToLower();
                                                dictWord.words = new List<string>();
                                                for (int i = 2; i < tokens.Length; i++)
                                                {
                                                    dictWord.words.Add(tokens[i]);
                                                }
                                                dt3.dictWords.Add(dictWord);

                                                dt2.dictTrees = new List<DictTree>();
                                                dt2.dictTrees.Add(dt3);


                                            }
                                            else
                                            {
                                                dt2.dictWords = new List<DictWord>();
                                                DictWord dictWord = new DictWord();
                                                dictWord.wholeWord = line;
                                                dictWord.type = tokens[1].ToLower();
                                                dictWord.words = new List<string>();
                                                for (int i = 2; i < tokens.Length; i++)
                                                {
                                                    dictWord.words.Add(tokens[i]);
                                                }
                                                dt2.dictWords.Add(dictWord);
                                            }
                                        }
                                        if (myDict.tree[t].dictTrees == null)
                                            myDict.tree[t].dictTrees = new List<DictTree>();
                                        myDict.tree[t].dictTrees.Add(dt2);
                                    }
                                    else
                                    {
                                        if (lenn != 2)
                                        {
                                            //        if (myDict.tree[t].Variants == null)
                                            //            myDict.tree[t].Variants = new List<string>();
                                            //        myDict.tree[t].Variants.Add(line.Substring(2));

                                            dt3 = new DictTree();
                                            dt3.Letter = line.Substring(2);
                                            dt3.dictWords = new List<DictWord>();
                                            DictWord dictWord = new DictWord();
                                            dictWord.wholeWord = line;
                                            dictWord.type = tokens[1].ToLower();
                                            dictWord.words = new List<string>();
                                            for (int i = 2; i < tokens.Length; i++)
                                            {
                                                dictWord.words.Add(tokens[i]);
                                            }
                                            dt3.dictWords.Add(dictWord);

                                            if (myDict.tree[t].dictTrees == null)
                                                myDict.tree[t].dictTrees = new List<DictTree>();
                                            myDict.tree[t].dictTrees.Add(dt3);

                                        }
                                        else
                                        {
                                            if (myDict.tree[t].dictWords == null)
                                                myDict.tree[t].dictWords = new List<DictWord>();
                                            DictWord dictWord = new DictWord();
                                            dictWord.wholeWord = line;
                                            dictWord.type = tokens[1].ToLower();
                                            dictWord.words = new List<string>();
                                            for (int i = 2; i < tokens.Length; i++)
                                            {
                                                dictWord.words.Add(tokens[i]);
                                            }
                                            myDict.tree[t].dictWords.Add(dictWord);
                                        }

                                    }

                                }
                                else
                                {
                                    if (lenn > 5)
                                    {
                                        int z = -1;
                                        if (myDict.tree[t].dictTrees[y].dictTrees == null)
                                            myDict.tree[t].dictTrees[y].dictTrees = new List<DictTree>();
                                        else
                                            z = myDict.tree[t].dictTrees[y].dictTrees.FindIndex(x => x.Letter == line.Substring(4, 2));
                                        if (z == -1)
                                        {
                                            if (lenn > 5)
                                            {
                                                dt3 = new DictTree();
                                                dt3.Letter = line.Substring(4, 2);
                                                if (lenn > 7)
                                                {
                                                    dt4 = new DictTree();
                                                    dt4.Letter = line.Substring(6);
                                                    dt4.dictWords = new List<DictWord>();
                                                    DictWord dictWord = new DictWord();
                                                    dictWord.wholeWord = line;
                                                    dictWord.type = tokens[1].ToLower();
                                                    dictWord.words = new List<string>();
                                                    for (int i = 2; i < tokens.Length; i++)
                                                    {
                                                        dictWord.words.Add(tokens[i]);
                                                    }
                                                    dt4.dictWords.Add(dictWord);

                                                    dt3.dictTrees = new List<DictTree>();
                                                    dt3.dictTrees.Add(dt4);
                                                }
                                                else
                                                {
                                                    if (lenn != 6)
                                                    {
                                                        //  if (dt3.Variants == null)
                                                        //      dt3.Variants = new List<string>();
                                                        //  dt3.Variants.Add(line.Substring(6));

                                                        dt4 = new DictTree();
                                                        dt4.Letter = line.Substring(6);
                                                        dt4.dictWords = new List<DictWord>();
                                                        DictWord dictWord = new DictWord();
                                                        dictWord.wholeWord = line;
                                                        dictWord.type = tokens[1].ToLower();
                                                        dictWord.words = new List<string>();
                                                        for (int i = 2; i < tokens.Length; i++)
                                                        {
                                                            dictWord.words.Add(tokens[i]);
                                                        }
                                                        dt4.dictWords.Add(dictWord);

                                                        dt3.dictTrees = new List<DictTree>();
                                                        dt3.dictTrees.Add(dt4);
                                                    }
                                                    else
                                                    {
                                                        dt3.dictWords = new List<DictWord>();
                                                        DictWord dictWord = new DictWord();
                                                        dictWord.wholeWord = line;
                                                        dictWord.type = tokens[1].ToLower();
                                                        dictWord.words = new List<string>();
                                                        for (int i = 2; i < tokens.Length; i++)
                                                        {
                                                            dictWord.words.Add(tokens[i]);
                                                        }
                                                        dt3.dictWords.Add(dictWord);
                                                    }

                                                }
                                                if (myDict.tree[t].dictTrees[y].dictTrees == null)
                                                    myDict.tree[t].dictTrees[y].dictTrees = new List<DictTree>();
                                                myDict.tree[t].dictTrees[y].dictTrees.Add(dt3);
                                            }
                                            else
                                            {
                                                if (lenn != 4)
                                                {
                                                    //  if (myDict.tree[t].dictTrees[y].Variants == null)
                                                    //      myDict.tree[t].dictTrees[y].Variants = new List<string>();
                                                    //  myDict.tree[t].dictTrees[y].Variants.Add(line.Substring(4));

                                                    dt3 = new DictTree();
                                                    dt3.Letter = line.Substring(4);
                                                    dt3.dictWords = new List<DictWord>();
                                                    DictWord dictWord = new DictWord();
                                                    dictWord.wholeWord = line;
                                                    dictWord.type = tokens[1].ToLower();
                                                    dictWord.words = new List<string>();
                                                    for (int i = 2; i < tokens.Length; i++)
                                                    {
                                                        dictWord.words.Add(tokens[i]);
                                                    }
                                                    dt3.dictWords.Add(dictWord);

                                                    if (myDict.tree[t].dictTrees[y].dictTrees == null)
                                                        myDict.tree[t].dictTrees[y].dictTrees = new List<DictTree>();
                                                    myDict.tree[t].dictTrees[y].dictTrees.Add(dt3);

                                                }
                                                else
                                                {
                                                    if (myDict.tree[t].dictTrees[y].dictWords == null)
                                                        myDict.tree[t].dictTrees[y].dictWords = new List<DictWord>();
                                                    DictWord dictWord = new DictWord();
                                                    dictWord.wholeWord = line;
                                                    dictWord.type = tokens[1].ToLower();
                                                    dictWord.words = new List<string>();
                                                    for (int i = 2; i < tokens.Length; i++)
                                                    {
                                                        dictWord.words.Add(tokens[i]);
                                                    }
                                                    myDict.tree[t].dictTrees[y].dictWords.Add(dictWord);
                                                }

                                            }
                                        }
                                        else
                                        {
                                            if (lenn > 7)
                                            {
                                                int z1 = -1;
                                                if (myDict.tree[t].dictTrees[y].dictTrees[z].dictTrees == null)
                                                {
                                                    myDict.tree[t].dictTrees[y].dictTrees[z].dictTrees = new List<DictTree>();
                                                }
                                                else
                                                {
                                                    z1 = myDict.tree[t].dictTrees[y].dictTrees[z].dictTrees.FindIndex(x => x.Letter == line.Substring(6, 2));
                                                }
                                                if (z1 == -1)
                                                {
                                                    if (lenn > 7)
                                                    {
                                                        dt4 = new DictTree();
                                                        dt4.Letter = line.Substring(6);
                                                        dt4.dictWords = new List<DictWord>();
                                                        DictWord dictWord = new DictWord();
                                                        dictWord.wholeWord = line;
                                                        dictWord.type = tokens[1].ToLower();
                                                        dictWord.words = new List<string>();
                                                        for (int i = 2; i < tokens.Length; i++)
                                                        {
                                                            dictWord.words.Add(tokens[i]);
                                                        }
                                                        dt4.dictWords.Add(dictWord);

                                                        if (myDict.tree[t].dictTrees[y].dictTrees[z].dictTrees == null)
                                                            myDict.tree[t].dictTrees[y].dictTrees[z].dictTrees = new List<DictTree>();
                                                        myDict.tree[t].dictTrees[y].dictTrees[z].dictTrees.Add(dt4);
                                                    }
                                                    else
                                                    {
                                                        if (lenn != 6)
                                                        {
                                                            //  if (myDict.tree[t].dictTrees[y].dictTrees[z].Variants == null)
                                                            //     myDict.tree[t].dictTrees[y].dictTrees[z].Variants = new List<string>();
                                                            // myDict.tree[t].dictTrees[y].dictTrees[z].Variants.Add(line.Substring(6));

                                                            dt4 = new DictTree();
                                                            dt4.Letter = line.Substring(6);
                                                            dt4.dictWords = new List<DictWord>();
                                                            DictWord dictWord = new DictWord();
                                                            dictWord.wholeWord = line;
                                                            dictWord.type = tokens[1].ToLower();
                                                            dictWord.words = new List<string>();
                                                            for (int i = 2; i < tokens.Length; i++)
                                                            {
                                                                dictWord.words.Add(tokens[i]);
                                                            }
                                                            dt4.dictWords.Add(dictWord);

                                                            if (myDict.tree[t].dictTrees[y].dictTrees[z].dictTrees == null)
                                                                myDict.tree[t].dictTrees[y].dictTrees[z].dictTrees = new List<DictTree>();
                                                            myDict.tree[t].dictTrees[y].dictTrees[z].dictTrees.Add(dt4);

                                                        }
                                                        else
                                                        {
                                                            if (myDict.tree[t].dictTrees[y].dictTrees[z].dictWords == null)
                                                                myDict.tree[t].dictTrees[y].dictTrees[z].dictWords = new List<DictWord>();
                                                            DictWord dictWord = new DictWord();
                                                            dictWord.wholeWord = line;
                                                            dictWord.type = tokens[1].ToLower();
                                                            dictWord.words = new List<string>();
                                                            for (int i = 2; i < tokens.Length; i++)
                                                            {
                                                                dictWord.words.Add(tokens[i]);
                                                            }
                                                            myDict.tree[t].dictTrees[y].dictTrees[z].dictWords.Add(dictWord);
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    if (lenn != 8)
                                                    {
                                                        // if (myDict.tree[t].dictTrees[y].dictTrees[z].dictTrees[z1].Variants == null)
                                                        //     myDict.tree[t].dictTrees[y].dictTrees[z].dictTrees[z1].Variants = new List<string>();
                                                        // myDict.tree[t].dictTrees[y].dictTrees[z].dictTrees[z1].Variants.Add(line.Substring(6));

                                                        dt4 = new DictTree();
                                                        dt4.Letter = line.Substring(8);
                                                        dt4.dictWords = new List<DictWord>();
                                                        DictWord dictWord = new DictWord();
                                                        dictWord.wholeWord = line;
                                                        dictWord.type = tokens[1].ToLower();
                                                        dictWord.words = new List<string>();
                                                        for (int i = 2; i < tokens.Length; i++)
                                                        {
                                                            dictWord.words.Add(tokens[i]);
                                                        }
                                                        dt4.dictWords.Add(dictWord);

                                                        if (myDict.tree[t].dictTrees[y].dictTrees[z].dictTrees[z1].dictTrees == null)
                                                            myDict.tree[t].dictTrees[y].dictTrees[z].dictTrees[z1].dictTrees = new List<DictTree>();
                                                        myDict.tree[t].dictTrees[y].dictTrees[z].dictTrees[z1].dictTrees.Add(dt4);



                                                    }
                                                    else
                                                    {
                                                        if (myDict.tree[t].dictTrees[y].dictTrees[z].dictTrees[z1].dictWords == null)
                                                            myDict.tree[t].dictTrees[y].dictTrees[z].dictTrees[z1].dictWords = new List<DictWord>();
                                                        DictWord dictWord = new DictWord();
                                                        dictWord.wholeWord = line;
                                                        dictWord.type = tokens[1].ToLower();
                                                        dictWord.words = new List<string>();
                                                        for (int i = 2; i < tokens.Length; i++)
                                                        {
                                                            dictWord.words.Add(tokens[i]);
                                                        }
                                                        myDict.tree[t].dictTrees[y].dictTrees[z].dictTrees[z1].dictWords.Add(dictWord);
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                if (lenn != 6)
                                                {
                                                    // if (myDict.tree[t].dictTrees[y].dictTrees[z].Variants == null)
                                                    //      myDict.tree[t].dictTrees[y].dictTrees[z].Variants = new List<string>();
                                                    // myDict.tree[t].dictTrees[y].dictTrees[z].Variants.Add(line.Substring(6));

                                                    dt4 = new DictTree();
                                                    dt4.Letter = line.Substring(6);
                                                    dt4.dictWords = new List<DictWord>();
                                                    DictWord dictWord = new DictWord();
                                                    dictWord.wholeWord = line;
                                                    dictWord.type = tokens[1].ToLower();
                                                    dictWord.words = new List<string>();
                                                    for (int i = 2; i < tokens.Length; i++)
                                                    {
                                                        dictWord.words.Add(tokens[i]);
                                                    }
                                                    dt4.dictWords.Add(dictWord);

                                                    if (myDict.tree[t].dictTrees[y].dictTrees[z].dictTrees == null)
                                                        myDict.tree[t].dictTrees[y].dictTrees[z].dictTrees = new List<DictTree>();
                                                    myDict.tree[t].dictTrees[y].dictTrees[z].dictTrees.Add(dt4);

                                                }
                                                else
                                                {
                                                    if (myDict.tree[t].dictTrees[y].dictTrees[z].dictWords == null)
                                                        myDict.tree[t].dictTrees[y].dictTrees[z].dictWords = new List<DictWord>();
                                                    DictWord dictWord = new DictWord();
                                                    dictWord.wholeWord = line;
                                                    dictWord.type = tokens[1].ToLower();
                                                    dictWord.words = new List<string>();
                                                    for (int i = 2; i < tokens.Length; i++)
                                                    {
                                                        dictWord.words.Add(tokens[i]);
                                                    }
                                                    myDict.tree[t].dictTrees[y].dictTrees[z].dictWords.Add(dictWord);
                                                }

                                                //// нашли хотя здесь в одной букве могут быть различия
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (lenn != 4)
                                        {
                                            //// нашли хотя здесь в одной букве могут быть различия
                                            // if (myDict.tree[t].dictTrees[y].Variants == null)
                                            //     myDict.tree[t].dictTrees[y].Variants = new List<string>();
                                            //myDict.tree[t].dictTrees[y].Variants.Add(line.Substring(4));

                                            dt4 = new DictTree();
                                            dt4.Letter = line.Substring(4);
                                            dt4.dictWords = new List<DictWord>();
                                            DictWord dictWord = new DictWord();
                                            dictWord.wholeWord = line;
                                            dictWord.type = tokens[1].ToLower();
                                            dictWord.words = new List<string>();
                                            for (int i = 2; i < tokens.Length; i++)
                                            {
                                                dictWord.words.Add(tokens[i]);
                                            }
                                            dt4.dictWords.Add(dictWord);

                                            if (myDict.tree[t].dictTrees[y].dictTrees == null)
                                                myDict.tree[t].dictTrees[y].dictTrees = new List<DictTree>();
                                            myDict.tree[t].dictTrees[y].dictTrees.Add(dt4);

                                        }
                                        else
                                        {
                                            if (myDict.tree[t].dictTrees[y].dictWords == null)
                                                myDict.tree[t].dictTrees[y].dictWords = new List<DictWord>();
                                            DictWord dictWord = new DictWord();
                                            dictWord.wholeWord = line;
                                            dictWord.type = tokens[1].ToLower();
                                            dictWord.words = new List<string>();
                                            for (int i = 2; i < tokens.Length; i++)
                                            {
                                                dictWord.words.Add(tokens[i]);
                                            }
                                            myDict.tree[t].dictTrees[y].dictWords.Add(dictWord);
                                        }


                                    }
                                }
                            }
                            else
                            {
                                if (lenn != 2)
                                {
                                    //  if (myDict.tree[t].Variants == null)
                                    //      myDict.tree[t].Variants = new List<string>();
                                    //  myDict.tree[t].Variants.Add(line.Substring(2));

                                    dt4 = new DictTree();
                                    dt4.Letter = line.Substring(2);
                                    dt4.dictWords = new List<DictWord>();
                                    DictWord dictWord = new DictWord();
                                    dictWord.wholeWord = line;
                                    dictWord.type = tokens[1].ToLower();
                                    dictWord.words = new List<string>();
                                    for (int i = 2; i < tokens.Length; i++)
                                    {
                                        dictWord.words.Add(tokens[i]);
                                    }
                                    dt4.dictWords.Add(dictWord);

                                    if (myDict.tree[t].dictTrees == null)
                                        myDict.tree[t].dictTrees = new List<DictTree>();
                                    myDict.tree[t].dictTrees.Add(dt4);

                                }
                                else
                                {
                                    if (myDict.tree[t].dictWords == null)
                                        myDict.tree[t].dictWords = new List<DictWord>();
                                    DictWord dictWord = new DictWord();
                                    dictWord.wholeWord = line;
                                    dictWord.type = tokens[1].ToLower();
                                    dictWord.words = new List<string>();
                                    for (int i = 2; i < tokens.Length; i++)
                                    {
                                        dictWord.words.Add(tokens[i]);
                                    }
                                    myDict.tree[t].dictWords.Add(dictWord);
                                }

                            }
                        }
                    }
                    else
                    {
                        int t = -1;
                        if (myDict.tree == null)
                            myDict.tree = new List<DictTree>();
                        else
                            t = myDict.tree.FindIndex(x => x.Letter == line.Substring(0));
                        if (t == -1)
                        {
                            dt = new DictTree();
                            dt.Letter = line.Substring(0);
                            if (myDict.tree == null)
                                myDict.tree = new List<DictTree>();
                            myDict.tree.Add(dt);
                        }
                        else
                        {
                            if (myDict.tree[t].dictWords == null)
                                myDict.tree[t].dictWords = new List<DictWord>();
                            DictWord dictWord = new DictWord();
                            dictWord.wholeWord = line;
                            dictWord.type = tokens[1].ToLower();
                            dictWord.words = new List<string>();
                            for (int i = 2; i < tokens.Length; i++)
                            {
                                dictWord.words.Add(tokens[i]);
                            }
                            myDict.tree[t].dictWords.Add(dictWord);


                            // if (myDict.tree[t].Variants == null)
                            //     myDict.tree[t].Variants = new List<string>();
                            // myDict.tree[t].Variants.Add(line.Substring(0));
                        }
                    }
                }

                // BinaryFormatter сохраняет данные в двоичном формате. Чтобы получить доступ к BinaryFormatter, понадобится
                // импортировать System.Runtime.Serialization.Formatters.Binary
                BinaryFormatter binFormat = new BinaryFormatter();
                // Сохранить объект в локальном файле.
                using (Stream fStream = new FileStream("user.dat",
                   FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    binFormat.Serialize(fStream, myDict);
                }
                MessageBox.Show("ВСЕ!");
            }
        }

        private void loadUserDict()
        {
            BinaryFormatter binFormat = new BinaryFormatter();

            using (Stream fStream = File.OpenRead("user.dat"))
            {
                loadedMyDict = (MyDict)binFormat.Deserialize(fStream);
            }
            MessageBox.Show("ВСЕ!");
        }

        private void loadDictXML_Click(object sender, RoutedEventArgs e)
        {
            loadUserDict();
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

            //string[] allText = File.ReadAllLines("mnist_train_100.csv", Encoding.GetEncoding(1251));
            //string[] allText = File.ReadAllLines("mnist_train.csv", Encoding.GetEncoding(1251));


            //NeuralNet myNNet = new NeuralNet(784, 200, 10, 7, 0.1f);

            NeuralNet myNNet;

            try
            {
                myNNet = NeuralNet.Load("weights");
                if (myNNet == null)
                {
                    myNNet = new NeuralNet(784, 200, 10, 7, 0.1f);
                    trainSet = TrainSet.Load("MNIST");
                    if (trainSet==null)
                    {
                        trainSet = new TrainSet(10, 784);
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                             (ThreadStart)delegate ()
                                {
                                    myDataContext.strings.Add("Start to train dataset!");
                                }
                             );

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

                }
            }

            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            // trainSet = new TrainSet(10, 784);
            /*
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    myDataContext.strings.Add("Start to train dataset!");
                }
                );
            
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
            */


            /*

            trainSet = TrainSet.Load("MNIST");
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

            */


            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    myDataContext.strings.Add("Finish train dataset! Time: " + DateTime.Now.ToString("mm:ss:ffff"));
                    myDataContext.strings.Add("Start to test dataset!");
                }
                );

            // trainSet.Save("MNIST");
            //trainSet.ClearTrainSet();
            


            /*this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    myDataContext.strings.Add("Start to load weights! Time: " + DateTime.Now.ToString("mm:ss:ffff"));
                }
                );

            

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    myDataContext.strings.Add("End load weights! Time: " + DateTime.Now.ToString("mm:ss:ffff"));
                }
                );*/

            //string[] allText1 = File.ReadAllLines("mnist_test_10.csv", Encoding.GetEncoding(1251));
            string[] allText1 = File.ReadAllLines("mnist_test.csv", Encoding.GetEncoding(1251));
            string[] tokens1;

            int countAll = 0;
            int countRight = 0;

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


                int yy = 1;
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
}
