namespace AutoMapper
{
    internal class AsyncTypeConverter<TSource, TDestination> : ITypeConverter<TSource, TDestination>
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
