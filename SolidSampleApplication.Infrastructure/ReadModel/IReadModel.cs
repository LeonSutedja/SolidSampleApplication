namespace SolidSampleApplication.Infrastructure
{
    public interface IReadModel<T>
    {
        void FromAggregate(T aggregate);
    }
}