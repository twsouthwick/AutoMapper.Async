using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AutoMapper.Async.Tests
{
    public class TypeConverterTests
    {
        [Fact]
        public void InvalidContext()
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<A, B>()
                    .Async()
                    .ConvertUsing<AsyncTypeConverterA>();
            }).CreateMapper();

            var a = new A { Item1 = Guid.NewGuid().ToString() };

            Assert.Throws<InvalidOperationException>(() => mapper.Map<B>(a));
        }

        [Fact]
        public async Task SimpleAsyncTypeConverter()
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<A, B>()
                    .Async()
                    .ConvertUsing<AsyncTypeConverterA>();
            }).CreateMapper();

            var a = new A { Item1 = Guid.NewGuid().ToString() };

            var b = await mapper.MapAsync<B>(a);

            Assert.Equal(a.Item1, b.Item1);
        }

        [Fact]
        public async Task SimpleAsyncValueResolver()
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<A, B>()
                    .ForMember(m => m.Item1, opt => opt.Async().MapFrom<AsyncValueResolverA>());
            }).CreateMapper();

            var a = new A { Item1 = Guid.NewGuid().ToString() };

            var b = await mapper.MapAsync<B>(a);

            Assert.Equal(a.Item1, b.Item1);
        }

        [Fact]
        public async Task SimpleAsyncValueResolverDelegate()
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<A, B>()
                    .ForMember(m => m.Item1, opt => opt.Async().MapFrom(async (src, dest, _) =>
                    {
                        await Task.Yield();
                        return src.Item1;
                    }));
            }).CreateMapper();

            var a = new A { Item1 = Guid.NewGuid().ToString() };

            var b = await mapper.MapAsync<B>(a);

            Assert.Equal(a.Item1, b.Item1);
        }


        public class A
        {
            public string Item1 { get; set; }
        }

        public class B
        {
            public string Item1 { get; set; }
        }

        private class AsyncValueResolverA : IAsyncValueResolver<A, B, string>
        {
            public async Task<string> ResolveAsync(A source, B destination, string destMember, ResolutionContext context, CancellationToken token)
            {
                await Task.Yield();

                return source.Item1;
            }
        }

        private class AsyncTypeConverterA : IAsyncTypeConverter<A, B>
        {
            public async Task ResolveAsync(A source, B destination, ResolutionContext context, CancellationToken token)
            {
                await Task.Yield();

                destination.Item1 = source.Item1;
            }
        }
    }
}
