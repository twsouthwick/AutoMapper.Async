using System.Threading;
using System.Threading.Tasks;

namespace AutoMapper
{
    public interface IAsyncTypeConverter<in TSource, in TDestination>
    {
        Task ResolveAsync(TSource source, TDestination destination, ResolutionContext context, CancellationToken token);
    }
}
