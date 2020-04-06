namespace SolidSampleApplication.Core
{
    public interface ISimpleEvent<T>
        where T : class
    {
        T ApplyToEntity(T entity);
    }
}