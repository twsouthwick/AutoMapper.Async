using System;

namespace AutoMapper
{
    public readonly struct AsyncMappingExpression<TSource, TDestination>
    {
        private readonly IMappingExpressionBase<TSource, TDestination, IMappingExpression<TSource, TDestination>> _mapping;

        internal AsyncMappingExpression(IMappingExpressionBase<TSource, TDestination, IMappingExpression<TSource, TDestination>> mapping)
        {
            _mapping = mapping;
        }

        public void ConvertUsing<TConverter>()
            where TConverter : IAsyncTypeConverter<TSource, TDestination>, new()
        {
            ConvertUsing(new TConverter());
        }

        public void ConvertUsing(IAsyncTypeConverter<TSource, TDestination> converter)
        {
            if (_mapping is null)
            {
                throw new InvalidOperationException("Needs to be created from a valid mapping expression.");
            }

            _mapping.ConvertUsing(new AsyncTypeConverter(converter));
        }

        private class AsyncTypeConverter : ITypeConverter<TSource, TDestination>
        {
            private readonly IAsyncTypeConverter<TSource, TDestination> _typeConverter;

            public AsyncTypeConverter(IAsyncTypeConverter<TSource, TDestination> typeConverter)
            {
                _typeConverter = typeConverter;
            }

            public TDestination Convert(TSource source, TDestination destination, ResolutionContext context)
            {
                var asyncContext = context.GetAsyncContext();

                destination = context.Options.CreateInstance<TDestination>();

                var task = _typeConverter.ResolveAsync(source, destination, context, asyncContext.Token);

                asyncContext.Add(task);

                return destination;
            }
        }
    }
}
