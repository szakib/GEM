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
 public class Parser_Tests
    {
        [Test]
        public void SubstituteTest()
        {
            string strAbase = @"1  x
                                x  3";


            string strA = @"1  2
                            2  3";

            Assert.That( Parser.Substitute(strAbase , "x" , 2), Is.EqualTo(Matrix.Parse(strA)));
        }
    }
}
