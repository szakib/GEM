using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MaNet;
using smt = MaNet_NUnit.StandardMatrixTests;
using System.Diagnostics;
using MaNet.Generators;
namespace MaNet_NUnit
{
    [TestFixture]
   public  class EigenvalueDecomposition_Tests
    {

        [Test]
        public void TwoByTwo_Symetric()
        {
 
            string strA = @"2	1
                            1	2";
            Matrix A = Matrix.Parse(strA);

            string strExpectedD = @"1  0
                                    0  3";
            Matrix ExpectedD = Matrix.Parse(strExpectedD);


            string strExpectedV = @" -0.7071     0.7071
                                      0.7071     0.7071";

            Matrix ExpectedV = Matrix.Parse(strExpectedV);
            //This is the value that one will get from Matlab. The eigenvalue Decomposition returns 
            //the  following           0.7071     0.7071
            //                        -0.7071     0.7071"
            // (Warning Proof ahead )
            //The problem stems from the fact that the Eigenvalue decomposition is not entirely unique.
            // The A is decomposed such that A = V * D * V';
            // Consider the diagonal  matrix J such that all of its diagonal values are eiher 1 or -1. J*J = I.
            // (V *J) * D * (V * J)' = V * J * D * J' * V'  using (AB)' = B'A' The transpose of the product is the transpose of the elements reversed.
            //  V * J * D * J' * V'  =  V * J * D * J * V'  as J is diagonal
            //  V * J * D * J * V'   = V * J * J * D * V' as J and D are diagonal.
            //  V * J * J * D * V'  = V * D * V' since J*J = I.
            // (Proof finished)
            //  In practical terms this means that    Assert.That(V, Is.EqualTo(ExpectedV) );
            //  is not a good test. and I will need to test if it is equivalent instead. 


            

            Assert.That(smt.IsSymetric(A), Is.True);

            EigenvalueDecomposition EofA = new EigenvalueDecomposition(A);

           

           double[] realEigenValues= EofA.getRealEigenvalues();
           double[] imaginaryEigenValues = EofA.getRealEigenvalues();

            Matrix V = EofA.getV();
            Debug.WriteLine(V.ToString());
           // Assert.That(V, Is.EqualTo(ExpectedV) ); Not enough uniqueness so does not work
            TestEigenvalueVEquivalent(V, ExpectedV);

            // V is orthogonal V times V transpose is the identity
            Assert.That(V.Times(V.Transpose()), Is.EqualTo(Matrix.Identity(V.RowDimension, V.RowDimension)).Within(.0000001));

            Matrix D = EofA.getD();
            Assert.That(smt.IsDiagonal(D), Is.True); //Diagonal which for 2x2 is diagonal
            Assert.That(D, Is.EqualTo(ExpectedD).Within (10).Ulps);

            //V * D * V,transpose = A
            Assert.That((V.Times(D.Times(V.Transpose()))).Array, Is.EqualTo(A.Array).Within(.0000001));

             

        }


//        [Test]
//        public void TwoByTwo_Singular()
//        {
//            string strSingular = @"1     2
//                                   3     6";
//            Matrix S = Matrix.Parse(strSingular);
//            EigenvalueDecomposition EofA = new EigenvalueDecomposition(S);


//        }


        [Test]
        public void TwoByTwo_AntiSymetric()
        {
            string strAnti = @"2    1
                              -1    2";

            Matrix A = Matrix.Parse(strAnti);
            EigenvalueDecomposition EofA = new EigenvalueDecomposition(A);
            Assert.That(EofA.getRealEigenvalues() , Is.EqualTo(new double[ ]{2,2}));
            Assert.That(EofA.getImagEigenvalues(), Is.EqualTo(new double[] { 1, -1 }));

        }

        [Test]
        public void LargeSymetric()
        {
            Matrix K = Specialized.K(100);
            TestDecomposition(K);

        }

        [Test]
        public void Zeromatrix()
        {
            Matrix Z = new Matrix(3);
            TestDecomposition(Z);


        }
 
 

        [Test]
        public void CloseEigenvaluesTest()
        {
            //This should produce two distinct small eigwnvalues that are close together.
            // http://www.mathworks.com/company/newsletters/news_notes/pdf/sum95cleve.pdf
            string aBase = @"0   1    0   0 
                             1   0   -d   0
                             0   d    0   1 
                             0   0    1   0";

            Matrix A = Parser.Substitute(aBase, "d", .0000000001);
            TestDecomposition(A);



        }

        private void TestDecomposition(Matrix A)
        {
            EigenvalueDecomposition EofA = new EigenvalueDecomposition(A);

            Matrix V = EofA.getV();
            Matrix D = EofA.getD();

            if (smt.IsSymetric(A))
            {

                // V is orthogonal V times V transpose is the identity
                Assert.That(V.Times(V.Transpose()).Array, Is.EqualTo(Matrix.Identity(V.RowDimension, V.RowDimension).Array).Within(.0000001));
                Assert.That((V.Times(D.Times(V.Transpose()))).Array, Is.EqualTo(A.Array).Within(.0000001));

            }
            else 
            {

                Assert.That(A * V, Is.EqualTo(V * D).Within(.0000001));

            }

        }

        private void TestEigenvalueVEquivalent(Matrix A, Matrix B)
        {
            double[] diagA = A.GetDiagonal();
            double[] diagB = B.GetDiagonal();

            double[] diaSignA = new double[diagA.Length];
            double[] diaSignB = new double[diagB.Length];
            for (int i = 0; i < diagA.Length; i++)
            {
                diaSignA[i] =  Math.Sign(diagA[i]) ;
                diaSignB[i] = Math.Sign(diagB[i]);
            }
            Matrix JA = Matrix.Diagonal(diaSignA);
            Matrix JB = Matrix.Diagonal(diaSignB);

            Assert.That(A * JA, Is.EqualTo(B * JB).Within(.001));

        }

        [Test]
        public void TwoByTwo_NonSymetric()
        {

            string strA = @"1	2
                            3	4";
            Matrix A = Matrix.Parse(strA);
 
            Assert.That(smt.IsSymetric(A), Is.False);

            EigenvalueDecomposition EofA = new EigenvalueDecomposition(A);

            Matrix V = EofA.getV();
            Matrix D = EofA.getD();
            Assert.That(smt.IsDiagonal(D), Is.True); //Block Diagonal which for 2x2 is diagonal

            // V*D * V.Inverse = A
            Assert.That((V.Times(D.Times(V.Inverse()))).Array, Is.EqualTo(A.Array).Within(.0000001).Percent );

            // A.times(V) equals V.times(D)
            Assert.That((A.Times(V)).Array, Is.EqualTo(V.Times(D).Array).Within(.0000001).Percent);
        }


        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(12)]
        public void Random_NByN_NonSymetric(int n)
        {
            Rectangular rand = new Rectangular();
            Matrix A = rand.RandomDouble(n, n);

            EigenvalueDecomposition EofA = new EigenvalueDecomposition(A);

            Matrix V = EofA.getV();
         

            Matrix D = EofA.getD();
            // V*D * V.Inverse = A
            Assert.That((V.Times(D.Times(V.Inverse()))).Array, Is.EqualTo(A.Array).Within(.0000001).Percent);
            // A.times(V) equals V.times(D)
            Assert.That((A.Times(V)).Array, Is.EqualTo(V.Times(D).Array).Within(.0000001).Percent);

        }


        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(12)]
        public void Random_NByN_Symetric(int n)
        {
            Rectangular rand = new Rectangular();
            Matrix A = rand.RandomDouble(n, n);
            A = A.Plus(A.Transpose()); /// Any matrix added to its transpose will be symetric
            Assert.That(smt.IsSymetric(A), Is.True);

            EigenvalueDecomposition EofA = new EigenvalueDecomposition(A);

            Matrix V = EofA.getV();
            // V is orthogonal V times V transpose is the identity
            Assert.That(V.Times(V.Transpose()).Array, Is.EqualTo(Matrix.Identity(V.RowDimension, V.RowDimension).Array).Within(.0000001));

            Matrix D = EofA.getD();

            Assert.That((V.Times(D.Times(V.Transpose()))).Array, Is.EqualTo(A.Array).Within(.0000001).Percent);



        }

        [Test]
        public void Rosser_Test()
        {
            Matrix R = Specialized.Rosser();


            EigenvalueDecomposition EofA = new EigenvalueDecomposition(R);

            Matrix V = EofA.getV();
            // V is orthogonal V times V transpose is the identity
            Assert.That(V.Times(V.Transpose()).Array, Is.EqualTo(Matrix.Identity(V.RowDimension, V.RowDimension).Array).Within(.0000001));

            Matrix D = EofA.getD();

            Assert.That((V.Times(D.Times(V.Transpose()))).Array, Is.EqualTo(R.Array).Within(.0000001).Percent);



        }



    }
}
