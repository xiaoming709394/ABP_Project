using System;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Modularity;
using Xunit;

namespace Acme.BookStore.Authors
{
    public abstract class AuthorAppService_Tests<TStartupModule> : BookStoreApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IAuthorAppService _authorAppService;

        protected AuthorAppService_Tests()
        {
            _authorAppService = GetRequiredService<IAuthorAppService>();
        }

        [Fact]
        public async Task Should_Get_All_Authors_Without_Any_Filter()
        {
            var result = _authorAppService.GetListAsync(new GetAuthorListDto());
        }
    }
}
