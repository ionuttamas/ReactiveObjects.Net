using ReactiveObjects.Reactive;

namespace ReactiveObjects.UpdateStrategy
{
    public interface IUpdateStrategy<T>
    {
        void Update(R<T> instance);
    }
}