using AutoMapper;
using Core.Entities;
using Core.Entities.Identity;
using Infrastructure.Models.DTO.AccountDTO;

namespace Infrastructure.Mapper
{
    public class AppMapProfile : Profile
    {
        public AppMapProfile() 
        {
            //User

            CreateMap<UserCreateDTO, UserEntity>()
                  .ForMember(x => x.Image, opt => opt.Ignore())
                  .ForMember(x => x.UserName, opt => opt.MapFrom(x => x.Email));
            CreateMap<UserItemDTO, UserEntity>();
            CreateMap<UserEntity, UserItemDTO>();
            CreateMap<UserEditDTO, UserEntity>();

            //Permission

            CreateMap<PermissionsEntity, PermissionItemDTO>();
            CreateMap<PermissionItemDTO, PermissionsEntity>();


        }
    }
}
