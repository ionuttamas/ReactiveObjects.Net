using System.Linq;

namespace ReactiveObjects
{
    public class ImmediateUpdateStrategy<T> : IUpdateStrategy<T>
    {
        public void Update(R<T> instance)
        {
            var newValue = instance.Compute(instance.Children.Select(x => x.ValueObject).ToArray());

            if (!instance.Value.Equals(newValue))
            {
                instance.Set(newValue);
            }
        }
    }
}