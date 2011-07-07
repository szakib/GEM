using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MaNet;
using System.Diagnostics;
namespace MaNet_NUnit
{
    [TestFixture]
  public  class CholeskyDecomposition_Tests
    {
        [Test]
        public void CholeskyDecomposition_Test()
        {
            string strD = @"  2    -1     0
                             -1     2    -1
                              0    -1     2";
            Matrix D =  Matrix.Parse(strD); // D is positive definite

        //L created in matlab however note that Matlab's convention is that the 
        // matrix is upper rather than lower triangular
        String strL = @"1.4142         0         0
                       -0.7071    1.2247         0
                             0   -0.8165    1.1547";

        string strB = @"1
                        2
                        3";

        string strExSln = @"2.5
                          4.0 
                          3.5"; //Expected Solution


        Matrix L = Matrix.Parse(strL);


        CholeskyDecomposition chol = new CholeskyDecomposition(D);
        Assert.That(chol.getL(), Is.EqualTo(L).Within(.0001));

          //This is the same as callin  D.chol();
        Assert.That(D.Chol().getL(), Is.EqualTo(L).Within(.0001));

        //Definition of Decomposition A = LL'
        Assert.That(L .Times( L.Transpose()), Is.EqualTo(D).Within(.001));

        //Checking verification of positive definiteness
        Assert.That(chol.IsSPD() , Is.EqualTo(true));


      Matrix sln = chol.Solve(Matrix.Parse(strB));
      Matrix eSln = Matrix.Parse(strExSln);
      Assert.That(sln, Is.EqualTo(eSln).Within(.0000000000001));

        }

        [Test]
        public void CholeskyDecompositionException_Test()
        {
            string strD = @"  2    -1     22
                             -1     2    -1
                              0    -1     2";

            string strBWrong = @"1
                                2
                                3
                                4";

            string strB = @"1
                            2
                            3";
            Matrix D = Matrix.Parse(strD); // D is not positive definite
            CholeskyDecomposition chol = new CholeskyDecomposition( D);

            Assert.Throws(typeof(ArgumentException), delegate { chol.Solve(Matrix.Parse(strBWrong)); });

            Assert.Throws(typeof( Exception), delegate { chol.Solve(Matrix.Parse(strB )); });

        }
    }
}
