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
            var mapper = new MapperConfiguration(opt =>
            {
                opt.CreateMap<A, B>()
                    .ForMember(dest => dest.Item1, o => o.MapFromAsync().WithResolver(new AsyncA()));
            }).CreateMapper();

            var a = new A { Item1 = Guid.NewGuid().ToString() };

            var exception = Assert.Throws<AutoMapperMappingException>(() => mapper.Map<B>(a));

            Assert.IsType<InvalidOperationException>(exception.InnerException);
        }

        [Fact]
        public async Task SimpleAsync()
        {
            var mapper = new MapperConfiguration(opt =>
            {
                opt.CreateMap<A, B>()
                    .ForMember(dest => dest.Item1, o => o.MapFromAsync().WithResolver(new AsyncA()));
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

        private class AsyncA : IAsyncValueResolver<A, B, string>
        {
            public async Task<string> ResolveAsync(A source, B destination, string destMember, ResolutionContext context, CancellationToken token)
            {
                //await Task.Delay(100);

                return source.Item1;
            }
        }
    }
}
