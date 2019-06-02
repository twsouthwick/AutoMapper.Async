using System.Threading.Tasks;

namespace AutoMapper
{
    internal class AsyncCustomResolver<TSource, TDestination, TDestMember> : IValueResolver<TSource, TDestination, TDestMember>
    {
        private readonly IAsyncValueResolver<TSource, TDestination, TDestMember> _asyncResolver;

        public AsyncCustomResolver(IAsyncValueResolver<TSource, TDestination, TDestMember> asyncResolver)
        {
            _asyncResolver = asyncResolver;
        }

        public TDestMember Resolve(TSource source, TDestination destination, TDestMember destMember, ResolutionContext context)
        {
            var asyncContext = context.GetAsyncContext();

            //destMember = context.Options.CreateInstance<TDestMember>();

            var task = _asyncResolver.ResolveAsync(source, destination, destMember, context, asyncContext.Token)
                .ContinueWith(t =>
                {
                    if (t.IsCompleted)
                    {
                        destMember = t.Result;
                    }
                });

            asyncContext.Add(task);

            task.Wait();

            return destMember;
        }
    }
}
