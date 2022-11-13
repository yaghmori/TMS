using AutoMapper;
using TMS.Core.Domain.Entities;
using TMS.Shared.Requests;
using TMS.Shared.Responses;

namespace TMS.Shared.MappingProfile
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
            DestinationMemberNamingConvention = new PascalCaseNamingConvention();

            CreateMap<SiloItem, SiloItemRequest>().ReverseMap();
            CreateMap<SiloItem, SiloItemResponse>().ReverseMap();
            CreateMap<SiloItemRequest, SiloItemResponse>().ReverseMap();
            //========================================
            CreateMap<User, UserRequest>().ReverseMap();
            CreateMap<User, UserResponse>().ReverseMap();
            CreateMap<User, RegisterRequest>().ReverseMap();/*.ForMember(dest => dest.NormalizedEmail, opt => opt.MapFrom(src => src.Email.ToUpper()));*/
            CreateMap<UserResponse, UserRequest>().ReverseMap();
            CreateMap<User, NewUserRequest>().ReverseMap();

            //========================================
            CreateMap<Tenant, TenantRequest>().ReverseMap();
            CreateMap<Tenant, TenantResponse>().ReverseMap();
            CreateMap<TenantResponse, TenantRequest>().ReverseMap();

            //=====================================
            CreateMap<Role, RoleRequest>().ReverseMap();
            CreateMap<Role, RoleResponse>().ReverseMap();
            CreateMap<RoleRequest, RoleResponse>().ReverseMap();

            //=====================================
            CreateMap<AppSetting, AppSettingsRequest>().ReverseMap();
            CreateMap<AppSetting, AppSettingsResponse>().ReverseMap();
            CreateMap<AppSettingsRequest, AppSettingsResponse>().ReverseMap();

            //=====================================
            CreateMap<UserSetting, UserSettingsRequest>().ReverseMap();
            CreateMap<UserSetting, UserSettingsResponse>().ReverseMap();
            CreateMap<UserSettingsRequest, UserSettingsResponse>().ReverseMap();

        }
    }
}
