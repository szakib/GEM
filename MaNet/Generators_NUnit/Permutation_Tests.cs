using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MaNet;
namespace Generators_NUnit
{[TestFixture]
  public  class Permutation_Tests
    {

    [Test]
    public void SwapMatrix_Test()
    {
        string strA = @" 1     2     3
                         4     5     6
                         7     8     9";

        Matrix A = Matrix.Parse(strA);


        string strExpectedS = @" 1     0     0
                                 0     0     1
                                 0     1     0";


        Matrix ExpectedS = Matrix.Parse(strExpectedS);

        Matrix S = MaNet.Generators.Permutation.SwapMatrix(3, 1, 2);
        Assert.That(S, Is.EqualTo(ExpectedS));

        string strExpectedRowSwaped = @"1     2     3
                                        7     8     9
                                        4     5     6";
        Matrix ExpectedRowSwaped = Matrix.Parse(strExpectedRowSwaped);

        Assert.That(S * A, Is.EqualTo(ExpectedRowSwaped));


        string strExpectedColumnSwaped = @"1     3     2
                                           4     6     5
                                           7     9     8";
        Matrix ExpectedColumnSwaped = Matrix.Parse(strExpectedColumnSwaped);

        Assert.That(A * S, Is.EqualTo(ExpectedColumnSwaped ));

    }


    }
}
