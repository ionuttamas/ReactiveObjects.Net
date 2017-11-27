using System;
using System.Linq.Expressions;

namespace ReactiveObjects
{
    public class R {
        public static Expression Of(Expression<Func<R<object>>> expression) {
            return expression;
        }
    }
}