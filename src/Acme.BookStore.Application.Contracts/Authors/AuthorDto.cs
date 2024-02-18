using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Acme.BookStore.Authors
{
    /// <summary>
    /// AuthorDto
    /// </summary>
    public class AuthorDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public string ShortBio { get; set; }
    }

    /// <summary>
    /// GetAuthorListDto
    /// PagedAndSortedResultRequestDto 具有标准分页和排序属性：int MaxResultCount, int SkipCount 和 string Sorting
    /// </summary>
    public class GetAuthorListDto : PagedAndSortedResultRequestDto
    {
        public string? Filter { get; set; }
    }

    /// <summary>
    /// CreateAuthorDto
    /// </summary>
    public class CreateAuthorDto
    {
        [Required]
        [StringLength(AuthorConsts.MaxNameLength)]
        public string Name { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        public string ShortBio { get; set; }
    }

    /// <summary>
    /// UpdateAuthorDto
    /// </summary>
    public class UpdateAuthorDto
    {
        [Required]
        [StringLength(AuthorConsts.MaxNameLength)]
        public string Name { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        public string ShortBio { get; set; }
    }
}
