using System;
using NUnit.Framework;
using MaNet;
namespace MaNet_NUnit
{
    [TestFixture]
  public  class Matrix_Tests2
    {
    [Test]
     public void Solve3by3()
    {
        // For equations
        // 2x + y + z  = 5
        // 4x -6y      = 2
        //-2x + 7y + 2 = 9
        // with solution x = 1, y = 1, z = 2

        String strMat = @"2  1  1
                          4 -6  0
                         -2  7  2";

        String strVals = @"5
                          -2
                           9";

        String strExpectedSoln = @"1
                                   1
                                   2";

        Matrix mat = Matrix.Parse(strMat);
        Matrix vals = Matrix.Parse(strVals);

        Matrix expectedSoln = Matrix.Parse(strExpectedSoln);

        Matrix soln = mat.Solve(vals);
       //Checks that solution solves matrix equation. 
       //Note that I can do this even if I don't know the solution.
        Assert.That(mat.Times(soln), Is.EqualTo(vals)); 

        //Check against expected solution
        Assert.That (soln ,Is.EqualTo(expectedSoln));
    }


    [Test]
    public void LeastSquares4by2()
    {
        // For equations
        // a +  b =  6
        // a + 2b =  5
        // a + 3b =  7
        // a + 4b = 10
        // with leastsqure approximate solution a=3.5 , b=1.4

        String strMat = @"1  1  
                          1  2   
                          1  3  
                          1  4";

        String strVals = @"6
                           5
                           7
                          10";

        String strExpectedSoln = @"3.5
                                   1.4";


        Matrix mat = Matrix.Parse(strMat);
        Matrix vals = Matrix.Parse(strVals);

        Matrix expectedSoln = Matrix.Parse(strExpectedSoln);


        Matrix soln = mat.Solve(vals);

        //Check against expected solution
        Assert.That(soln, Is.EqualTo(expectedSoln).Within(.001));
    }

    }
}
