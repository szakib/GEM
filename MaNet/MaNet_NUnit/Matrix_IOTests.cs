using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using smt = MaNet_NUnit.StandardMatrixTests;
using System.Data;
using MaNet.Generators;
using MaNet;

namespace MaNet_NUnit
{
    [TestFixture]
 public class Matrix_IOTests
    {
        [Test]
        public void ToString_Test()
        {
            Matrix A = new Matrix(2);
            Assert.That(A.ToString() , Is.EqualTo( "[0 0;0 0]"));
            Assert.That(A.ToString("<", "{", "\n", ", ", "}", ">"), Is.EqualTo("<{0, 0}\n{0, 0}>"));
            A = new Matrix(2,2,9);
            Assert.That(A.ToString(), Is.EqualTo("[9 9;9 9]"));
            Assert.That(A.ToString("<", "{", "\n", ", ", "}", ">"), Is.EqualTo("<{9, 9}\n{9, 9}>"));

        }


        [Test]
        public void Parse_Test()
        {
            Matrix A = Matrix.Parse("9 9\n9 9");
            Assert.That(A, Is.EqualTo(new Matrix(2,2,9)));
            A = Matrix.Parse("<{9, 9}\n{9, 9}>", "<", "{", "\n", ", ", "}", ">");
            Assert.That(A, Is.EqualTo(new Matrix(2, 2, 9)));
        }

        [Test]
        [TestCase(2, 2, 1)]
        public void ToStringParse_CycleTest(int m, int n, int timesToRun)
        {
            Rectangular rand = new Rectangular();
            

            for (int i = 0; i < timesToRun; i++)
            {
                Matrix A = rand.RandomDouble(m, n);
                string strA = A.ToString();
                Matrix AReconstituted = Matrix.Parse(strA);
                Assert.That(AReconstituted , Is.EqualTo(A  ));
            }

            for (int i = 0; i < timesToRun; i++)
            {
                Matrix A = rand.RandomDouble(m, n);
                string strA = A.ToString("<", "{", "\n", ", ", "}", ">");
                Matrix AReconstituted = Matrix.Parse(strA, "<", "{", "\n", ", ", "}", ">");
                Assert.That(AReconstituted , Is.EqualTo(A ));
            }

        }
        

        [Test]
        public void ToMatLabString_Test()
        {
            Matrix A = Matrix.Parse("1 2\n3 4");
            Assert.That(A.ToMatLabString(), Is.EqualTo("[1 2;3 4]"));
        }


        [Test]
        public void ParseMatLab_Test()
        {
            Matrix A = Matrix.ParseMatLab("[1 2;3 4]");
            Assert.That(A , Is.EqualTo(Matrix.Parse("1 2\n3 4")));
        }


        [Test]
        [TestCase(2, 2, 1)]
        public void ToMatLabStringParse_CycleTest(int m, int n, int timesToRun)
        {
            Rectangular rand = new Rectangular();

            for (int i = 0; i < timesToRun; i++)
            {
                Matrix A = rand.RandomDouble(m, n);
                string strA = A.ToMatLabString();
                Matrix AReconstituted = Matrix.ParseMatLab(strA);
                Assert.That(AReconstituted, Is.EqualTo(A));
            }
        }

        [Test]
        public void ToMathematicaString_Test()
        {
            Matrix A = Matrix.Parse("1 2\n3 4");
            Assert.That(A.ToMathematicaString(), Is.EqualTo("{{1, 2}, {3, 4}}"));
        }


        [Test]
        public void ParseMathematica_Test()
        {
            Matrix A = Matrix.ParseMathematica("{{1, 2}, {3, 4}}");
            Assert.That(A, Is.EqualTo(Matrix.Parse("1 2\n3 4")));
        }


        [Test]
        [TestCase(2, 2, 1)]
        public void ToMathematicaStringParse_CycleTest(int m, int n, int timesToRun)
        {
            Rectangular rand = new Rectangular();

            for (int i = 0; i < timesToRun; i++)
            {
                Matrix A = rand.RandomDouble(m, n);
                string strA = A.ToMathematicaString();
                Matrix AReconstituted = Matrix.ParseMathematica(strA);
                Assert.That(AReconstituted, Is.EqualTo(A));
            }
        }

        [Test]
        public void ToArrayString_Test()
        {
            Matrix A = Matrix.ParseMatLab("[1 2;3 4]");
            Assert.That(A.ToArrayString(), Is.EqualTo("new double[2][] { new double[2] { 1, 2 }, new double[2] { 3, 4 } }"));

        }

        [Test]
        public void FromDataTable_Test()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Col0", typeof(double));
            dt.Columns.Add("Col1", typeof(double));
            dt.Rows.Add(1, 2);
            dt.Rows.Add(3, 4);
            dt.Rows.Add(5, 6);

            Matrix A  = Matrix.FromDataTable(dt);
            Assert.That(A.ToMatLabString(), Is.EqualTo("[1 2;3 4;5 6]"));


        }


        [Test]
        public void ToDataTable_Test()
        {
            Matrix A = Matrix.ParseMatLab("[1 2;3 4;5 6]");
            DataTable dt = A.ToDataTable();
            Assert.That((double)dt.Rows[0][0], Is.EqualTo(1)) ;
            Assert.That((double)dt.Rows[0][1], Is.EqualTo(2));
            Assert.That((double)dt.Rows[1][0], Is.EqualTo(3));
            Assert.That((double)dt.Rows[1][1], Is.EqualTo(4));
            Assert.That((double)dt.Rows[2][0], Is.EqualTo(5));
            Assert.That((double)dt.Rows[2][1], Is.EqualTo(6));


        }

        [Test]
        [TestCase(2, 3, 4)]
        public void ToFromDataTable_CycleTest(int m, int n, int timesToRun)
        {
            Rectangular rand = new Rectangular();

               for (int i = 0; i < timesToRun; i++)
               {
                   Matrix A = rand.RandomDouble(m, n);
                   DataTable dt = A.ToDataTable();
                   Matrix AReconstituted = Matrix.FromDataTable(dt);
                   Assert.That(AReconstituted, Is.EqualTo(A));

               }

        }



        [Test]
        public void DisplayText_Test()
        {
            Matrix A = Matrix.ParseMatLab("[1 2;3 4;5 6]");
            Assert.That(A.DisplayText(0), Is.EqualTo("1 2\n3 4\n5 6"));
            Assert.That(A.DisplayText(1), Is.EqualTo("1.0 2.0\n3.0 4.0\n5.0 6.0"));


            A = Matrix.ParseMatLab("[1.1 2.1;3.1 4.1;5.1 6.1]");

            Assert.That(A.DisplayText(0), Is.EqualTo("1 2\n3 4\n5 6"));
            Assert.That(A.DisplayText(1), Is.EqualTo("1.1 2.1\n3.1 4.1\n5.1 6.1"));

            A = Matrix.ParseMatLab("[1.6 2.6;3.6 4.6;5.6 6.6]");
            Assert.That(A.DisplayText(0), Is.EqualTo("2 3\n4 5\n6 7"));

            A = Matrix.ParseMatLab("[1 2;30 40;500 600]");
            Assert.That(A.DisplayText(0), Is.EqualTo("  1   2\n 30  40\n500 600"));
        }

    }
}
