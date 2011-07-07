using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaNet.Generators
{
  public  class Parser
    {

      public static Matrix Substitute(string baseMatrix, string stringToSubtitute, double substitutionValue)
      {
          string working = baseMatrix.Replace(stringToSubtitute, substitutionValue.ToString("R"));
          return Matrix.Parse(working);

      }
    }
}
