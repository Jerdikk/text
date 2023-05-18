using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class GlobalData
    {
        private static object syncRoot = new Object();
        private static volatile GlobalData instance;
        public static GlobalData Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new GlobalData();
                    }
                }

                return instance;
            }
        }

        public double progress01;
        public double progress02;
        public string fileName;
        public Encoding encoding;
    }
}
