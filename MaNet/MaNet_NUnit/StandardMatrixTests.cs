using System;
using MaNet;

namespace MaNet_NUnit
{
  public static  class StandardMatrixTests
    {
        public static bool IsUpperTriangular(Matrix m)
        {
            for (int iRow = 0; iRow < m.RowDimension; iRow++)
            {
                for (int iCol = 0; iCol < m.ColumnDimension; iCol++)
                {
                    if (iRow > iCol && m.Get(iRow, iCol) != 0) return false;
                }
            }
            return true;
        }


        public static bool IsLowerTriangular(Matrix m)
        {
            for (int iRow = 0; iRow < m.RowDimension; iRow++)
            {
                for (int iCol = 0; iCol < m.ColumnDimension; iCol++)
                {
                    if (iRow < iCol && m.Get(iRow, iCol) != 0) return false;
                }
            }
            return true;
        }

        public static bool IsDiagonal(Matrix m)
        {
            for (int iRow = 0; iRow < m.RowDimension; iRow++)
            {
                for (int iCol = 0; iCol < m.ColumnDimension; iCol++)
                {
                    if (iRow != iCol && m.Get(iRow, iCol) != 0) return false;
                }
            }
            return true;
        }


        public static bool IsSymetric(Matrix m)
        {
            if (m.RowDimension != m.ColumnDimension) return false;
            int n = m.ColumnDimension;
          bool  issymmetric = true;
            for (int j = 0; (j < n) & issymmetric; j++)
            {
                for (int i = 0; (i < n) & issymmetric; i++)
                {
                    issymmetric = (m.Get(i, j) == m.Get(j, i));
                }
            }
            return issymmetric;
        }


        public static  bool IsNonnegativeDiagonal(Matrix mat)
        {
            for (int i = 0; i < mat.RowDimension; i++)
            {
                for (int j = 0; j < mat.ColumnDimension; j++)
                {
                    if (i == j)
                    {
                        if (mat.Get(i, j) < 0) return false;
                    }
                    else
                    {
                       
                        if (mat.Get(i, j) != 0) return false;

                    }

                }
            }
            return true;

        }


        public static bool IsIntegerValued(Matrix mat)
        {
            for (int i = 0; i < mat.RowDimension; i++)
            {
                for (int j = 0; j < mat.ColumnDimension; j++)
                {
                    if (Math.Truncate(mat.Get(i, j)) != mat.Get(i, j)) return false;
                }
            }
            return true;

        }

    }
}
