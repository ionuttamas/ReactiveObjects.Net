using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveObjects
{
    class R {
        public static Expression Of(Expression<Func<R<object>>> expression) {
            return expression;
        }
    }

    class R<T> {


        public T Value { get; }

        public R(T value) {
            Value = value;
        }

        public static implicit operator T(R<T> instance) {
            return instance.Value;
        }

        public static implicit operator R<T>(T instance) {
            return new R<T>(instance);
        }

        public static implicit operator R<T>(Expression expression) {

            return null;
        }
    }
}
