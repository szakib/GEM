using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MaNet;
using smt = MaNet_NUnit.StandardMatrixTests;
namespace MaNet_NUnit
{
    [TestFixture]
  public  class LUDecomposition_Tests
    {

        [Test]
        public void TwoDimensional_UniqueSolution()
        {
            string strA = @"2	1
                            1	2";
            Matrix A = Matrix.Parse(strA);

            string strExpectedL = @"1.0    0
                                    0.5    1.0";

            Matrix ExpectedL = Matrix.Parse(strExpectedL);


            string strExpectedU = @" 2.0  1.0
                                     0    1.5";

            Matrix ExpectedU = Matrix.Parse(strExpectedU);

           

            string strX = @"3 
                            4";
            Matrix X = Matrix.Parse(strX);

            Assert.That(A.Rank(), Is.EqualTo(2));// Rows are linearly independent.

            LUDecomposition LUofA = new LUDecomposition(A);

            Matrix L = LUofA.GetL();
            Assert.That(L, Is.EqualTo(ExpectedL));
            Assert.That(smt.IsLowerTriangular(L), Is.True);

            Matrix U = LUofA.GetU();
            Assert.That(U, Is.EqualTo(ExpectedU));
            Assert.That(smt.IsUpperTriangular(U), Is.True);

           

            Assert.That(L.Times(U), Is.EqualTo(A));

            Assert.That(LUofA.IsNonsingular(), Is.True);
            Matrix S = LUofA.solve(X);

            // AS=B so S is the solution
            Assert.That(A.Times(S), Is.EqualTo(X).Within(.00000000001));

        }


        [Test]
        public void TwoDimensional_Singular()
        {
            string strA = @"2	4
                            1	2";
            Matrix A = Matrix.Parse(strA);

            string strX = @"3 
                            4";
            Matrix X = Matrix.Parse(strX);

            Assert.That(A.Rank(), Is.EqualTo(1));// Rows are linearly dependent.

            LUDecomposition LUofA = new LUDecomposition(A);

            Matrix L = LUofA.GetL();
            Assert.That(smt.IsLowerTriangular(L), Is.True);

            Matrix U = LUofA.GetU();
            Assert.That(smt.IsUpperTriangular(U), Is.True);

            Assert.That(L.Times(U), Is.EqualTo(A));

            Assert.That(LUofA.IsNonsingular(), Is.False);
        

            // Attempt to solve throws exception
            Assert.Throws(typeof(System.Exception), delegate { LUofA.solve(X); });

        }


        [Test]
        public void TwoDimensional_Fat()
        {
            string strA = @"1	2   3
                            4	5   6";
            Matrix A = Matrix.Parse(strA);

            string strExpectedL = @"1.0000         0
                                    0.2500    1.0000";

            Matrix ExpectedL = Matrix.Parse(strExpectedL);

            string strExpectedU = @"4.00  5.00  6.00
                                    0     0.75  1.50";

            Matrix ExpectedU = Matrix.Parse(strExpectedU);

            string strExpectedP = @"0  1
                                    1  0";

            Matrix ExpectedP = Matrix.Parse(strExpectedP);

            string strX = @"7 
                            8";
            Matrix X = Matrix.Parse(strX);

            Assert.That(A.Rank(), Is.EqualTo(2));// Rows are linearly independent.

            LUDecomposition LUofA = new LUDecomposition(A);
            

            Matrix L = LUofA.GetL();
            Assert.That(L.RowDimension, Is.EqualTo(2));
            Assert.That(L.ColumnDimension, Is.EqualTo(2));
            Assert.That(L, Is.EqualTo(ExpectedL));

            //Assert.That(smt.IsLowerTriangular(L), Is.True);

            Matrix U = LUofA.GetU();
            Assert.That(smt.IsUpperTriangular(U), Is.True);
            Assert.That(U, Is.EqualTo(ExpectedU));

            Matrix P = LUofA.GetP();
            Assert.That(P, Is.EqualTo(ExpectedP));
            Assert.That(LUofA.getDoublePivot(), Is.EqualTo(new double[] { 1.0, 0.0 }));

            Assert.That(L.Times(U), Is.EqualTo(P.Times(A)));
            Assert.That(L * U, Is.EqualTo(P*A)); // In Operator Form

           


            // Attempt to solve throws exception
            Assert.Throws(typeof(System.Exception), delegate { LUofA.solve(X); });

            //Determinent throws exception as A is not square
            Assert.Throws(typeof(System.ArgumentException), delegate { LUofA.det() ; });

        }


        [Test]
        public void TwoDimensional_Skinny()
        {
            string strA = @"1	2 
                            3	4   
                            5   6";
            Matrix A = Matrix.Parse(strA);

            string strExpectedL = @"1.0  0
                                    0.2  1.0
                                    0.6  0.5";

            Matrix ExpectedL = Matrix.Parse(strExpectedL);


            string strExpectedU = @"5.0  6.0
                                    0    0.8";

            Matrix ExpectedU = Matrix.Parse(strExpectedU);

            string strExpectedP = @"0  0  1
                                    1  0  0
                                    0  1  0";

            Matrix ExpectedP = Matrix.Parse(strExpectedP);

            string strX = @"7 
                            8";
            Matrix X = Matrix.Parse(strX);

            Assert.That(A.Rank(), Is.EqualTo(2));// Columns are linearly dependent.

            LUDecomposition LUofA = new LUDecomposition(A);

            Matrix L = LUofA.GetL();
            Assert.That(smt.IsLowerTriangular(L), Is.True);
            Assert.That(L, Is.EqualTo(ExpectedL).Within(100).Ulps);

            Matrix U = LUofA.GetU();
            Assert.That(smt.IsUpperTriangular(U), Is.True);
            Assert.That(U, Is.EqualTo(ExpectedU).Within(100).Ulps);

            Matrix P = LUofA.GetP();
            Assert.That(P, Is.EqualTo(ExpectedP));

            Assert.That(L.Times(U), Is.EqualTo(P.Times(A)));

         

            // Attempt to solve throws exception
            Assert.Throws(typeof(System.ArgumentException), delegate { LUofA.solve(X); });



        }


     
    }
}
