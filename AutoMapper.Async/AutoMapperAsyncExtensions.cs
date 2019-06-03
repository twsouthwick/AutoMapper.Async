using System;
using System.Threading;
using System.Threading.Tasks;

namespace AutoMapper
{
    public static class AutoMapperAsyncExtensions
    {
        internal static readonly string ItemKey = Guid.NewGuid().ToString();

        public static AsyncMappingExpression<TSource, TDestination> Async<TSource, TDestination>(this IMappingExpressionBase<TSource, TDestination, IMappingExpression<TSource, TDestination>> mapping)
            => new AsyncMappingExpression<TSource, TDestination>(mapping);

        public static AsyncMemberConfigurationExpression<TSource, TDestination, TMember> Async<TSource, TDestination, TMember>(this IMemberConfigurationExpression<TSource, TDestination, TMember> config)
            => new AsyncMemberConfigurationExpression<TSource, TDestination, TMember>(config);

        public static async Task<TDestination> MapAsync<TDestination>(this IMapper mapper, object obj, CancellationToken token = default)
        {
            var context = new AsyncContext(token);

            var result = mapper.Map<TDestination>(obj, opts =>
            {
                opts.Items.Add(ItemKey, context);

            });

            await context.WhenAllAsync().ConfigureAwait(false);

            return result;
        }

        internal static AsyncContext GetAsyncContext(this ResolutionContext context)
        {
            if (context.Items.TryGetValue(ItemKey, out var asyncCtx) && asyncCtx is AsyncContext memberAsyncCtx)
            {
                return memberAsyncCtx;
            }

            throw new InvalidOperationException("Value resolution must occur in an async context.");
        }
    }
}
