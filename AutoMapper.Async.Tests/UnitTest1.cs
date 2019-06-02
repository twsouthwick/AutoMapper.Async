using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AutoMapper.Async.Tests
{

    public class UnitTest1
    {
        [Fact]
        public void InvalidContext()
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<A, B>()
                    .Async()
                    .ConvertUsing<AsyncA>();
            }).CreateMapper();

            var a = new A { Item1 = Guid.NewGuid().ToString() };

            Assert.Throws<InvalidOperationException>(() => mapper.Map<B>(a));
        }

        [Fact]
        public async Task SimpleAsync()
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<A, B>()
                    .Async()
                    .ConvertUsing<AsyncA>();
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


        private class AsyncA : IAsyncTypeConverter<A, B>
        {
            public async Task ResolveAsync(A source, B destination, ResolutionContext context, CancellationToken token)
            {
                await Task.Delay(100);

                destination.Item1 = source.Item1;
            }
        }
    }
}
