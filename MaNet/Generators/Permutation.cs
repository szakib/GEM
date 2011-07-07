using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaNet.Generators
{
  public class Permutation
    {
      /// <summary>
      /// Creates A matrix to swap the appropriate rows or columns
      /// such that S * A swaps Rows and A * S swaps columns
      /// </summary>
      /// <param name="dimension"></param>
      /// <param name="firstRowOrColumn"></param>
      /// <param name="secondRowOrColumn"></param>
      /// <returns>Swao Matrix</returns>
      public static Matrix SwapMatrix(int dimension, int firstRowOrColumn, int secondRowOrColumn)
      {
          Matrix M = new Matrix(dimension);
          for (int i = 0; i < dimension; i++)
          {
              if (i == firstRowOrColumn)
              {
                  M.Array[i][secondRowOrColumn] = 1;

              }
              else if (i == secondRowOrColumn)
              {

                  M.Array[i][firstRowOrColumn] = 1;

              }
              else
              {
                  M.Array[i][i] = 1;
              }


          }

          return M; 

      }
    }
}
