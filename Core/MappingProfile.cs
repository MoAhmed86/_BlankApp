using AutoMapper;
using Core.DTOs;
using Core.DTOs.Identity;
using Core.DTOs.ServiceNow;
using Core.Entities;
using System;

namespace Core
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LookupItem, LookupItemDto>().ReverseMap();

            CreateMap<LookupItem, OrganizerDto>()
                .ForMember(dest => dest.PlatformName, opt => opt.MapFrom(src => src.ParentLookupItem.DescAr));

            CreateMap<RegisterDto, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.EmailAddress))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailAddress));

            CreateMap<ApplicationUser, RegisterDto>()
                .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.Email));

            CreateMap<ApplicationUser, UserInfoDto>();

            #region Center

            CreateMap<CenterDto, Center>().ReverseMap();

            #endregion

            #region ServiceNow

            CreateMap<TicketVM, TicketDto>()
            .ForMember(d => d.u_source, opts => opts.MapFrom(s => s.Source))
            .ForMember(d => d.category, opts => opts.MapFrom(s => s.CategoryId))
            .ForMember(d => d.subcategory, opts => opts.MapFrom(s => s.SubCategoryId))
            .ForMember(d => d.u_customer_name, opts => opts.MapFrom(s => s.FullName))
            .ForMember(d => d.short_description, opts => opts.MapFrom(s => s.ShortDescription))
            .ForMember(d => d.u_customer_email, opts => opts.MapFrom(s => s.EMail))
            .ForMember(d => d.u_id, opts => opts.MapFrom(s => s.IdentityNumber))
            .ForMember(d => d.u_mobile, opts => opts.MapFrom(s => s.MobileNumber))
            .ForMember(d => d.description, opts => opts.MapFrom(s => s.Description))
            .ForMember(d => d.comments, opts => opts.MapFrom(s => s.Comments));

            CreateMap<UserTicketsResponseDetails, TicketVM>()
            .ForMember(d => d.Source, opts => opts.MapFrom(s => s.Source))
            .ForMember(d => d.Category, opts => opts.MapFrom(s => s.Category))
            .ForMember(d => d.SubCategory, opts => opts.MapFrom(s => s.Subcategory))
            .ForMember(d => d.FullName, opts => opts.MapFrom(s => s.Customer_Name))
            .ForMember(d => d.ShortDescription, opts => opts.MapFrom(s => s.Short_Description))
            .ForMember(d => d.EMail, opts => opts.MapFrom(s => s.Customer_Email))
            .ForMember(d => d.IdentityNumber, opts => opts.MapFrom(s => s.ID))
            .ForMember(d => d.MobileNumber, opts => opts.MapFrom(s => s.Mobile))
            .ForMember(d => d.Description, opts => opts.MapFrom(s => s.Description))
            .ForMember(d => d.Comments, opts => opts.MapFrom(s => s.Work_Notes))
            .ForMember(d => d.TicketNumber, opts => opts.MapFrom(s => s.Number))
            .ForMember(d => d.SystemId, opts => opts.MapFrom(s => s.Sys_id))
            .ForMember(d => d.LastUpdatedDateHijri, opts => opts.MapFrom(s => CommonStaticFunctions.ConvertGregToUmAlQura(DateTime.Parse(s.Last_Updated))))
            .ForMember(d => d.State, opts => opts.MapFrom(s => CommonStaticFunctions.GetArabicState(s.State_Id)))
            .ForMember(d => d.CreatedDate, opts => opts.MapFrom(s => DateTime.Parse(s.Created_On)))
            .ForMember(d => d.CreatedDateHijri, opts => opts.MapFrom(s => CommonStaticFunctions.ConvertGregToUmAlQura(DateTime.Parse(s.Created_On))))
            .ForMember(d => d.CreatedBy, opts => opts.MapFrom(s => s.Created_By))
            .ForMember(d => d.StateId, opts => opts.MapFrom(s => int.Parse(s.State_Id)))
            .ForMember(d => d.StateColor, opts => opts.MapFrom(s => CommonStaticFunctions.GetStateColor(s.State_Id)));

            #endregion
        }
    }
}
