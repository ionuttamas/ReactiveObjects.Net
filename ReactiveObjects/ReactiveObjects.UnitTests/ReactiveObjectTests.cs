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

            Expression<Func<int>> expression = () => a.Value.Length * 2 + 4;

            R<int> firstOperand = 2;
            R<int> secondOperand = 3;

            R<int> sum = R.Of(() => a.Value.Length * 2 + 4);
            R<int> sumX = R.Of(() => firstOperand + secondOperand);
            Assert.Equals(5, sum);

            firstOperand = 4;
            Assert.Equals(7, sum);
        }
    }
}
