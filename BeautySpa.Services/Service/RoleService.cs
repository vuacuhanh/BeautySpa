using AutoMapper;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.RoleModelViews;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Services.Interface;

namespace BeautySpa.Services.Service
{
    public class RoleService : IRoles
    {
        private readonly RoleManager<ApplicationRoles> _roleManager;
        private readonly IMapper _mapper;

        public RoleService(RoleManager<ApplicationRoles> roleManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<BasePaginatedList<GETRoleModelViews>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                throw new ArgumentException("Page number and page size must be greater than 0.");
            }

            var query = _roleManager.Roles.AsQueryable();

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mappedItems = _mapper.Map<IReadOnlyCollection<GETRoleModelViews>>(items);

            return new BasePaginatedList<GETRoleModelViews>(mappedItems, totalCount, pageNumber, pageSize);
        }

        public async Task<GETRoleModelViews> GetByIdAsync(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                throw new Exception("Role not found.");
            }

            return _mapper.Map<GETRoleModelViews>(role);
        }

        public async Task<Guid> CreateAsync(POSTRoleModelViews model)
        {
            if (string.IsNullOrWhiteSpace(model.RoleName))
            {
                throw new ArgumentException("Role name cannot be empty.");
            }

            var roleExists = await _roleManager.RoleExistsAsync(model.RoleName);
            if (roleExists)
            {
                throw new Exception("Role already exists.");
            }

            var role = _mapper.Map<ApplicationRoles>(model);
            role.Id = Guid.NewGuid();
            role.CreatedTime = DateTimeOffset.UtcNow;
            role.LastUpdatedTime = role.CreatedTime;

            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                throw new Exception("Failed to create role: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return role.Id;
        }

        public async Task UpdateAsync(PUTRoleModelViews model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id.ToString());
            if (role == null)
            {
                throw new Exception("Role not found.");
            }

            _mapper.Map(model, role);
            role.LastUpdatedTime = DateTimeOffset.UtcNow;

            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                throw new Exception("Failed to update role: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}