using System.Threading;
using System.Threading.Tasks;

namespace AutoMapper
{
    public interface IAsyncValueResolver<in TSource, in TDestination, TDestMember>
    {
        Task<TDestMember> ResolveAsync(TSource source, TDestination destination, TDestMember destMember, ResolutionContext context, CancellationToken token);
    }
}
