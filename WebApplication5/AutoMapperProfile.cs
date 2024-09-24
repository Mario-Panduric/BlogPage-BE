using AutoMapper;
using WebApplication5.Entities;
using WebApplication5.DTOs;

namespace WebApplication5
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            this.CreateMap<Posts, PostsDto>().ReverseMap().ForMember(x => x.Id, opt => opt.Ignore());
            this.CreateMap<User, UserDto>().ReverseMap().ForMember(x => x.Id, opt => opt.Ignore());
            this.CreateMap<UsersComments, UsersCommentsDto>().ReverseMap().ForMember(x => x.Id, opt => opt.Ignore());
            this.CreateMap<Posts, GetPostDto>().ReverseMap().ForMember(x => x.Id, opt => opt.Ignore());
        }
    }
}
