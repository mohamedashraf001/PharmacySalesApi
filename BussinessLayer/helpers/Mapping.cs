using AutoMapper;
using BusinessLayer.Requests;
using BusinessLayer.Responses;
using DataAccessLayer.Entities;
using DataLayer.Entities;
using SharedClasses.Responses;
using System.Numerics;
namespace BusinessLayer.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            //user mapping
            CreateMap<UserRequest, User>();
            CreateMap<User, UserResponse>();

            //role mapping
            CreateMap<Role, RoleResponse>();
            CreateMap<CreateRoleRequest, Role>();

            //permission mapping
            CreateMap<Permission, PermissionResponse>();

            CreateMap<Customer, CustomerResponse>();
            CreateMap<CustomerRequest, Customer>();

            CreateMap<Operation, OperationResponse>();
            CreateMap<OperationRequest, Operation>();
        }

    }
}