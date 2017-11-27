using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ReactiveObjects
{
    public class R<T>
    {
        private readonly Delegate computeFunc;
        private readonly List<R<object>> children;

        private R(Delegate computeFunc, List<R<object>> children)
        {
            this.computeFunc = computeFunc;
            this.children = children;
            ConfigureEventHandling();
        }

        public R(T value) {
            Value = value;
        }

        public event EventHandler<T> ValueChanged;
        public T Value { get; private set; }

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

            return new R<T>(modifier.ComputeFunc, modifier.ReactiveInstances);
        }

        public override bool Equals(object obj)
        {
            return Value.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        private void ConfigureEventHandling()
        {
            foreach (R<object> child in children)
            {
                child.ValueChanged += Child_ValueChanged;
            }
        }

        private void Child_ValueChanged(object sender, object e)
        {
            Value = (T)computeFunc.DynamicInvoke(children.Select(x => x.Value).ToArray());
        }

        private class ConvertModifier : ExpressionVisitor {
            private readonly Dictionary<ParameterExpression, R<object>> parameters;

            public Delegate ComputeFunc { get; private set; }
            public List<R<object>> ReactiveInstances { get; private set; }

            public ConvertModifier() {
                parameters = new Dictionary<ParameterExpression, R<object>>();
            }

            public void Modify(Expression expression) {
                var visitedExpression = (LambdaExpression)Visit(expression);
                var expressionReturnType = expression.Type.GetGenericArguments()[0];
                var expressionType = GetFuncType(expressionReturnType);
                var resultExpression = Expression.Lambda(expressionType, visitedExpression.Body, parameters.Keys);
                ComputeFunc = resultExpression.Compile();
                ReactiveInstances = parameters.Values.ToList();
            }

            protected override Expression VisitBinary(BinaryExpression b) {
                return base.VisitBinary(b);
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
                    R<object> capturedVariable = ((FieldInfo)memberExpression.Member).GetValue(closureExpression.Value);
                    string variableName = memberExpression.Member.Name;
                    ParameterExpression paramExpression = Expression.Parameter(memberType.GenericTypeArguments[0], variableName);
                    parameters[paramExpression] = capturedVariable;

                    var result = Expression.MakeMemberAccess(paramExpression, node.Member);
                    return result;
                }

                return base.VisitMember(node);
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
