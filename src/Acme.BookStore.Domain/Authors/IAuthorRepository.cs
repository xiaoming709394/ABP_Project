using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Acme.BookStore.Authors
{
    public interface IAuthorRepository : IRepository<Author, Guid>
    {
        /// <summary>
        /// 通过姓名获取作者信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<Author> FindByNameAsync(string name);
        /// <summary>
        /// 获取作者信息列表
        /// </summary>
        /// <param name="skipCount"></param>
        /// <param name="maxResultCount"></param>
        /// <param name="sorting"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<List<Author>> GetListAsync(int skipCount, int maxResultCount, string sorting, string filter = null);
    }
}
