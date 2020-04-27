namespace SolidSampleApplication.Core
{
    public interface IReadModel<T>
    {
        void FromAggregate(T aggregate);
    }
}