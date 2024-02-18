using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Acme.BookStore.Authors
{
    public class AuthorManager : DomainService
    {
        private readonly IAuthorRepository _authorRepository;
        public AuthorManager(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        /// <summary>
        /// 新建作者实体
        /// </summary>
        /// <param name="name"></param>
        /// <param name="birthDate"></param>
        /// <param name="shortBio"></param>
        /// <returns></returns>
        public async Task<Author> CreateAsync(string name, DateTime birthDate, string? shortBio = null)
        {
            //判断名称是否为空
            Check.NotNullOrWhiteSpace(name, nameof(name));

            //判断作者是否存在
            var existingAuthor = await _authorRepository.FindByNameAsync(name);
            if (existingAuthor != null)
            {
                throw new AuthorAlreadyExistsException(name);
            }

            return new Author(
                GuidGenerator.Create(),
                name,
                birthDate,
                shortBio
                );
        }

        /// <summary>
        /// 改名
        /// </summary>
        /// <param name="author"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        /// <exception cref="AuthorAlreadyExistsException"></exception>
        public async Task ChangeNameAsync(Author author, string newName)
        {
            Check.NotNull(author, nameof(author));
            Check.NotNullOrWhiteSpace(newName, nameof(newName));

            var existingAuthor = await _authorRepository.FindByNameAsync(newName);
            if(existingAuthor != null && existingAuthor.Id != author.Id)
            {
                throw new AuthorAlreadyExistsException(newName);
            }

            author.ChangeName(newName);
        }
    }
}
