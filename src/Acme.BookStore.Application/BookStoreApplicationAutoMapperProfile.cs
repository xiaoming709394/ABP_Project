using Acme.BookStore.Authors;
using Acme.BookStore.Books;
using AutoMapper;

namespace Acme.BookStore;

public class BookStoreApplicationAutoMapperProfile : Profile
{
    public BookStoreApplicationAutoMapperProfile()
    {
        //Book
        CreateMap<Book, BookDto>();
        CreateMap<CreateUpdateBookDto, Book>();

        //author
        //由于AuthorAppService 使用了ObjectMapper.Map<Author, AuthorDto> 所以需要该声明
        CreateMap<Author, AuthorDto>();
        CreateMap<Author, AuthorLookupDto>();
    }
}
