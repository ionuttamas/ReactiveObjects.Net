using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using ReactiveObjects.Extensions;
using ReactiveObjects.Reactive;

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
        public void ReactiveObject_ForNestedReactivePropertiesOnly_ComputesCorrectly() {
            var person = new Person
            {
                Id = 0,
                Age = 30,
                Name = "John Doe",
                Type = "VIP"
            };

            var invoice = new Insurance
            {
                Cost = R.Of(() => person.Type=="VIP" ? person.Age * 10 : person.Age * 30)
            };

            Assert.AreEqual(person.Age * 10, invoice.Cost);

            person.Age.Set(35);
            Assert.AreEqual(person.Age * 10, invoice.Cost);

            person.Type.Set("Regular");
            Assert.AreEqual(person.Age * 30, invoice.Cost);
        }

        [Test]
        public void ReactiveObject_ForNestedReactivePropertiesAndRegularProperties_ComputesCorrectly() {
            var person = new Person {
                Id = 0,
                Risk = 10,
                Age = 30,
                Name = "John Doe",
                Type = "VIP"
            };

            var invoice = new Insurance {
                Cost = R.Of(() => person.Type == "VIP" ? person.Age * 10 : person.Age * 30 + person.Risk)
            };

            Assert.AreEqual(person.Age * 10, invoice.Cost);

            person.Age.Set(35);
            Assert.AreEqual(person.Age * 10, invoice.Cost);

            person.Type.Set("Regular");
            Assert.AreEqual(person.Age * 30 + person.Risk, invoice.Cost);
        }

        [Test]
        public void ReactiveObject_ForMultipleLevelNestedReactiveProperties_ComputesCorrectly() {
            var person = new Person {
                Id = 0,
                Age = 30,
                Name = "John Doe",
                Type = "VIP"
            };

            var personWrapper = new PersonWrapper
            {
                Person =  person,
                AdditionalRisk = 10
            };

            var personWrapperWrapper = new PersonWrapperWrapper {
                PersonWrapper = personWrapper
            };

            var invoice = new Insurance {
                Cost = R.Of(() => personWrapperWrapper.PersonWrapper.Person.Type == "VIP" ? personWrapper.Person.Age * 10 : personWrapper.Person.Age * 30 + personWrapper.AdditionalRisk)
            };

            Assert.AreEqual(person.Age * 10, invoice.Cost);

            person.Age.Set(35);
            Assert.AreEqual(person.Age * 10, invoice.Cost);

            person.Type.Set("Regular");
            Assert.AreEqual(personWrapper.Person.Age * 30 + personWrapper.AdditionalRisk, invoice.Cost);
        }

        [Test]
        public void ReactiveObject_ForCollectionReactiveOperations_ComputesCorrectly()
        {
            R<List<int>> list = new List<int> {1, 2};
            R<Stack<int>> stack = new Stack<int>();
            R<int> result = R.Of(() => 2 * list.Value.Count * stack.Value.Count + list.Value.First());

            Assert.AreEqual(2 * list.Value.Count * stack.Value.Count + list.Value.First(), result.Value);

            list.Add(3);
            stack.Push(3);
            Assert.AreEqual(2 * list.Value.Count * stack.Value.Count + list.Value.First(), result.Value);

            list.Remove(1);
            list.Remove(2);
            stack.Push(2);
            Assert.AreEqual(2 * list.Value.Count * stack.Value.Count + list.Value.First(), result.Value);

            list.Add(1);
            stack.Pop();
            Assert.AreEqual(2 * list.Value.Count * stack.Value.Count + list.Value.First(), result.Value);
        }

        [Ignore]
        [Test]
        public void ReactiveObject_ForGroups_ComputesCorrectly() {
            var person1 = new Person {
                Id = 0,
                Age = 30,
                Name = "John Doe",
                Type = "VIP"
            };

            var person2 = new Person {
                Id = 0,
                Age = 30,
                Name = "Joe Smith",
                Type = "Regular"
            };

            var group = new InsuranceGroup
            {
                CountryRisk = 10,
                Persons = new List<Person> {person1, person2}
            };

            var invoice = new Insurance {
                Cost = R.Of(() => group.CountryRisk * group.Persons.Sum(x => x.Age * 20))
            };

            Assert.AreEqual(group.CountryRisk * group.Persons.Sum(x => x.Age * 20), invoice.Cost);

            person1.Age.Set(35);
            Assert.AreEqual(group.CountryRisk * group.Persons.Sum(x => x.Age * 20), invoice.Cost);

            person2.Age.Set(60);
            Assert.AreEqual(group.CountryRisk * group.Persons.Sum(x => x.Age * 20), invoice.Cost);
        }

        private class InsuranceGroup
        {
            public int CountryRisk { get; set; }
            public List<Person> Persons { get; set; }
        }

        private class PersonWrapperWrapper
        {
            public PersonWrapper PersonWrapper { get; set; }
        }

        private class PersonWrapper
        {
            public Person Person { get; set; }
            public int AdditionalRisk { get; set; }
        }

        private class Person
        {
            public int Id { get; set; }
            public int Risk { get; set; }
            public R<int> Age { get; set; }
            public R<string> Name { get; set; }
            public R<string> Type { get; set; }
        }

        private class Insurance {
            public R<int> Cost { get; set; }
        }
    }
}
