using System.Linq;
using System.Timers;

namespace ReactiveObjects
{
    public class ThrottledUpdateStrategy<T> : IUpdateStrategy<T> {
        private readonly Timer timer;
        private R<T> reactiveObject;

        public ThrottledUpdateStrategy(int throttle)
        {
            timer = new Timer(throttle);
            timer.Elapsed += (sender, args) => InternalUpdate();
        }

        public void Update(R<T> instance)
        {
            reactiveObject = instance;
            timer.Start();
        }

        private void InternalUpdate()
        {
            var newValue = reactiveObject.Compute(reactiveObject.Children.Select(x => x.ValueObject).ToArray());

            if (!reactiveObject.Value.Equals(newValue)) {
                reactiveObject.Set(newValue);
            }
        }
    }
}