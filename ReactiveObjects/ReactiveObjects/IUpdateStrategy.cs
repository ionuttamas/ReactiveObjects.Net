using System;

namespace ReactiveObjects
{
    public interface IUpdateStrategy
    {
        void Update<T>(R<T> instance, object[] values);
    }

    public class ImmediateUpdateStrategy : IUpdateStrategy
    {
        public void Update<T>(R<T> instance, object[] values)
        {
            var newValue = instance.Compute(values);

            if (!instance.Value.Equals(newValue))
            {
                instance.Set(newValue);
            }
        }
    }
}