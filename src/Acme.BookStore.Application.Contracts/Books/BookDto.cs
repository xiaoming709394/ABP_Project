﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Acme.BookStore.Books
{
    public class BookDto : AuditedEntityDto<Guid>
    {
        public string Name { get; set; }

        public BookType Type { get; set; }

        public DateTime PublishDate { get; set; }

        public float Price { get; set; }
        /// <summary>
        /// 作者Id
        /// </summary>
        public Guid AuthorId { get; set; }
        /// <summary>
        /// 作者名称
        /// </summary>
        public string AuthorName { get; set; }

    }
}
