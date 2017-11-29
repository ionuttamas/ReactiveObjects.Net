using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;

namespace ReactiveObjects.UnitTests {
    [TestFixture]
    public class ReactiveObjectTests {
        [TestCase(2, 3)]
        [TestCase(12, 3)]
        [TestCase(2, 43)]
        [TestCase(12, 13)]
        public void ReactiveObject_ForAddOperation_ComputesCorrectly(int leftValue, int rightValue) {
            R<int> leftOperand = leftValue;
            R<int> rightOperand = rightValue;

            R<int> result = R.Of(() => leftOperand + rightOperand);
            Assert.AreEqual(leftValue + rightValue, result.Value);

            leftValue = 2*leftValue;
            leftOperand.Set(leftValue);
            Assert.AreEqual(leftValue + rightValue, result.Value);

            rightValue = 3*rightValue;
            rightOperand.Set(rightValue);
            Assert.AreEqual(leftValue + rightValue, result.Value);
        }

        [TestCase(2,3)]
        [TestCase(12,3)]
        [TestCase(2,43)]
        [TestCase(12,13)]
        public void ReactiveObject_ForMultiplyOperation_ComputesCorrectly(int leftValue, int rightValue) {
            R<int> leftOperand = leftValue;
            R<int> rightOperand = rightValue;

            R<int> result = R.Of(() => leftOperand * rightOperand);
            Assert.AreEqual(leftValue * rightValue, result.Value);

            leftValue = 2 * leftValue;
            leftOperand.Set(leftValue);
            Assert.AreEqual(leftValue * rightValue, result.Value);

            rightValue = 3 * rightValue;
            rightOperand.Set(rightValue);
            Assert.AreEqual(leftValue * rightValue, result.Value);
        }

        [TestCase("ab", 3)]
        [TestCase("xyz", 3)]
        [TestCase("xyz", 10)]
        public void ReactiveObject_ForArithmeticAndStringOperations_ComputesCorrectly(string leftValue, int rightValue) {
            R<string> leftOperand = leftValue;
            R<int> rightOperand = rightValue;

            R<int> result = R.Of(() => 2 * leftOperand.Value.Length * rightOperand + rightOperand * 3);
            Assert.AreEqual(2 * leftValue.Length * rightValue + rightValue * 3, result.Value);

            leftValue += leftValue;
            leftOperand.Set(leftValue);
            Assert.AreEqual(2 * leftValue.Length * rightValue + rightValue * 3, result.Value);

            rightValue = 3 * rightValue;
            rightOperand.Set(rightValue);
            Assert.AreEqual(2 * leftValue.Length * rightValue + rightValue * 3, result.Value);
        }

        [Test]
        public void ReactiveObject_ForReactivePropertiesOperations_ComputesCorrectly() {
            var person = new Person
            {
                Id = 0,
                Age = 30,
                Name = "John Doe",
                Type = "VIP"
            };

            var invoice = new Invoice
            {
                Amount = R.Of(() => person.Type=="VIP" ? person.Age * 30 : person.Age*10)
            };

            Assert.AreEqual(person.Age * 30, invoice.Amount);

            person.Age.Set(35);
            Assert.AreEqual(person.Age * 30, invoice.Amount);

            person.Type.Set("Regular");
            Assert.AreEqual(person.Age * 10, invoice.Amount);
        }

        internal class Person
        {
            public int Id { get; set; }
            public R<int> Age { get; set; }
            public R<string> Name { get; set; }
            public R<string> Type { get; set; }
        }

        internal class Invoice {
            public R<int> Amount { get; set; }
        }
    }
}
