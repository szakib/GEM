using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaNet.Generators
{
  public  class Rectangular
    {

        public Random Random
        {
            get { return rand; }
            set { rand = value; }


        }

        protected Random rand = new Random();

        public Matrix RandomDouble(int m)
        {
            return RandomDouble(m, m, 0, 1);

        }

        public Matrix RandomDouble(int m, int n)
        {
            return RandomDouble(m, n, 0, 1);

        }


        public Matrix RandomDouble(int m, int n, double min, double max)
        {
            Matrix A = new Matrix(m, n);
            double[][] X = A.Array;
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    X[i][j] = min + rand.NextDouble() * (max - min);
                }
            }
            return A;
        }


        public Matrix RandomInt(int m, int n, int min, int max)
        {
            Matrix A = new Matrix(m, n);
            double[][] X = A.Array;
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    X[i][j] = rand.Next(min, max);
                }
            }
            return A;

        }

        public Matrix RandomInt(int m, int n)
        {
            return RandomInt(m, n, 0, 9);
        }

        public Matrix RandomInt(int m )
        {
            return RandomInt(m, m, 0, 9);
        }
    }
}
