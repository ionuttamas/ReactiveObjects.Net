namespace ReactiveObjects
{
    public interface IUpdateStrategy<T>
    {
        void Update(R<T> instance);
    }
}