using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;

namespace ReactiveObjects.UnitTests {
    [TestFixture]
    public class ReactiveObjectTests {
        [Test]
        public void ReactiveObject_ForSimpleArithmeticExpression()
        {
            R<string> a = "4";
            R<int> da = 4;


            R<int> sum = R.Of(() => a.Value.Length * 2 + 4);
            Assert.AreEqual(6, sum.Value);
            a.Set("43");
            Assert.AreEqual(8, sum.Value);

            R<int> firstOperand = 2;
            R<int> secondOperand = 3;
            R<int> sumX = R.Of(() => firstOperand + secondOperand);
            Assert.AreEqual(5, sumX.Value);

            firstOperand.Set(4);
            Assert.AreEqual(7, sumX.Value);
        }
    }
}
