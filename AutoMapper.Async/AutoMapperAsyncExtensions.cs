using System;
using System.Threading;
using System.Threading.Tasks;

namespace AutoMapper
{
    public static class AutoMapperAsyncExtensions
    {
        internal static readonly string ItemKey = Guid.NewGuid().ToString();

        internal static AsyncContext GetAsyncContext(this ResolutionContext context)
        {
            if (context.Items.TryGetValue(ItemKey, out var asyncCtx) && asyncCtx is AsyncContext memberAsyncCtx)
            {
                return memberAsyncCtx;
            }

            throw new InvalidOperationException("Value resolution must occur in an async context.");
        }

        public static AsyncMapFromBuilder<TSource, TDestination, TMember> MapFromAsync<TSource, TDestination, TMember>(this IMemberConfigurationExpression<TSource, TDestination, TMember> config)
        {
            return new AsyncMapFromBuilder<TSource, TDestination, TMember>(config);
        }

        public static async Task<TDestination> MapAsync<TDestination>(this IMapper mapper, object obj, CancellationToken token = default)
        {
            var context = new AsyncContext(token);

            var result = mapper.Map<TDestination>(obj, opts =>
            {
                opts.Items.Add(ItemKey, context);
            });

            await context.WhenAllAsync();

            return result;
        }
    }

    public readonly struct AsyncMapFromBuilder<TSource, TDestination, TMember>
    {
        private readonly IMemberConfigurationExpression<TSource, TDestination, TMember> _configuration;

        public AsyncMapFromBuilder(IMemberConfigurationExpression<TSource, TDestination, TMember> configuration)
        {
            _configuration = configuration;
        }

        public void WithResolver<TResolver>(TResolver resolver)
            where TResolver : IAsyncValueResolver<TSource, TDestination, TMember>
        {
            _configuration.MapFrom(new AsyncCustomResolver<TSource, TDestination, TMember>(resolver));
        }
    }
}
