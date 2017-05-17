namespace Nop.Plugin.Api.Factories
{
    public interface IMapper<in T, out TK>
    {
        TK Map(T item);
    }
}
