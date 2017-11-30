using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ReactiveObjects
{
    public class R<T> : ReactiveBase {
        private readonly Func<object[], T> computeFunc;
        private readonly IUpdateStrategy<T> updateStrategy;

        private R(Delegate computeFunc, List<ReactiveBase> children)
        {
            this.computeFunc = (Func<object[], T>)computeFunc;
            this.Children = children;
            updateStrategy = new ImmediateUpdateStrategy<T>();
            Configure();
        }

        public R(T value) {
            Value = value;
            OnValueObjectChanged(value);
            ValueChanged?.Invoke(this, value);
            updateStrategy = new ImmediateUpdateStrategy<T>();
        }

        public T Value { get; private set; }

        public List<ReactiveBase> Children { get; }

        internal override object ValueObject => Value;

        public event EventHandler<T> ValueChanged;

        public static implicit operator T(R<T> instance) {
            return instance.Value;
        }

        public static implicit operator R<T>(T instance) {
            return new R<T>(instance);
        }

        public static implicit operator R<T>(Expression expression)
        {
            var modifier = new ConvertModifier();
            modifier.Modify(expression);

            return new R<T>(modifier.ComputeFunc, modifier.Children);
        }

        public void Set(T value) {
            Value = value;
            OnValueObjectChanged(value);
            ValueChanged?.Invoke(this, value);
        }

        public T Compute(object[] values)
        {
            var value = computeFunc(values);

            return value;
        }

        public override bool Equals(object obj)
        {
            return Value.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        private void Configure()
        {
            foreach (ReactiveBase child in Children)
            {
                child.ValueObjectChanged += Child_ValueChanged;
            }

            var arguments = Children.Select(x => x.ValueObject).ToArray();
            var value = computeFunc(arguments);
            Value = value;
        }

        private void Child_ValueChanged(object sender, object e)
        {
            updateStrategy.Update(this);
        }

        private class ConvertModifier : ExpressionVisitor {
            private readonly Dictionary<ParameterExpression, object> parameters;

            public Delegate ComputeFunc { get; private set; }
            public List<ReactiveBase> Children { get; private set; }

            public ConvertModifier() {
                parameters = new Dictionary<ParameterExpression, object>();
            }

            public void Modify(Expression expression) {
                var visitedExpression = (LambdaExpression)Visit(expression);
                var expressionReturnType = expression.Type.GetGenericArguments()[0];
                var expressionType = GetFuncType(expressionReturnType);
                var resultExpression = Expression.Lambda(expressionType, visitedExpression.Body, parameters.Keys);
                ComputeFunc = resultExpression.Compile();
                Children = parameters.Values.Select(x=>(ReactiveBase)x).ToList();

                var computeBody = Expression.Lambda(Expression.Convert(resultExpression.Body, typeof(T)), resultExpression.Parameters);
                var inputs = Expression.Parameter(typeof(object[]), "args");
                var conversionExpression = resultExpression.Parameters.Select((v, i) => Expression.Convert(Expression.ArrayIndex(inputs, Expression.Constant(i)), v.Type)).ToArray();
                var expressionWrapper = Expression.Lambda(Expression.Invoke(computeBody, conversionExpression), inputs);
                ComputeFunc = (Func<object[], T>)expressionWrapper.Compile();
            }

            protected override Expression VisitBinary(BinaryExpression node)
            {
                Expression leftConvertedExpression = VisitOperand(node.Left);
                Expression rightConvertedExpression = VisitOperand(node.Right);

                return Expression.MakeBinary(node.NodeType, leftConvertedExpression, rightConvertedExpression);
            }

            protected override Expression VisitMember(MemberExpression node) {
                if (!(node.Expression is MemberExpression))
                    return base.VisitMember(node);

                var valueExpression = (MemberExpression)node.Expression;

                if (valueExpression.Member.Name == nameof(R<object>.Value) &&
                    valueExpression.Expression.Type.Name == typeof(R<>).Name) {
                    var memberExpression = (MemberExpression)valueExpression.Expression;
                    var closureExpression = (ConstantExpression)memberExpression.Expression;
                    var memberType = ((FieldInfo)memberExpression.Member).FieldType;
                    var capturedVariable = ((FieldInfo)memberExpression.Member).GetValue(closureExpression.Value);
                    string variableName = memberExpression.Member.Name;
                    ParameterExpression paramExpression = Expression.Parameter(memberType.GenericTypeArguments[0], variableName);
                    parameters[paramExpression] = capturedVariable;

                    var result = Expression.MakeMemberAccess(paramExpression, node.Member);
                    return result;
                }

                return base.VisitMember(node);
            }

            private Expression VisitOperand(Expression expression)
            {
                if (expression.NodeType == ExpressionType.Convert && expression is UnaryExpression) {
                    var unaryExpression = (UnaryExpression)expression;
                    Expression convertedExpression;

                    if (!(unaryExpression.Operand is MemberExpression)) {
                        convertedExpression = Visit(expression);
                    } else {
                        var operandExpression = (MemberExpression)unaryExpression.Operand;

                        if (operandExpression.Type.Name != typeof(R<>).Name) {
                            convertedExpression = Visit(expression);
                        } else if(operandExpression.Expression is MemberExpression) {
                            var memberExpression = (MemberExpression)operandExpression.Expression;
                            var closureExpression = (ConstantExpression)memberExpression.Expression;
                            Type memberType = operandExpression.Type;
                            object containerInstance = ((FieldInfo)memberExpression.Member).GetValue(closureExpression.Value);
                            object capturedVariable = containerInstance.GetValue(operandExpression.Member.Name);
                            string variableName = $"{memberExpression.Member.Name}{operandExpression.Member.Name}";
                            Type genericType = memberType.GenericTypeArguments[0];
                            ParameterExpression paramExpression = Expression.Parameter(genericType, variableName);
                            parameters[paramExpression] = capturedVariable;
                            convertedExpression = paramExpression;
                        }
                        else {
                            var closureExpression = (ConstantExpression)operandExpression.Expression;
                            Type memberType = ((FieldInfo)operandExpression.Member).FieldType;
                            object capturedVariable = ((FieldInfo)operandExpression.Member).GetValue(closureExpression.Value);
                            string variableName = operandExpression.Member.Name;
                            Type genericType = memberType.GenericTypeArguments[0];
                            ParameterExpression paramExpression = Expression.Parameter(genericType, variableName);
                            parameters[paramExpression] = capturedVariable;
                            convertedExpression = paramExpression;
                        }
                    }

                    return convertedExpression;
                }

                if (expression is BinaryExpression)
                {
                    return VisitBinary((BinaryExpression) expression);
                }

                return Visit(expression);
            }

            private Type GetFuncType(Type returnType) {
                Type delegateType = null;

                switch (parameters.Count) {
                    case 0:
                        delegateType = typeof(Func<>);
                        break;
                    case 1:
                        delegateType = typeof(Func<,>);
                        break;
                    case 2:
                        delegateType = typeof(Func<,,>);
                        break;
                    case 3:
                        delegateType = typeof(Func<,,,>);
                        break;
                    case 4:
                        delegateType = typeof(Func<,,,,>);
                        break;
                    case 5:
                        delegateType = typeof(Func<,,,,,>);
                        break;
                    case 6:
                        delegateType = typeof(Func<,,,,,,>);
                        break;
                    case 7:
                        delegateType = typeof(Func<,,,,,,,>);
                        break;
                    case 8:
                        delegateType = typeof(Func<,,,,,,,,>);
                        break;
                    case 9:
                        delegateType = typeof(Func<,,,,,,,,,>);
                        break;
                    case 10:
                        delegateType = typeof(Func<,,,,,,,,,,>);
                        break;
                    case 11:
                        delegateType = typeof(Func<,,,,,,,,,,,>);
                        break;
                    case 12:
                        delegateType = typeof(Func<,,,,,,,,,,,,>);
                        break;
                    case 13:
                        delegateType = typeof(Func<,,,,,,,,,,,,,>);
                        break;
                    case 14:
                        delegateType = typeof(Func<,,,,,,,,,,,,,,>);
                        break;
                    case 15:
                        delegateType = typeof(Func<,,,,,,,,,,,,,,,>);
                        break;
                    case 16:
                        delegateType = typeof(Func<,,,,,,,,,,,,,,,,>);
                        break;
                }

                var funcType = delegateType.MakeGenericType(parameters.Select(x => x.Key.Type).Concat(new[] { returnType }).ToArray());

                return funcType;
            }
        }
    }
}
