using NUnit.Framework;
using MaNet;
using smt = MaNet_NUnit.StandardMatrixTests;
using MaNet.Generators;
namespace MaNet_NUnit
{
    [TestFixture]
 public   class SingularValueDecomposition_Tests
    {

       // A = U*S*V'
        [Test]
        public void TwoDimensionalSquare()
        {
            string strA = @"1	2
                            3	4";
            Matrix A = Matrix.Parse(strA);
            SingularValueDecomposition SVofA = new SingularValueDecomposition(A);

            Matrix U = SVofA.getU();
            Assert.That(U.RowDimension, Is.EqualTo(A.RowDimension));    //U should be a square matrix
            Assert.That(U.ColumnDimension, Is.EqualTo(A.RowDimension)); // of dimension RowDimension
            // U should be unitary which for real matricies means that U times U transpose is the identity.
            Assert.That (U.Times(U.Transpose()), Is.EqualTo(Matrix.Identity(U.RowDimension, U.ColumnDimension)));
            
            Matrix S = SVofA.getS();
            Assert.That(S.RowDimension, Is.EqualTo(A.RowDimension));         //S should have the same 
            Assert.That(S.ColumnDimension, Is.EqualTo(A.ColumnDimension));   // dimensions as A.
            Assert.That(smt.IsNonnegativeDiagonal(S), Is.True); // S should be diagonal and nonmegative
            
            Matrix V = SVofA.getV();
            Assert.That(V.RowDimension, Is.EqualTo(A.ColumnDimension));      //U should be a square matrix
            Assert.That(V.ColumnDimension, Is.EqualTo(A.ColumnDimension));   // of dimension ColumnDimension
            // V should be unitary which for real matricies means that V times V transpose is the identity.
            Assert.That(V.Times(V.Transpose()), Is.EqualTo(Matrix.Identity(V.RowDimension, V.ColumnDimension)));

            // A = U*S*V'
            Assert.That(U.Times(S.Times(V.Transpose())), Is.EqualTo(A).Within(.00000001));
            Assert.That(SVofA.Rank(), Is.EqualTo(2));

        }

        [Test]
        public void ThreeByTwo_Test()
        {

        }


        [TestCase(3, 3)]
        [TestCase(5, 4)]
        //[TestCase(100, 75)]
        public void RandomMatrixTest(int m, int n)
        {
             
            Rectangular rand = new Rectangular();
            Matrix A = rand.RandomDouble(m,n);

            SingularValueDecomposition SVofA = new SingularValueDecomposition(A);

            Matrix U = SVofA.getU();
            Assert.That(U.RowDimension, Is.EqualTo(m));    
            Assert.That(U.ColumnDimension, Is.EqualTo(n));  
            // U should be unitary which for real matricies means that  U transpose times U is the identity.
            Assert.That(U.Transpose().Times(U).Array , Is.EqualTo(Matrix.Identity(n, n).Array ).Within(.000000001));

            Matrix S = SVofA.getS();
            Assert.That(S.RowDimension, Is.EqualTo(n));         
            Assert.That(S.ColumnDimension, Is.EqualTo(n));      
            Assert.That(smt.IsNonnegativeDiagonal(S), Is.True); // S should be diagonal and nonmegative

            Matrix V = SVofA.getV();
            Assert.That(V.RowDimension, Is.EqualTo(n));      
            Assert.That(V.ColumnDimension, Is.EqualTo(n));    
            // U should be unitary which for real matricies means that  V transpose times V is the identity.
            Assert.That(V.Transpose().Times(V).Array, Is.EqualTo(Matrix.Identity(n, n).Array).Within(.000000001));

            // A = U*S*V
            // The percent is used as the elements in the matrix are rahter large 
            Assert.That(U.Times(S.Times(V.Transpose())).Array, Is.EqualTo(A.Array).Within(.00000001).Percent);

        }

       
    }
}
