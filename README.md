# ReactiveObjects
Library for easy handling changes to objects 

# Arithmetic reactivity
```csharp
R<int> leftOperand = 2;
R<int> rightOperand = 3;

R<int> result = R.Of(() => leftOperand * rightOperand);
Assert.AreEqual(6, result.Value);

leftOperand.Set(4);
Assert.AreEqual(12, result.Value);
```

# Arithmetic and string based reactivity
```csharp
R<string> stringOperand = "abc";
R<int> intOperand = 2;

R<int> result = R.Of(() => 2 * stringOperand.Value.Length + intOperand);
Assert.AreEqual(8, result.Value);

leftOperand.Set("abcde");
Assert.AreEqual(12, result.Value);
```

# Properties based reactivity
```csharp
public class Person
{ 
    public int Risk { get; set; }
    public R<int> Age { get; set; }
    public R<string> Name { get; set; }
    public R<string> Type { get; set; }
}

public class Insurance 
{
    public R<int> Cost { get; set; }
}

var person = new Person
            {
                Age = 30,
                Name = "John Doe",
                Type = "VIP"
            };

var invoice = new Insurance();
invoice.Cost = R.Of(() => person.Type=="VIP" ? person.Age * 10 : person.Age * 20)

Assert.AreEqual(300, invoice.Cost);

person.Age.Set(35);
Assert.AreEqual(350, invoice.Cost);

person.Type.Set("Regular");
Assert.AreEqual(700, invoice.Cost);
```

# Collections based reactivity
```csharp
R<List<int>> list = new List<int> {1, 2};
R<Stack<int>> stack = new Stack<int>();
R<int> result = R.Of(() => stack.Value.Count + list.Value.First());

Assert.AreEqual(stack.Value.Count + list.Value.First(), result.Value);

list.Add(3);
stack.Push(3);
Assert.AreEqual(stack.Value.Count + list.Value.First(), result.Value);

list.Remove(1);
list.Remove(2);
stack.Push(2);
Assert.AreEqual(stack.Value.Count + list.Value.First(), result.Value);

list.Add(1);
stack.Pop();
Assert.AreEqual(stack.Value.Count + list.Value.First(), result.Value);
```
