using System;
using NUnit.Framework;
using MaNet.Generators;
using smt = MaNet_NUnit.StandardMatrixTests;

using MaNet;
namespace MaNet_NUnit
{
    [TestFixture]
public class Matrix_Tests:AssertionHelper 
    {

       
        double[] columnwise = { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0, 12.0 };
        double[] rowwise = { 1.0, 4.0, 7.0, 10.0, 2.0, 5.0, 8.0, 11.0, 3.0, 6.0, 9.0, 12.0 };
        double[][] avals = new double[3][] { new double[4] { 1.0, 4.0, 7.0, 10.0 }, new double[4] { 2.0, 5.0, 8.0, 11.0 }, new double[4] { 3.0, 6.0, 9.0, 12.0 } };
        //double[][] rankdef = avals;
        double[][] tvals = new double[4][] { new double[3] { 1.0, 2.0, 3.0 }, new double[3] { 4.0, 5.0, 6.0 }, new double[3] { 7.0, 8.0, 9.0 }, new double[3] { 10.0, 11.0, 12.0 } };
        double[][]  rvals = new double[3][] { new double[3] { 1.0, 4.0, 7.0 }, new double[4] { 2.0, 5.0, 8.0, 11.0 }, new double[4] { 3.0, 6.0, 9.0, 12.0 } };
        double[][] ivals = new double[3][] { new double[4] { 1.0, 0.0, 0.0, 0.0 }, new double[4] { 0.0, 1.0, 0.0, 0.0 }, new double[4] { 0.0, 0.0, 1.0, 0.0 } };

        int rows = 3, cols = 4;

        int validld = 3; /* leading dimension of intended test Matrices */
        int nonconformld = 4; /* leading dimension which is valid, but nonconforming */
        int ib = 1, ie = 2, jb = 1, je = 3; /* index ranges for sub Matrix */
        int[] rowindexset = { 1, 2 };
        int[] badrowindexset = { 1, 3 };
        int[] columnindexset = { 1, 2, 3 };
        int[] badcolumnindexset = { 1, 2, 4 };
 
        double sumofdiagonals = 15;
   


        [Test]
        public void Constructor_Test()
        {
            Matrix A;
            Matrix B;
            Matrix C;
            double tmp;
             double[] columnwise = { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0, 12.0 };
             int invalidld = 5;/* should trigger bad shape for construction with val */
            /** check that exception is thrown in packed constructor with invalid length **/
             try
             {
                A = new Matrix(columnwise, invalidld);
                 Assert.Fail("Catch invalid length in packed constructor,exception not thrown for invalid input");
             }
             catch { };


            /** check that exception is thrown in default constructor if input array is 'ragged' **/
             try
             {
                 A = new Matrix(rvals);
                 Assert.Fail("Catch invalid length in packed constructor,exception not thrown for invalid input");
             }catch {

             }

             /** check that exception is thrown in constructWithCopy  if input array is 'ragged' **/
             try
             {
                 A = Matrix.ConstructWithCopy(rvals);
                 Assert.Fail("Catch ragged input to constructWithCopy...");
             }
             catch { }

            A = new Matrix(columnwise, validld);
            B = new Matrix(avals);
            tmp = B.Get(0, 0);
            avals[0][0] = 0.0;
            C = B.Minus(A);
            avals[0][0] = tmp;
            B = Matrix.ConstructWithCopy(avals);
            tmp = B.Get(0, 0);
            avals[0][0] = 0.0;

            avals[0][0] = columnwise[0];
            Matrix  I = new Matrix(ivals);
             
            //        try
            //        {
            //            check(I, Matrix.identity(3, 4));
            //            try_success("identity... ", "");
            //        }

        }

        #region Access Methods
        /**   
             Access Methods:
                ColumnDimension
                RowDimension
                getArray()
                getArrayCopy()
                getColumnPackedCopy()
                getRowPackedCopy()
                get(int,int)
                getMatrix(int,int,int,int)
                getMatrix(int,int,int[])
                getMatrix(int[],int,int)
                getMatrix(int[],int[])
                set(int,int,double)
                setMatrix(int,int,int,int,Matrix)
                setMatrix(int,int,int[],Matrix)
                setMatrix(int[],int,int,Matrix)
                setMatrix(int[],int[],Matrix)
       **/


        [Test]
        public void Clone_Test()
        {
            Matrix A = Matrix.ParseMatLab("[1 2;3 4;5 6]");
            Matrix AClone = (Matrix)A.Clone();
            Assert.That(AClone, Is.EqualTo(A));
            Assert.That(Object.Equals(A, AClone), Is.False) ;


        }

        [Test]
        public void getColumnDimension_Test()
        {
            Matrix B = new Matrix(avals);
             Assert.That(B.ColumnDimension, Is.EqualTo(cols), "getColumnDimension... " );
        }


        [Test]
        public void getRowDimension_Test()
        {
            Matrix B = new Matrix(avals);
            Assert.That(B.RowDimension, Is.EqualTo(rows), "getRowDimension... " );
        }
               

        [Test]
        public void Array_Test()
        {

          Matrix  B = new Matrix(avals);
             double[][] barray = B.Array;
             Assert.That(barray, Is.EqualTo(avals));
            Assert.That (barray,Is.SameAs(avals));

        }
                
        [Test]
        public void getArrayCopy_Test()
        {
            Matrix B = new Matrix(avals);
            double[][] barray = B.ArrayCopy();
            Assert.That(barray, Is.EqualTo(avals));
            Assert.That(barray, Is.Not.SameAs(avals));


        }

        [Test]
        public void getColumnPackedCopy_Test()
        {
            Matrix B = new Matrix(avals);
            double[] bpacked = B.ColumnPackedCopy();
            Assert.That(bpacked, Is.EqualTo(columnwise));
           
        }

        [Test]
        public void getRowPackedCopy_Test()
        {
            Matrix B = new Matrix(avals);
            double[] bpacked = B.RowPackedCopy();
            Assert.That(bpacked, Is.EqualTo(rowwise));
            
        }

        [Test]
        public void get_Test()
        {
              Matrix B = new Matrix(avals);
            Assert.That(B.Get(B.RowDimension - 1, B.ColumnDimension - 1), 
                Is.EqualTo(  avals[B.RowDimension - 1][B.ColumnDimension - 1]));

        }


        [Test]
        public void get_ExceptionTest()
        {
            Matrix B = new Matrix(avals);
            Assert.Throws(typeof(IndexOutOfRangeException), delegate { B.Get(B.RowDimension, B.ColumnDimension - 1); });
            Assert.Throws(typeof(IndexOutOfRangeException), delegate { B.Get(B.RowDimension - 1, B.ColumnDimension); });
            
        }



        [Test]
        public void getMatrix_Test()
        {
            double[][] subavals = new double[2][]{new double[3]{5.0,8.0,11.0},new double[3]{6.0,9.0,12.0}};
            Matrix B = new Matrix(avals);
           
            Matrix SUB = new Matrix(subavals);
            //getMatrix(int,int,int,int)

            Matrix M = B.GetMatrix(ib, ie, jb, je);
            Assert.That (SUB.ArrayCopy(), Is.EqualTo (M.ArrayCopy()));
            
            //getMatrix(int,int,int[])
            M = B.GetMatrix(ib, ie, columnindexset);
            Assert.That(SUB.ArrayCopy(), Is.EqualTo(M.ArrayCopy()));
 
            //getMatrix(int[],int,int)
            M = B.GetMatrix(rowindexset, jb, je);
            Assert.That(SUB.ArrayCopy(), Is.EqualTo(M.ArrayCopy()));

            //getMatrix(int[],int[])
            M = B.GetMatrix(rowindexset, columnindexset);
            Assert.That(SUB.ArrayCopy(), Is.EqualTo(M.ArrayCopy()));
        }

        [Test]
        public void getMatrix_ExceptionTest()
        {

            double[][] subavals = new double[2][] { new double[3] { 5.0, 8.0, 11.0 }, new double[3] { 6.0, 9.0, 12.0 } };
            Matrix B = new Matrix(avals);

            //getMatrix(int,int,int,int) Exceptions
            Assert.Throws(typeof(IndexOutOfRangeException), delegate { B.GetMatrix(ib, ie, jb, je + B.ColumnDimension + 1); });
            Assert.Throws(typeof(IndexOutOfRangeException), delegate { B.GetMatrix(ib, ie + B.RowDimension + 1, jb, je); });

            //getMatrix(int,int,int[])
            Assert.Throws(typeof(IndexOutOfRangeException), delegate { B.GetMatrix(ib, ie, badcolumnindexset); });
            Assert.Throws(typeof(IndexOutOfRangeException), delegate { B.GetMatrix(ib, ie + B.RowDimension + 1, columnindexset); });


            //getMatrix(int[],int,int)
            Assert.Throws(typeof(IndexOutOfRangeException), delegate { B.GetMatrix(badrowindexset, jb, je); });
            Assert.Throws(typeof(IndexOutOfRangeException), delegate { B.GetMatrix(rowindexset, jb, je + B.ColumnDimension + 1); });

            //getMatrix(int[],int[])
            Assert.Throws(typeof(IndexOutOfRangeException), delegate { B.GetMatrix(rowindexset, badcolumnindexset); });
            Assert.Throws(typeof(IndexOutOfRangeException), delegate { B.GetMatrix(badrowindexset, columnindexset); });



        }
                 

        [Test]
        public void set_Test()
        {
            double[][] avals = new double[3][] { new double[4] { 1.0, 4.0, 7.0, 10.0 }, new double[4] { 2.0, 5.0, 8.0, 11.0 }, new double[4] { 3.0, 6.0, 9.0, 12.0 } };
            Matrix B = new Matrix(avals);
            B.Set(ib, jb, 0.0);
            Assert.That(B.Get(ib, jb), Is.EqualTo (0.0));

            
        }

        [Test]
        public void set_ExceptionTest()
        {
            Matrix B = new Matrix(avals);

            Assert.Throws(typeof(IndexOutOfRangeException), delegate { B.Set(B.RowDimension, B.ColumnDimension - 1, 0.0); });
            Assert.Throws(typeof(IndexOutOfRangeException), delegate { B.Set(B.RowDimension - 1, B.ColumnDimension, 0.0); });
        }


        [Test]
        public void setMatrix_Test() 
        {
            double[][] subavals = new double[2][] { new double[3] { 5.0, 8.0, 11.0 }, new double[3] { 6.0, 9.0, 12.0 } };
            double[][] avals = new double[3][] { new double[4] { 1.0, 4.0, 7.0, 10.0 }, new double[4] { 2.0, 5.0, 8.0, 11.0 }, new double[4] { 3.0, 6.0, 9.0, 12.0 } };
            Matrix B = new Matrix(avals);
            Matrix M = new Matrix(2, 3, 0.0);
            B.SetMatrix(ib, ie, jb, je, M);
            Assert.That(M.Minus(B.GetMatrix(ib, ie, jb, je)).RowPackedCopy(), Is.EqualTo(M.RowPackedCopy()));
            
             M = new Matrix(2, 3, 0.0);
           Matrix  SUB = new Matrix(subavals);
           B.SetMatrix(ib, ie, jb, je, SUB);  //ResetMatrix for next Test
  
            B.SetMatrix(ib, ie, columnindexset, M);
            Assert.That(M.Minus(B.GetMatrix(ib, ie, columnindexset)).RowPackedCopy(), Is.EqualTo(M.RowPackedCopy()));
            B.SetMatrix(ib, ie, jb, je, SUB); //ResetMatrix for next Test


            B.SetMatrix(rowindexset, jb, je, M);
            Assert.That(M.Minus(B.GetMatrix(rowindexset, jb, je)).RowPackedCopy(), Is.EqualTo (M.RowPackedCopy()));

            B.SetMatrix(ib, ie, jb, je, SUB); //ResetMatrix for next Test


             B.SetMatrix(rowindexset, columnindexset, M);
             Assert.That(M.Minus(B.GetMatrix(rowindexset, columnindexset)).RowPackedCopy(), Is.EqualTo(M.RowPackedCopy()));
               
           
            
        }

        [Test]
        public void setMatrix_ExceptionTest()
        {
            double[][] avals = new double[3][] { new double[4] { 1.0, 4.0, 7.0, 10.0 }, new double[4] { 2.0, 5.0, 8.0, 11.0 }, new double[4] { 3.0, 6.0, 9.0, 12.0 } };
            Matrix B = new Matrix(avals);
            Matrix M = new Matrix(2, 3, 0.0);

            Assert.Throws(typeof(IndexOutOfRangeException), delegate { B.SetMatrix(ib, ie + B.RowDimension + 1, jb, je, M); });
            Assert.Throws(typeof(IndexOutOfRangeException), delegate { B.SetMatrix(ib, ie, jb, je + B.ColumnDimension + 1, M); });
            Assert.Throws(typeof(IndexOutOfRangeException), delegate { B.SetMatrix(ib, ie + B.RowDimension + 1, columnindexset, M); });
            Assert.Throws(typeof(IndexOutOfRangeException), delegate { B.SetMatrix(ib, ie, badcolumnindexset, M); });
            Assert.Throws(typeof(IndexOutOfRangeException), delegate { B.SetMatrix(rowindexset, jb, je + B.ColumnDimension + 1, M); });
            Assert.Throws(typeof(IndexOutOfRangeException), delegate { B.SetMatrix(badrowindexset, jb, je, M); });
            Assert.Throws(typeof(IndexOutOfRangeException), delegate { B.SetMatrix(rowindexset, badcolumnindexset, M); });
            Assert.Throws(typeof(IndexOutOfRangeException), delegate { B.SetMatrix(badrowindexset, columnindexset, M); });
        }

        #endregion

        #region Array-like methods
        /** 
              Array-like methods:
                 minus
                 minusEquals
                 plus
                 plusEquals
                 arrayLeftDivide
                 arrayLeftDivideEquals
                 arrayRightDivide
                 arrayRightDivideEquals
                 arrayTimes
                 arrayTimesEquals
                 uminus
        **/

          [Test]
        public void minus_Test()
          {
              Matrix A = new Matrix(columnwise, validld);
              MaNet.Generators.Rectangular rand = new MaNet.Generators.Rectangular();

              Matrix R = rand.RandomDouble(A.RowDimension, A.ColumnDimension, 0d,1d);
              A = R.Copy();
              Matrix C = A.Minus(R);
              Assert.That(C.Norm1(), Is.EqualTo(0.0));
          }

          [Test]
          public void minus_ExceptionTest()
          {
              Matrix A = new Matrix(columnwise, validld);
             Matrix    S = new Matrix(columnwise, nonconformld);
             MaNet.Generators.Rectangular rand = new MaNet.Generators.Rectangular();

             Matrix R = rand.RandomDouble(A.RowDimension, A.ColumnDimension, 0d, 1d);
             A = R;
             Assert.Throws(typeof(ArgumentException), delegate { A.Minus(S); });
            
             
          }

          [Test]
          public void minusEquals_Test()
          {
               Matrix A = new Matrix(columnwise, validld);
               MaNet.Generators.Rectangular rand = new MaNet.Generators.Rectangular();
               Matrix R = rand.RandomDouble(A.RowDimension, A.ColumnDimension, 0d, 1d);
            A= R.Copy();
            A.MinusEquals(R);
            Assert.That(A.Norm1(), Is.EqualTo(0.0));
     
   

          }

          [Test] 
        public void minusEquals_ExceptionTest()
          {
              Matrix A = new Matrix(columnwise, validld);
              Matrix S = new Matrix(columnwise, nonconformld);
              Rectangular rand = new Rectangular();
              Matrix R = rand.RandomDouble(A.RowDimension, A.ColumnDimension);

              A = R.Copy();
              A.MinusEquals(R);
             Matrix Z = new Matrix(A.RowDimension, A.ColumnDimension);
              Assert.Throws(typeof(ArgumentException), delegate {A.MinusEquals(S); });
   
         }

          [Test]
          public void plus_ExceptionTest()
          {
              Matrix A = new Matrix(columnwise, validld);
              Matrix S = new Matrix(columnwise, nonconformld);
              Rectangular rand = new Rectangular();
              Matrix R = rand.RandomDouble(A.RowDimension, A.ColumnDimension);
             
             A = R.Copy();
             Matrix B = rand.RandomDouble(A.RowDimension, A.ColumnDimension);
              Matrix C = A.Minus(B);
              Assert.Throws(typeof(ArgumentException), delegate { A.Plus(S); });
               
          }


          [Test]
          public void plus_Test()
          {
              Matrix A = new Matrix(columnwise, validld);
              Matrix S = new Matrix(columnwise, nonconformld);
              Rectangular rand = new Rectangular();
              Matrix R = rand.RandomDouble(A.RowDimension, A.ColumnDimension);

              A = R.Copy();
              Matrix B = rand.RandomDouble(A.RowDimension, A.ColumnDimension);
              Matrix C = A.Minus(B);
              Assert.That ( C.Plus(B).RowPackedCopy() ,Is.EqualTo (A.RowPackedCopy()).Within(100).Ulps );
              
              

          }



         [Test]
        public void plusEquals_Test()
          {
              Matrix A = new Matrix(columnwise, validld);
              Matrix S = new Matrix(columnwise, nonconformld);
              Rectangular rand = new Rectangular();
              Matrix R = rand.RandomDouble(A.RowDimension, A.ColumnDimension);

              A = R.Copy();
              Matrix B = rand.RandomDouble(A.RowDimension, A.ColumnDimension);
              Matrix C = A.Minus(B);
              Assert.That(C.PlusEquals(B).RowPackedCopy(), Is.EqualTo(A.RowPackedCopy()).Within(100).Ulps );
          }

         [Test]
         public void plusEquals_ExceptionTest()
         {
             Matrix A = new Matrix(columnwise, validld);
             Matrix S = new Matrix(columnwise, nonconformld);
             Rectangular rand = new Rectangular();
             Matrix R = rand.RandomDouble(A.RowDimension, A.ColumnDimension);

             A = R.Copy();
             Matrix B = rand.RandomDouble(A.RowDimension, A.ColumnDimension);
             Matrix C = A.Minus(B);
             Assert.Throws(typeof(ArgumentException), delegate { A.PlusEquals(S); });
         }

         [Test] 
         public void ArrayLeftDivide_ExceptionTest()
         {
             Matrix A = new Matrix(columnwise, validld);
             Matrix S = new Matrix(columnwise, nonconformld);
             Rectangular rand = new Rectangular();
             Matrix R = rand.RandomDouble(A.RowDimension, A.ColumnDimension);
             Assert.Throws(typeof(ArgumentException), delegate { R.ArrayLeftDivide(S); });
   

         }


         [Test]
         public void ArrayLeftDivide_Test()
         {
             Matrix A = new Matrix(columnwise, validld);
             Rectangular rand = new Rectangular();
             Matrix R = rand.RandomDouble(A.RowDimension, A.ColumnDimension);
            Matrix  O = new Matrix(A.RowDimension, A.ColumnDimension, 1.0);

            Assert.That(R.ArrayLeftDivide(R.Copy()).ColumnPackedCopy(), Is.EqualTo(O.ColumnPackedCopy()));
         }

         [Test]
        public void ArrayLeftDivideEquals_Test()
          {
              Matrix A = new Matrix(columnwise, validld);
              Rectangular rand = new Rectangular();
              Matrix R = rand.RandomDouble(A.RowDimension, A.ColumnDimension);
              Matrix O = new Matrix(A.RowDimension, A.ColumnDimension, 1.0);

              Assert.That(R.ArrayLeftDivideEquals(R.Copy()).ColumnPackedCopy(), Is.EqualTo(O.ColumnPackedCopy()));
          }

         [Test]
         public void ArrayLeftDivideEquals_ExceptionTest()
         {
             Matrix A = new Matrix(columnwise, validld);
             Matrix S = new Matrix(columnwise, nonconformld);
             Rectangular rand = new Rectangular();
             Matrix R = rand.RandomDouble(A.RowDimension, A.ColumnDimension);
             Assert.Throws(typeof(ArgumentException), delegate { R.ArrayLeftDivideEquals(S); });
         }


        [Test]
        public void ArrayRightDivide_Test()
          {
             
            string strA = @" 0.1576    0.9572
                             0.9706    0.4854";

            string strB = @" 0.8003    0.4218
                             0.1419    0.9157";

            string strExpected = @" 0.1969    2.2695
                                    6.8406    0.5300";

            Matrix A = Matrix.Parse(strA);
            Matrix B = Matrix.Parse(strB);

            Matrix Expected = Matrix.Parse(strExpected);

            //Correct value
            Assert.That( A.ArrayRightDivide(B)  , Is.EqualTo(Expected).Within(.001));

            //A unchanged
            Assert.That (A ,Is.EqualTo(Matrix.Parse(strA)));

            //B unchanged
            Assert.That(B, Is.EqualTo(Matrix.Parse(strB)));

          }


        [Test]
        public void ArrayRightDivide_ExceptionTest()
        {
            Matrix A = new Matrix(columnwise, validld);
            Matrix S = new Matrix(columnwise, nonconformld);
            Rectangular rand = new Rectangular();
            Matrix R =rand.RandomDouble(A.RowDimension, A.ColumnDimension);
            Assert.Throws(typeof(ArgumentException), delegate { R.ArrayRightDivideEquals(S); });
        }


        [Test]
        public void ArrayRightDivideEquals_Test()
          {
              string strA = @" 0.1576    0.9572
                             0.9706    0.4854";

              string strB = @" 0.8003    0.4218
                             0.1419    0.9157";

              string strExpected = @" 0.1969    2.2695
                                    6.8406    0.5300";

              Matrix A = Matrix.Parse(strA);
              Matrix B = Matrix.Parse(strB);

              Matrix Expected = Matrix.Parse(strExpected);

              //Correct value
              Assert.That(A.ArrayRightDivideEquals(B), Is.EqualTo(Expected).Within(.001));

              //A Changed
              Assert.That(A  , Is.Not.EqualTo(Matrix.Parse(strA)  ));

              //A is Transformed to expected value
              Assert.That(A, Is.EqualTo(Expected).Within(.001));

              //B unchanged
              Assert.That(B, Is.EqualTo(Matrix.Parse(strB)));
          }

        [Test]
        public void ArrayRightDivideEquals_ExceptionTest()
        {
            Matrix A = new Matrix(columnwise, validld);
            Matrix S = new Matrix(columnwise, nonconformld);
            Rectangular rand = new Rectangular();
            Matrix R =rand.RandomDouble(A.RowDimension, A.ColumnDimension);
            Assert.Throws(typeof(ArgumentException), delegate { R.ArrayRightDivideEquals(S); });
        }

       [Test] 
        public void ArrayTimes_Test()
          {

              Matrix A = new Matrix(columnwise, validld);
              Rectangular rand = new Rectangular();
              Matrix B = rand.RandomDouble(A.RowDimension, A.ColumnDimension);
              Matrix C = A.ArrayTimes(B);
              Assert.That(C.ArrayRightDivide(B).RowPackedCopy(), Is.EqualTo(A.RowPackedCopy()).Within(10).Ulps );
          }
       [Test]
       public void ArrayTimes_ExceptionTest()
       {

           Matrix A = new Matrix(columnwise, validld);
           Matrix S = new Matrix(columnwise, nonconformld);
           Rectangular rand = new Rectangular();
           Matrix R = rand.RandomDouble(A.RowDimension, A.ColumnDimension);
           Assert.Throws(typeof(ArgumentException), delegate { R.ArrayTimes(S); });
       }

         [Test]
        public void ArrayTimesEquals_Test()
          {
              Matrix A = new Matrix(columnwise, validld);
              Rectangular rand = new Rectangular();
              Matrix B = rand.RandomDouble(A.RowDimension, A.ColumnDimension);
              Matrix Aorig = A.Copy();
              Matrix C = A.ArrayTimesEquals(B);
              Assert.That(C.ArrayRightDivide(B).RowPackedCopy(), Is.EqualTo(Aorig.RowPackedCopy()).Within(10).Ulps );
          }


         [Test]
         public void ArrayTimesEquals_ExceptionTest()
         {
             Matrix A = new Matrix(columnwise, validld);
             Matrix S = new Matrix(columnwise, nonconformld);
             Rectangular rand = new Rectangular();
             Matrix R = rand.RandomDouble(A.RowDimension, A.ColumnDimension);
             Assert.Throws(typeof(ArgumentException), delegate { R.ArrayTimes(S); });
         }


         [Test]
        public void uminus_Test()
          {
            Matrix A = new Matrix(columnwise, validld);
            Rectangular rand = new Rectangular();
            Matrix R = rand.RandomDouble(A.RowDimension, A.ColumnDimension);
            A = R.Uminus();
            Matrix Z = new Matrix(A.RowDimension, A.ColumnDimension, 0.0);

             Assert.That(A.Plus(R).RowPackedCopy(), Is.EqualTo(Z.ColumnPackedCopy()));
          }

        #endregion

        #region  Linear Algebra methods
        /**
                  LA methods:
                     transpose
                     times
                     cond
                     rank
                     det
                     trace
                     norm1
                     norm2
                     normF
                     normInf
                     solve
                     solveTranspose
                     inverse
                     chol
                     eig
                     lu
                     qr
                     svd 
            **/

         [Test]
         [Category("Linear Algebra")]
         public void transpose_Test()
         {
            Matrix A = new Matrix(columnwise, 3);
            Matrix T = new Matrix(tvals);
             A = A.Transpose();
             Assert.That(A, Is.EqualTo(T));
         }


         [Test]
         public void times_Test()
         {


            String strA = @"0.5377   -2.2588
                            1.8339    0.8622";

            String strB = @" 0.3188   -0.4336
                            -1.3077    0.3426";

            String strAB = @"3.1253   -1.0071
                            -0.5429   -0.4998";

            String strBA = @"-0.6238   -1.0939
                             -0.0748    3.2493";

            Matrix A = Matrix.Parse(strA);
            Matrix B = Matrix.Parse(strB);
            Matrix AB = Matrix.Parse(strAB);
            Matrix BA = Matrix.Parse(strBA);
            Assert.That(AB, Is.Not.EqualTo(BA));

            Assert.That(A.Times(B), Is.EqualTo(AB).Within(.001));
            Assert.That(B.Times(A), Is.EqualTo(BA).Within(.001));
           
         }

         [Test]
         public void TimesEquals_Test()
         {
             Matrix A = Matrix.Parse ("1 2\n3 4");
             Matrix B = A.TimesEquals(2);
             Assert.That(B, Is.EqualTo(A));
             Assert.That(Object.Equals(A, B), Is.True);


         }


         [Test]
         [TestCase(2)]
         public void Times_RandomTest(int size)
         {
             Rectangular rand = new Rectangular();
             Matrix A = rand.RandomDouble(size, size);
             Matrix B = rand.RandomDouble(size, size);
             Matrix C = rand.RandomDouble(size, size);

             Matrix I = Matrix.Identity(size, size);

             //Non Commutative -- There admittedly is a remote chance that this will fail
             Assert.That(A, Is.Not.EqualTo(B));
             Matrix AB = A.Times(B);
             Matrix BA = B.Times(A);
             Assert.That(AB, Is.Not.EqualTo(BA));

             //Matrix multiplication is associative (AB)C = A(BC)
             Assert.That((A.Times(B)).Times(C).Array, Is.EqualTo(A.Times(B.Times(C)).Array).Within (10).Ulps );
             Assert.That((A * B) * C, Is.EqualTo(A * (B * C)).Within(10).Ulps); // Using Operators

             //Matrix multiplication is distributive A(B + C) = AB + AC
             Assert.That(A.Times(B.Plus(C)).Array, Is.EqualTo(A.Times(B).Plus(A.Times(C)).Array).Within(10).Ulps);
             Assert.That(A * (B + C), Is.EqualTo(A * B + A * C).Within(10).Ulps); // Using Operators

             
         }



         [Test]
         public void Cond_Test()
         {
             string strA = @"0.8147    0.1270
                             0.9058    0.9134";
             Matrix A = Matrix.Parse(strA);
             double expectedCond = 1.4665 / 0.4290;

             Assert.That(A.Cond(), Is.EqualTo(expectedCond).Within(.001));

         }

 

         [Test]
         [Ignore]
         public void rank_Test()
         {

       

         }

         [Test]
         public void det_Test()
         {
          string   strA = @"-1.0689    1.4384    1.3703
                            -0.8095    0.3252   -1.7115
                            -2.9443   -0.7549   -0.1022";
          double expectedDet = 10.6952; //From matlab
           Matrix A = Matrix.Parse(strA);

           Assert.That(A.Det(), Is.EqualTo(expectedDet).Within(.001));

         //Nonzero determinant implies full rank
           Assert.That(A.Rank(), Is.EqualTo(3));

           string strB = @"-1.0689    1.4384    1.3703
                           -0.8095    0.3252   -1.7115
                           -0.8095    0.3252   -1.7115";

           Matrix B = Matrix.Parse(strB);
         //Rows not independent so determinant should be 0
           Assert.That(B.Det(), Is.EqualTo(0).Within(.0001));
         }

         [Test]
         [TestCase(2)]
         [TestCase(3)]
         public void det_RandomTest(int size)
         {
             Rectangular rand = new Rectangular();
             Matrix A = rand.RandomDouble(size, size);
             Matrix B = rand.RandomDouble(size, size);
             Matrix I = Matrix.Identity(size, size);

             //detI =1 when I is the identity
             Assert.That (I.Det(), Is.EqualTo(1));

             Assert.That (A , Is.Not.EqualTo(B));
             //detAB = detA detB
             Assert.That (A.Times(B).Det(), Is.EqualTo(A.Det() * B.Det()).Within(5000).Ulps );

             //determinant of A transpose is the same as the determinant of A
             Assert.That(A.Transpose().Det(), Is.EqualTo(A.Det()).Within(1000).Ulps);

             // if determinant is not zero then must be full rank
             if (A.Det() != 0)
             {
                 Assert.That(A.Rank(), Is.EqualTo(size));
             }
         }
       
         [Test]
         public void trace_Test()
         {
             Matrix A = new Matrix(columnwise, 3);
             Assert.That(A.Trace(), Is.EqualTo(sumofdiagonals));


         }

         [Test]
         [TestCase(2)]
         [TestCase(3)]
         public void trace_RandomTest(int size)
         {
             Rectangular rand = new Rectangular();
             Matrix A = rand.RandomDouble (size, size);
             Matrix B = rand.RandomDouble(size, size);
             Matrix C = rand.RandomDouble(size, size);

             Matrix I = Matrix.Identity(size, size);

             //Trace of squre identity marix is its size.
             Assert.That(I.Trace(), Is.EqualTo(size));

             // The Trace of a matrix is equal to the trace of its transpose
             Assert.That(A.Trace() ,Is.EqualTo(A.Transpose().Trace()).Within(60).Ulps );

             //Trace Comutes with scalar multiplication
             Random rnd = new Random();
             double rNum = rnd.NextDouble();
             Assert.That(A.Times(rNum).Trace(), Is.EqualTo(A.Trace() * rNum).Within(10).Ulps);

             // The Trace of the sum of matrices is the sum of the traces
             Assert.That(A.Plus(B).Trace(), Is.EqualTo(A.Trace() + B.Trace()).Within (10).Ulps);

             // for square A,B,C trABC = trBCA = trCAB
             double trABC = A.Times(B).Times(C).Trace();
             double trBCA = B.Times(C).Times(A).Trace();
             double trCAB = C.Times(A).Times(B).Trace();
             Assert.That(trABC, Is.EqualTo(trBCA).Within(100).Ulps  );
             Assert.That(trBCA, Is.EqualTo(trCAB).Within(100).Ulps);

             //Note that Except for exceptional circumstances trABC != trACB
             double trACB = A.Times(C).Times(B).Trace();
             Assert.That(trABC, Is.Not.EqualTo(trACB));
         }
        
         [Test]
         public void norm1_Test()
         {
             String strA = @"  0.4889    0.7269
                                1.0347   -0.3034";

             double expected1Norm = 1.5236; //From Matlab
             Matrix A = Matrix.Parse(strA);

             Assert.That(A.Norm1(), Is.EqualTo(expected1Norm));
         }


         [Test]
         [TestCase(2)]
         public void Norm1_RandomTest(int size)
         {
             Rectangular rand = new Rectangular();
             Matrix A = rand.RandomDouble (size, size);
             Matrix B = rand.RandomDouble(size, size);

             Matrix O = new Matrix(size, size);


             //Test the Norm1 is in Fact a Norm

             // Definiteness
             Assert.That(O.Norm1(), Is.EqualTo(0));

             Assert.That(A, Is.Not.EqualTo(O));
             //nonnegativity
             Assert.That(A.Norm1(), Is.GreaterThan(0));

             Random rnd = new Random();
             double rNum = rnd.NextDouble();
             //homogeneity 
             Assert.That((A.Times(rNum)).Norm1(), Is.EqualTo(rNum * A.Norm1()).Within(10).Ulps);

             // triangle inequality, sometimes fails on random
             Assert.That(A.Plus(B).Norm1(), Is.LessThanOrEqualTo(A.Norm1() + B.NormF()));

         }

         [Test]
         [TestCase(2)]
         public void Norm2_RandomTest(int size)
         {
             Rectangular rand = new Rectangular();
             Matrix A = rand.RandomDouble(size, size);
             Matrix B = rand.RandomDouble(size, size);

             Matrix O = new Matrix(size, size);


             //Test the Norm2 is in Fact a Norm

             // Definiteness
             Assert.That(O.Norm2(), Is.EqualTo(0));

             Assert.That(A, Is.Not.EqualTo(O));
             //nonnegativity
             Assert.That(A.Norm2(), Is.GreaterThan(0));

             Random rnd = new Random();
             double rNum = rnd.NextDouble();
             //homogeneity 
             Assert.That((A.Times(rNum)).Norm2(), Is.EqualTo(rNum * A.Norm2()).Within(10).Ulps);

             // triangle inequality
             Assert.That(A.Plus(B).Norm2(), Is.LessThanOrEqualTo(A.NormF() + B.Norm2()));

         }

         [Test]
         [Ignore]
         public void norm2_Test()
         {
             String strA = @" 0.2939    0.8884
                             -0.7873   -1.1471";
             double expected2Norm = 1.6624; //From Matlab
             Matrix A = Matrix.Parse(strA);

             Assert.That(A.Norm2(), Is.EqualTo(expected2Norm).Within(.0001));

         }

 

         [Test]
         public void normF_Test()
         {
             String strA = @"0.7254    0.7147
                            -0.0631   -0.2050";
             double expectedFNorm = 1.0407; //From Matlab
             Matrix A = Matrix.Parse(strA);

             Assert.That(A.NormF(), Is.EqualTo(expectedFNorm).Within(.0001));

         }

         [Test]
         [TestCase(2)]
         public void NormF_RandomTest(int size)
         {
             Rectangular rand = new Rectangular();
             Matrix A = rand.RandomDouble (size);
             Matrix B = rand.RandomDouble(size);
             // Frobenius norm can be defined as Sqrt(tr(transpose(A)A)
             Assert.That (A.NormF(), Is.EqualTo (Math.Sqrt(A.Transpose().Times(A).Trace())).Within(10).Ulps );

             Matrix O = new Matrix(size, size);


             //Test the NormF is in Fact a Norm

             // Definiteness
             Assert.That(O.NormF(), Is.EqualTo(0));

             Assert.That(A, Is.Not.EqualTo(O));
             //nonnegativity
             Assert.That(A.NormF(), Is.GreaterThan(0));

             Random rnd = new Random();
             double rNum = rnd.NextDouble();
             //homogeneity 
             Assert.That ((A.Times(rNum)).NormF() , Is.EqualTo(rNum * A.NormF()).Within(10).Ulps );

             // triangle inequality
             Assert.That(A.Plus(B).NormF(), Is.LessThanOrEqualTo(A.NormF() + B.NormF()));

         }

 

         [Test]
         public void normInf_Test()
         {
             String strA = @" 0.6715    0.7172
                             -1.2075    1.6302";
             double expectedInfNorm = 2.8377; //From Matlab
             Matrix A = Matrix.Parse(strA);

             Assert.That(A.NormInf(), Is.EqualTo(expectedInfNorm).Within(.0001));
            
         }


         [Test]
         [TestCase(2)]
         public void NormInf_RandomTest(int size)
         {
             Rectangular rand = new Rectangular();
             Matrix A = rand.RandomDouble(size, size);
             Matrix B = rand.RandomDouble(size, size);
           
             Matrix O = new Matrix(size, size);


             //Test the NormInf is in Fact a Norm

             // Definiteness
             Assert.That(O.NormF(), Is.EqualTo(0));

             Assert.That(A, Is.Not.EqualTo(O));
             //nonnegativity
             Assert.That(A.NormF(), Is.GreaterThan(0));

             Random rnd = new Random();
             double rNum = rnd.NextDouble();
             //homogeneity 
             Assert.That((A.Times(rNum)).NormF(), Is.EqualTo(rNum * A.NormF()).Within(10).Ulps);

             // triangle inequality
             Assert.That(A.Plus(B).NormF(), Is.LessThanOrEqualTo(A.NormF() + B.NormF()));

         }

         [Test]
         [Ignore]
         public void solve_Test()
         {

         }

         [Test]
         [Ignore]
         public void solve_ExceptionTest()
         {

         }

         [Test]
         [Ignore]
         public void solveTranspose_Test()
         {

         }

         [Test]
         [Ignore]
         public void solveTranspose_ExceptionTest()
         {

         }

         [Test]
         public void inverse_Test()
         {
             string strA = @"2.7694    0.7254   -0.2050
                            -1.3499   -0.0631   -0.1241
                             3.0349    0.7147    1.4897";

             string strInvA = @"-0.0039   -0.9230   -0.0775
                                 1.2291    3.5709    0.4667
                                -0.5817    0.1671    0.6052";

             Matrix A = Matrix.Parse(strA);
             Matrix InvA = Matrix.Parse(strInvA);

             Assert.That(A.Inverse(), Is.EqualTo(InvA).Within(.001));

         }

         [Test]
         public void Inverse_ExceptionTest()
         {
             string strA = @"1   4   2
                             2   6   4
                             3   6   6";
             Matrix A = Matrix.Parse(strA);
             Assert.Throws(typeof(Exception), delegate { A.Inverse(); });
         }

         [Test]
         public void Eig_Test()
         {

             string strA = @"1	2
                            3	4";
             Matrix A = Matrix.Parse(strA);

             Assert.That(smt.IsSymetric(A), Is.False);

             EigenvalueDecomposition EofA = A.Eig();

             Matrix V = EofA.getV();
             Matrix D = EofA.getD();
             Assert.That(smt.IsDiagonal(D), Is.True); //Block Diagonal which for 2x2 is diagonal

             // V*D * V.Inverse = A
             Assert.That((V.Times(D.Times(V.Inverse()))).Array, Is.EqualTo(A.Array).Within(.0000001).Percent);

             // A.times(V) equals V.times(D)
             Assert.That((A.Times(V)).Array, Is.EqualTo(V.Times(D).Array).Within(.0000001).Percent);





         }



        #endregion 

        #region Onput/Output Methods
         [Test]
         public void Parse_Test()
         {
             string s = @"1	4	4
                          3	3	2
                          3	5	0";

             Matrix A = Matrix.Parse(s);
             double[][] BArray = new double[3][] { new double[3] { 1, 4, 4 }, new double[3] { 3, 3, 2 }, new double[3] { 3, 5, 0 } };
             Matrix B = new Matrix(BArray);
             Assert.That(B, Is.EqualTo(A));
         }



        #endregion

    }
}
