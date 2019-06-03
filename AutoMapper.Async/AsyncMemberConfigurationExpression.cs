using System;
using System.Reflection;
using System.Threading.Tasks;

namespace AutoMapper
{
    public readonly struct AsyncMemberConfigurationExpression<TSource, TDestination, TMember>
    {
        private readonly IMemberConfigurationExpression<TSource, TDestination, TMember> _configuration;

        public AsyncMemberConfigurationExpression(IMemberConfigurationExpression<TSource, TDestination, TMember> configuration)
        {
            _configuration = configuration;

        }

        public void MapFrom<TValueResolver>()
            where TValueResolver : IAsyncValueResolver<TSource, TDestination, TMember>, new()
        {
            _configuration.MapFrom(new AsyncValueResolver(new TValueResolver(), _configuration.DestinationMember));
        }

        private class AsyncValueResolver : IValueResolver<TSource, TDestination, TMember>
        {
            private readonly IAsyncValueResolver<TSource, TDestination, TMember> _resolver;
            private readonly MemberInfo _member;

            public AsyncValueResolver(IAsyncValueResolver<TSource, TDestination, TMember> resolver, MemberInfo member)
            {
                _resolver = resolver;
                _member = member;
            }

            private void SetValue(TDestination destination, TMember member)
            {
                if (_member is PropertyInfo property)
                {
                    property.GetSetMethod().Invoke(destination, new object[] { member });
                }
                else
                {
                    throw new InvalidOperationException("Cannot set value");
                }
            }

            public TMember Resolve(TSource source, TDestination destination, TMember destMember, ResolutionContext context)
            {
                var asyncContext = context.GetAsyncContext();
                var task = RunAsync(source, destination, destMember, context, asyncContext);

                asyncContext.Add(task);

                return destMember;
            }

            private async Task RunAsync(TSource source, TDestination destination, TMember destMember, ResolutionContext context, AsyncContext asyncContext)
            {
                var result = await _resolver.ResolveAsync(source, destination, destMember, context, asyncContext.Token).ConfigureAwait(false);

                SetValue(destination, result);
            }
        }
    }
}
