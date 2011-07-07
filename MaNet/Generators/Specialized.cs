using System;

namespace MaNet.Generators
{
 public class Specialized
    {

     public static Matrix Toeplitz(double[] firstRow){
         Double[][] array = new Double[firstRow.Length][];
         array[0] = firstRow;
         for (int i = 1; i < firstRow.Length; i++)
         {
             Double[] row = new Double[firstRow.Length];
             for (int j = 0; j < row.Length; j++)
             {
                 row[j] = firstRow[Math.Abs(j - i)];
             }
             array[i] = row;
         }
         Matrix A = new Matrix(array);
         return A;

     }

     public static Matrix Stiffness( int dimension )
     {
         if (dimension < 2) throw new Exception("Matrix only defined for dimension 2 and above");
         Double[] array = new Double[dimension];
         array[0] = 2;
         array[1] = -1;
         return Toeplitz(array);

     }

     public static Matrix K( int dimension)
     {
         return Stiffness(dimension);

     }

     public static Matrix Circulant(int dimension)
     {
         if (dimension < 2) throw new Exception("Matrix only defined for dimension 3 and above");
         Matrix A = K(dimension);
         A.Array[0][dimension - 1] = -1;
         A.Array[dimension - 1][0] = -1;
         return A;
     }


     public static Matrix C(int dimension)
     {
         return Circulant(dimension);
     }


     public static Matrix T(int dimension)
     {
         if (dimension < 2) throw new Exception("Matrix only defined for dimension 2 and above");
         Double[] array = new Double[dimension];
         array[0] = 2;
         array[1] = -1;
         Matrix A =Toeplitz(array);
         A.Array[0][0] = 1;
         return A;

     }

     public static Matrix B(int dimension)
     {
         if (dimension < 2) throw new Exception("Matrix only defined for dimension 2 and above");
         Double[] array = new Double[dimension];
         array[0] = 2;
         array[1] = -1;
         Matrix A = Toeplitz(array);
         A.Array[0][0] = 1;
         A.Array[dimension - 1][dimension - 1] = 1;
         return A;

     }



     /// <summary>
     /// Returns the Rosser matrix, a famous 8 by eight matrix which many algorithms have trouble with.
     /// </summary>
     /// <returns>Rosser Matrix</returns>
     public static Matrix Rosser(){

         string strRosser = @" 611   196  -192   407    -8   -52   -49    29
                               196   899   113  -192   -71   -43    -8   -44
                              -192   113   899   196    61    49     8    52
                               407  -192   196   611     8    44    59   -23
                                -8   -71    61     8   411  -599   208   208
                               -52   -43    49    44  -599   411   208   208
                               -49    -8     8    59   208   208    99  -911
                                29   -44    52   -23   208   208  -911    99";

         return Matrix.Parse(strRosser);

     }


    }
}
