using Microsoft.VisualStudio.TestTools.UnitTesting;
using WpfApp1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Tests
{
    [TestClass()]
    public class MatrixTests
    {
        [TestMethod()]
        public void MatrixTestIfRowsOrColsLessOrEqual0()
        {
            try
            {
                Matrix test = new Matrix(-1, -1);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public void mulAdamarTest()
        {
            try
            {
                Matrix testA = new Matrix(1, 1);
                Matrix testB = new Matrix(1, 1);

                bool ident = testA == testB;
                ident = testA != testB;
                ident = testA != null;
                ident = testA == null;

                ident =  testA.MakeIdentity();
                ident = testB.MakeIdentity();
                Matrix testC = Matrix.mulAdamar(testA, testB);
                testC = testA + testB;
                testC = testC - testA;

            }
            catch
            {
                Assert.Fail();
            }
        }
    }
}