using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Acme.BookStore.Books
{
    /// <summary>
    /// 作者下拉信息实体
    /// </summary>
    public class AuthorLookupDto: EntityDto<Guid>
    {
        public string Name { get; set; }
    }
}
