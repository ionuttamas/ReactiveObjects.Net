using System;
using System.Linq.Expressions;

namespace ReactiveObjects.Reactive
{
    public class R {
        public static Expression Of(Expression<Func<object>> expression) {
            return expression;
        }
    }
}