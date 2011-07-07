using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MaNet;
using MaNet.Generators;
namespace Generators_NUnit
{
    [TestFixture]
  public  class Specialized_Tests
    {
        [Test]
        public void Toeplitz_Test()
        {

            string strExpected = @"1     3     9
                                   3     1     3
                                   9     3     1";

            Matrix E = Matrix.Parse(strExpected);

            Assert.That(Specialized.Toeplitz(new double[]{1,3,9}), Is.EqualTo(E));

        }

        [Test]
        public void KStiffnes_Test()
        {

            string strK2 = @" 2 -1
                             -1  2";
            Matrix K2 = Matrix.Parse(strK2);
            Assert.That(Specialized.K(2), Is.EqualTo(K2));
            Assert.That(Specialized.Stiffness(2), Is.EqualTo(K2));

            string strK3 = @" 2 -1  0
                             -1  2 -1
                              0 -1  2";
            Matrix K3 = Matrix.Parse(strK3);
            Assert.That(Specialized.K(3), Is.EqualTo(K3));
            Assert.That(Specialized.Stiffness(3), Is.EqualTo(K3));


            string strK4 = @" 2 -1  0  0
                             -1  2 -1  0
                              0 -1  2 -1 
                              0  0 -1  2";
            Matrix K4 = Matrix.Parse(strK4);
            Assert.That(Specialized.K(4), Is.EqualTo(K4));
            Assert.That(Specialized.Stiffness(4), Is.EqualTo(K4));
        }

        [Test]
        public void CCirculant_Test()
        {
 

            string strC3 = @" 2  -1  -1
                             -1   2  -1
                             -1  -1   2";
            Matrix C3 = Matrix.Parse(strC3);
            Assert.That(Specialized.C(3), Is.EqualTo(C3));
            Assert.That(Specialized.Circulant(3), Is.EqualTo(C3));


            string strC4 = @" 2  -1   0  -1
                             -1   2  -1   0
                              0  -1   2  -1 
                             -1   0  -1   2";
            Matrix C4 = Matrix.Parse(strC4);
            Assert.That(Specialized.C(4), Is.EqualTo(C4));
            Assert.That(Specialized.Circulant(4), Is.EqualTo(C4));
        }

        [Test]
        public void T_Test()
        {

            string strT2 = @" 1 -1
                             -1  2";
            Matrix T2 = Matrix.Parse(strT2);
            Assert.That(Specialized.T(2), Is.EqualTo(T2));
             
            string strT3 = @" 1 -1  0
                             -1  2 -1
                              0 -1  2";
            Matrix T3 = Matrix.Parse(strT3);
            Assert.That(Specialized.T(3), Is.EqualTo(T3));
           
            string strT4 = @" 1 -1  0  0
                             -1  2 -1  0
                              0 -1  2 -1 
                              0  0 -1  2";
            Matrix T4 = Matrix.Parse(strT4);
            Assert.That(Specialized.T(4), Is.EqualTo(T4));
        }

        [Test]
        public void B_Test()
        {

            string strB2 = @" 1 -1
                             -1  1";
            Matrix B2 = Matrix.Parse(strB2);
            Assert.That(Specialized.B(2), Is.EqualTo(B2));
         

            string strB3 = @" 1 -1  0
                             -1  2 -1
                              0 -1  1";
            Matrix K3 = Matrix.Parse(strB3);
            Assert.That(Specialized.B(3), Is.EqualTo(K3));
          


            string strB4 = @" 1 -1  0  0
                             -1  2 -1  0
                              0 -1  2 -1 
                              0  0 -1  1";
            Matrix B4 = Matrix.Parse(strB4);
            Assert.That(Specialized.B(4), Is.EqualTo(B4));
           
        }

        [Test]
        public void RosserTest()
        {

            string strRosser = @"  611   196  -192   407    -8   -52   -49    29
                                   196   899   113  -192   -71   -43    -8   -44
                                  -192   113   899   196    61    49     8    52
                                   407  -192   196   611     8    44    59   -23
                                    -8   -71    61     8   411  -599   208   208
                                   -52   -43    49    44  -599   411   208   208
                                   -49    -8     8    59   208   208    99  -911
                                    29   -44    52   -23   208   208  -911    99";

             Matrix  R = Matrix.Parse(strRosser);
             Assert.That(Specialized.Rosser(),Is.EqualTo( R));


        }


    }
}
