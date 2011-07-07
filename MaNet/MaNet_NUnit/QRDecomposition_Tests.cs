using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MaNet;
using System.Diagnostics;
using smt = MaNet_NUnit.StandardMatrixTests;
namespace MaNet_NUnit
{
  [TestFixture]
  public class QRDecomposition_Tests
    {

     
        #region 2x2 matrices
        [Test]
        public void TwoDimensional_UniqueSolution()
        {
            string strA = @"2	1
                            1	2";
            Matrix A = Matrix.Parse(strA);

            string strX = @"3 
                            4";
            Matrix X = Matrix.Parse(strX);

            Assert.That(A.Rank(), Is.EqualTo(2));// Rows are linearly independent.

            QRDecomposition QRofA = new QRDecomposition(A);

            Matrix Q = QRofA.GetQ();
            Matrix Identity = Matrix.Identity(2, 2);
            Matrix ExpectedIdent = Q.Times(Q.Transpose());
            Assert.That(ExpectedIdent , Is.EqualTo(Identity ).Within(.00000000001)); // Q is Orthogonal
           Matrix R = QRofA.GetR();
           Assert.That(smt.IsUpperTriangular(R), Is.True); // R is upper Triangular
           Assert.That(Q.Times(R), Is.EqualTo(A).Within(.00000000001)); // A=QR

           Assert.That(QRofA.IsFullRank(), Is.True);
           Matrix S =    QRofA.Solve(X);


            // AS=B so S is the solution
           Assert.That(A.Times(S), Is.EqualTo(X).Within(.00000000001));

        }

      


       
     
        

        [Test]
        public void SolveException_Test()
        {
            string strA = @"1  2
                            3  6"; //Rank Deficient

            string strBWrongDim = @"1
                                    2
                                    3";

            string strB = @"1
                            2";

            Matrix A = Matrix.Parse(strA);
            Matrix B = Matrix.Parse(strBWrongDim);

            QRDecomposition qr = A.Qr();

            Assert.Throws(typeof(ArgumentException), delegate { qr.Solve(B); });

             B = Matrix.Parse(strB );
            Assert.That(A.Rank() , Is.EqualTo(1));

            Assert.That(qr.IsFullRank(), Is.True );//Big Problem as A is not of full rank;

            //note that this is a Bit of a problem as this number should be 0
            Assert.That(qr.GetR().Array[1][ 1], Is.EqualTo(-0.00000000000000088817841970012523));

            //For that reason we have the RankTolerance Property
            qr.RankTolerance = .000000000000001;
            Assert.That(qr.RankTolerance, Is.EqualTo(.000000000000001));


            //With the change in tolerance
            Assert.That(qr.IsFullRank(), Is.False);
           
            //With the rank Tolerance set we will throw the appropriate exception. 
            Assert.Throws(typeof(Exception), delegate { qr.Solve(B); });
        }
#endregion

      [Test]
        public void ThreeDimension()
        {
            string strA = @"0.8491    0.7577    0.6555
                            0.9340    0.7431    0.1712
                            0.6787    0.3922    0.7060";

            string strH = @"1.5924    0         0 
                            0.6517    1.1484    0 
                            0.4735    0.9889    2"; //Expected Householder vectors
          


            Matrix A = Matrix.Parse(strA);
            QRDecomposition qr = A.Qr();

          Assert.That(qr.GetH(), Is.EqualTo(Matrix.Parse(strH)).Within(.0001));

         

        }

       
    }
}
