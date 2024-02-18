using Acme.BookStore.Authors;
using Acme.BookStore.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace Acme.BookStore.Books
{
    [Authorize(BookStorePermissions.Books.Default)]
    public class BookAppService :
        CrudAppService<
            Book, //The Book entity
            BookDto, //Used to show books
            Guid, //Primary key of the book entity
            PagedAndSortedResultRequestDto, //Used for paging/sorting
            CreateUpdateBookDto>, //Used to create/update a book
        IBookAppService //implement the IBookAppService
    {
        //注入作者仓储
        private readonly IAuthorRepository _authorRepository;

        public BookAppService(IRepository<Book, Guid> repository, IAuthorRepository authorRepository)
            : base(repository)
        {
            _authorRepository = authorRepository;
            GetPolicyName = BookStorePermissions.Books.Default;
            GetListPolicyName = BookStorePermissions.Books.Default;
            CreatePolicyName = BookStorePermissions.Books.Create;
            UpdatePolicyName = BookStorePermissions.Books.Edit;
            DeletePolicyName = BookStorePermissions.Books.Delete;
        }

        /// <summary>
        /// 重新获取书籍信息接口
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task<BookDto> GetAsync(Guid id)
        {
            //从repository获取IQueryable<Book>
            var queryable = await Repository.GetQueryableAsync();

            var query = from book in queryable
                        join author in await _authorRepository.GetQueryableAsync() on book.AuthorId equals author.Id
                        where book.Id == id
                        select new { book, author };

            //获取作者信息
            var queryResult = await AsyncExecuter.FirstOrDefaultAsync(query);
            if (queryResult == null)
            {
                throw new EntityNotFoundException(typeof(Book), id);
            }

            //实体映射
            var bookDto = ObjectMapper.Map<Book, BookDto>(queryResult.book);
            bookDto.AuthorName = queryResult.author.Name;
            return bookDto;
        }

        public override async Task<PagedResultDto<BookDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            //从repository获取IQueryable<Book>
            var queryable = await Repository.GetQueryableAsync();

            //查看列表信息
            var query = from book in queryable
                        join author in await _authorRepository.GetQueryableAsync() on book.AuthorId equals author.Id
                        select new { book, author };

            //分页
            //OrderBy 方法需要引入using System.Linq.Dynamic.Core 命名空间
            query = query.OrderBy(NormalizeSorting(input.Sorting))
                .Skip(input.SkipCount).Take(input.MaxResultCount);

            //执行查询获取list
            var queryResult = await AsyncExecuter.ToListAsync(query);

            //映射实体（其实也不用这么麻烦，直接把查询到信息装到BookDto即可）
            var bookDtos = queryResult.Select(x =>
            {
                var bookDto = ObjectMapper.Map<Book, BookDto>(x.book);
                bookDto.AuthorName = x.author.Name;
                return bookDto;
            }).ToList();

            //获取总行数
            var totalCount = bookDtos.Count;//await Repository.GetCountAsync();

            return new PagedResultDto<BookDto>(totalCount, bookDtos);
        }

        /// <summary>
        /// 获取作者下拉列表
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ListResultDto<AuthorLookupDto>> GetAuthorLookupAsync()
        {
            var authors = await _authorRepository.GetListAsync();

            return new ListResultDto<AuthorLookupDto>(
                ObjectMapper.Map<List<Author>, List<AuthorLookupDto>>(authors));
        }

        /// <summary>
        /// 组装排序字段
        /// </summary>
        /// <param name="sorting"></param>
        /// <returns></returns>
        private static string NormalizeSorting(string sorting)
        {
            //默认以书籍名称排序
            if (sorting.IsNullOrEmpty())
            {
                return $"book.{nameof(Book.Name)}";
            }

            if (sorting.Contains("authorName", StringComparison.OrdinalIgnoreCase))
            {
                return sorting.Replace("authorName", "author.Name", StringComparison.OrdinalIgnoreCase);
            }

            return $"book.{sorting}";
        }
    }
}
