using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.RequestBecomeProviderModelView;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using EntityServiceProvider = BeautySpa.Contract.Repositories.Entity.ServiceProvider;

namespace BeautySpa.Services.Service
{
    public class RequestBecomeProviderService : IRequestBecomeProvider
    {
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        public RequestBecomeProviderService(IEmailService emailService, IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<BaseResponseModel<Guid>> CreateRequestAsync(POSTRequestBecomeProviderModelView model)
        {
            var userId = Guid.Parse(CurrentUserId);
            var user = await _unitOfWork.GetRepository<ApplicationUsers>().GetByIdAsync(userId)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "User not found.");

            foreach (var categoryId in model.ServiceCategoryIds)
            {
                _ = await _unitOfWork.GetRepository<ServiceCategory>().GetByIdAsync(categoryId)
                    ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, $"Service category {categoryId} not found.");
            }

            var existing = await _unitOfWork.GetRepository<RequestBecomeProvider>()
                .Entities.FirstOrDefaultAsync(x => x.UserId == userId && x.RequestStatus == "pending");

            if (existing != null)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Already have pending request.");

            var request = _mapper.Map<RequestBecomeProvider>(model);
            request.Id = Guid.NewGuid();
            request.UserId = userId;
            request.RequestStatus = "pending";
            request.CreatedBy = CurrentUserId;
            request.CreatedTime = CoreHelper.SystemTimeNow;
            request.LastUpdatedBy = CurrentUserId;
            request.LastUpdatedTime = request.CreatedTime;

            await _unitOfWork.GetRepository<RequestBecomeProvider>().InsertAsync(request);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<Guid>.Success(request.Id);
        }
        public async Task<BaseResponseModel<Guid>> RegisterByGuestAsync(RegisterRequestBecomeProviderModelView model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.FullName))
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Email và họ tên là bắt buộc.");

            // Kiểm tra email đã tồn tại trong hệ thống chưa
            bool exists = await _unitOfWork.GetRepository<ApplicationUsers>()
                .Entities.AnyAsync(u => u.Email == model.Email && u.DeletedTime == null);
            if (exists)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Email đã tồn tại trong hệ thống.");

            foreach (var catId in model.ServiceCategoryIds)
            {
                var categoryExists = await _unitOfWork.GetRepository<ServiceCategory>().GetByIdAsync(catId);
                if (categoryExists == null)
                    throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, $"Danh mục {catId} không tồn tại.");
            }

            var request = _mapper.Map<RequestBecomeProvider>(model);
            request.Id = Guid.NewGuid();
            request.RequestStatus = "pending";
            request.CreatedTime = CoreHelper.SystemTimeNow;
            request.LastUpdatedTime = request.CreatedTime;

            // Lưu tạm email và fullname vào Description (hoặc bạn có thể mở rộng entity)
            request.Description ??= $"GUEST_REGISTER: {model.FullName} - {model.Email}";

            await _unitOfWork.GetRepository<RequestBecomeProvider>().InsertAsync(request);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<Guid>.Success(request.Id);
        }

        public async Task<BaseResponseModel<BasePaginatedList<GETRequestBecomeProviderModelView>>> GetAllAsync(string? requestStatus, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid pagination.");

            var query = _unitOfWork.GetRepository<RequestBecomeProvider>()
                .Entities
                .Where(r => r.DeletedTime == null);

            if (!string.IsNullOrEmpty(requestStatus))
            {
                requestStatus = requestStatus.ToLower();
                query = query.Where(r => r.RequestStatus.ToLower() == requestStatus);
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(r => r.CreatedTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mapped = _mapper.Map<List<GETRequestBecomeProviderModelView>>(items);
            return BaseResponseModel<BasePaginatedList<GETRequestBecomeProviderModelView>>.Success(
            new BasePaginatedList<GETRequestBecomeProviderModelView>(mapped, total, pageNumber, pageSize));
        }

        public async Task<BaseResponseModel<string>> ApproveRequestAsync(Guid requestId)
        {
            var requestRepo = _unitOfWork.GetRepository<RequestBecomeProvider>();
            var userRepo = _unitOfWork.GetRepository<ApplicationUsers>();
            var providerRepo = _unitOfWork.GetRepository<EntityServiceProvider>();
            var providerCategoryRepo = _unitOfWork.GetRepository<ServiceProviderCategory>();
            var workingHourRepo = _unitOfWork.GetRepository<WorkingHour>();
            var branchRepo = _unitOfWork.GetRepository<SpaBranchLocation>();
            var imageRepo = _unitOfWork.GetRepository<ServiceImage>();

            var request = await requestRepo.Entities
                .FirstOrDefaultAsync(r => r.Id == requestId && r.RequestStatus == "pending" && r.DeletedTime == null)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Request Not Found.");

            var user = await userRepo.GetByIdAsync(request.UserId)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "User Not Found.");

            if (user.ServiceProvider != null)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "User is already a provider.");

            // ✅ Cập nhật role
            var roleManager = _contextAccessor.HttpContext?.RequestServices.GetRequiredService<RoleManager<ApplicationRoles>>();
            var userManager = _contextAccessor.HttpContext?.RequestServices.GetRequiredService<UserManager<ApplicationUsers>>();

            var currentRoles = await userManager!.GetRolesAsync(user);
            if (currentRoles.Contains("Customer"))
            {
                await userManager.RemoveFromRoleAsync(user, "Customer");
            }
            if (!currentRoles.Contains("Provider"))
            {
                var roleExists = await roleManager!.RoleExistsAsync("Provider");
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new ApplicationRoles { Name = "Provider" });
                }
                await userManager.AddToRoleAsync(user, "Provider");
            }

            // ✅ Tạo ServiceProvider
            var provider = new EntityServiceProvider
            {
                Id = Guid.NewGuid(),
                BusinessName = request.BusinessName,
                PhoneNumber = request.PhoneNumber,
                Description = request.Description,
                ImageUrl = request.ImageUrl ?? "",
                ProviderId = user.Id,
                ContactFullName = user.UserName ?? "",
                ContactPosition = "Chủ cơ sở",
                Status = "approved",
                IsApproved = true,
                AverageRating = 0,
                TotalReviews = 0,
                OpenTime = request.OpenTime,
                CloseTime = request.CloseTime,
                MaxAppointmentsPerSlot = 5
            };
            await providerRepo.InsertAsync(provider);

            // ✅ Tạo chi nhánh chính nếu đủ thông tin
            if (!string.IsNullOrWhiteSpace(request.ProvinceId) &&
                !string.IsNullOrWhiteSpace(request.DistrictId) &&
                !string.IsNullOrWhiteSpace(request.AddressDetail))
            {
                // ✅ Gọi Esgoo API để xác thực và lấy tên
                var esgoo = _contextAccessor.HttpContext?.RequestServices.GetRequiredService<IEsgooService>();

                var province = await esgoo!.GetProvinceByIdAsync(request.ProvinceId)
                    ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Province not found");

                var district = await esgoo.GetDistrictByIdAsync(request.DistrictId, request.ProvinceId)
                    ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "District not found");

                var branch = new SpaBranchLocation
                {
                    Id = Guid.NewGuid(),
                    ServiceProviderId = provider.Id,
                    BranchName = "Cơ sở chính",
                    Street = request.AddressDetail ?? "",
                    District = district.name,
                    City = province.name,
                    Country = "Vietnam",
                    ProvinceId = request.ProvinceId,
                    DistrictId = request.DistrictId,
                    ProvinceName = province.name,
                    DistrictName = district.name,
                    PostalCode = request.PostalCode ?? "700000",
                    CreatedBy = CurrentUserId,
                    CreatedTime = CoreHelper.SystemTimeNow
                };

                await branchRepo.InsertAsync(branch);
            }

            // ✅ Gắn các danh mục
            var categoryIds = request.ServiceCategoryIds?.Split('|', StringSplitOptions.RemoveEmptyEntries).Select(Guid.Parse).ToList() ?? new();
            foreach (var catId in categoryIds)
            {
                await providerCategoryRepo.InsertAsync(new ServiceProviderCategory
                {
                    Id = Guid.NewGuid(),
                    ServiceProviderId = provider.Id,
                    ServiceCategoryId = catId
                });
            }

            // ✅ Giờ làm việc mặc định
            if (request.OpenTime.HasValue && request.CloseTime.HasValue)
            {
                await workingHourRepo.InsertAsync(new WorkingHour
                {
                    Id = Guid.NewGuid(),
                    ServiceProviderId = provider.Id,
                    DayOfWeek = 1,
                    OpeningTime = request.OpenTime.Value,
                    ClosingTime = request.CloseTime.Value,
                    IsWorking = true
                });
            }

            // ✅ Ảnh mô tả
            var descImages = request.DescriptionImages?.Split('|', StringSplitOptions.RemoveEmptyEntries);
            if (descImages != null)
            {
                foreach (var img in descImages)
                {
                    await imageRepo.InsertAsync(new ServiceImage
                    {
                        Id = Guid.NewGuid(),
                        ServiceProviderId = provider.Id,
                        ImageUrl = img
                    });
                }
            }

            // ✅ Cập nhật trạng thái yêu cầu
            request.RequestStatus = "approved";
            request.LastUpdatedBy = CurrentUserId;
            request.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await requestRepo.UpdateAsync(request);
            await _unitOfWork.SaveAsync();

            // ✅ Gửi email xác nhận
            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                var subject = "Yêu cầu trở thành nhà cung cấp đã được duyệt";
                var body = $@"
                <p>Xin chào <strong>{user.UserName}</strong>,</p>
                <p>Chúc mừng! Yêu cầu trở thành nhà cung cấp của bạn trên hệ thống <strong>ZENORA</strong> đã được <strong>phê duyệt</strong>.</p>
                <p>Bạn đã có thể đăng nhập và cập nhật thêm thông tin về dịch vụ, lịch làm việc, hình ảnh,... trong trang quản lý.</p>
                <p>Nếu có bất kỳ thắc mắc nào, vui lòng liên hệ với đội ngũ hỗ trợ của chúng tôi.</p>
                <p>Trân trọng,<br/>Đội ngũ ZENORA (An Ngu nè)</p>";

                    await _emailService.SendEmailAsync(user.Email, subject, body);
                }

            return BaseResponseModel<string>.Success("Đã duyệt yêu cầu, cấp quyền Provider, tạo chi nhánh và gửi email thông báo.");
        }

        // ✅ Thêm phương thức riêng cho guest để duyệt request và tạo tài khoản provider mới
        public async Task<BaseResponseModel<string>> ApproveGuestRequestAsync(Guid requestId)
        {
            var requestRepo = _unitOfWork.GetRepository<RequestBecomeProvider>();
            var userRepo = _unitOfWork.GetRepository<ApplicationUsers>();
            var providerRepo = _unitOfWork.GetRepository<EntityServiceProvider>();
            var providerCategoryRepo = _unitOfWork.GetRepository<ServiceProviderCategory>();
            var workingHourRepo = _unitOfWork.GetRepository<WorkingHour>();
            var branchRepo = _unitOfWork.GetRepository<SpaBranchLocation>();
            var imageRepo = _unitOfWork.GetRepository<ServiceImage>();

            var request = await requestRepo.Entities
                .FirstOrDefaultAsync(r => r.Id == requestId && r.RequestStatus == "pending" && r.DeletedTime == null)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Request Not Found.");

            // Nếu request đã có UserId thì dùng phương thức ApproveRequestAsync gốc
            if (request.UserId != Guid.Empty)
                return await ApproveRequestAsync(requestId);

            // ✅ Tạo tài khoản mới
            var userManager = _contextAccessor.HttpContext?.RequestServices.GetRequiredService<UserManager<ApplicationUsers>>();
            var roleManager = _contextAccessor.HttpContext?.RequestServices.GetRequiredService<RoleManager<ApplicationRoles>>();

            var newUser = new ApplicationUsers
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                UserName = request.Email,
                Status = "active",
                CreatedTime = CoreHelper.SystemTimeNow,
                LastUpdatedTime = CoreHelper.SystemTimeNow
            };

            var randomPassword = CoreHelper.GenerateRandomPassword(8);
            var createResult = await userManager.CreateAsync(newUser, randomPassword);
            if (!createResult.Succeeded)
                throw new ErrorException(500, ErrorCode.InternalServerError, string.Join("; ", createResult.Errors.Select(e => e.Description)));

            if (!await roleManager.RoleExistsAsync("Provider"))
                await roleManager.CreateAsync(new ApplicationRoles { Name = "Provider" });
            await userManager.AddToRoleAsync(newUser, "Provider");

            request.UserId = newUser.Id;

            // ✅ Tạo provider
            var provider = new EntityServiceProvider
            {
                Id = Guid.NewGuid(),
                BusinessName = request.BusinessName,
                PhoneNumber = request.PhoneNumber,
                Description = request.Description,
                ImageUrl = request.ImageUrl ?? "",
                ProviderId = newUser.Id,
                ContactFullName = request.FullName,
                ContactPosition = "Chủ cơ sở",
                Status = "approved",
                IsApproved = true,
                AverageRating = 0,
                TotalReviews = 0,
                OpenTime = request.OpenTime,
                CloseTime = request.CloseTime,
                MaxAppointmentsPerSlot = 5
            };
            await providerRepo.InsertAsync(provider);

            // ✅ Tạo chi nhánh
            if (!string.IsNullOrWhiteSpace(request.ProvinceId) &&
                !string.IsNullOrWhiteSpace(request.DistrictId) &&
                !string.IsNullOrWhiteSpace(request.AddressDetail))
            {
                var esgoo = _contextAccessor.HttpContext?.RequestServices.GetRequiredService<IEsgooService>();
                var province = await esgoo!.GetProvinceByIdAsync(request.ProvinceId)
                    ?? throw new ErrorException(404, ErrorCode.NotFound, "Province not found");
                var district = await esgoo.GetDistrictByIdAsync(request.DistrictId, request.ProvinceId)
                    ?? throw new ErrorException(404, ErrorCode.NotFound, "District not found");

                await branchRepo.InsertAsync(new SpaBranchLocation
                {
                    Id = Guid.NewGuid(),
                    ServiceProviderId = provider.Id,
                    BranchName = "Cơ sở chính",
                    Street = request.AddressDetail ?? "",
                    ProvinceId = request.ProvinceId,
                    DistrictId = request.DistrictId,
                    ProvinceName = province.name,
                    DistrictName = district.name,
                    Country = "Vietnam",
                    City = province.name,
                    District = district.name,
                    PostalCode = request.PostalCode ?? "700000",
                    CreatedBy = CurrentUserId,
                    CreatedTime = CoreHelper.SystemTimeNow
                });
            }

            // ✅ Gắn danh mục
            var categoryIds = request.ServiceCategoryIds?.Split('|', StringSplitOptions.RemoveEmptyEntries).Select(Guid.Parse).ToList() ?? new();
            foreach (var catId in categoryIds)
            {
                await providerCategoryRepo.InsertAsync(new ServiceProviderCategory
                {
                    Id = Guid.NewGuid(),
                    ServiceProviderId = provider.Id,
                    ServiceCategoryId = catId
                });
            }

            // ✅ Cập nhật trạng thái
            request.RequestStatus = "approved";
            request.LastUpdatedBy = CurrentUserId;
            request.LastUpdatedTime = CoreHelper.SystemTimeNow;
            await requestRepo.UpdateAsync(request);
            await _unitOfWork.SaveAsync();

            // ✅ Gửi email xác nhận
            await _emailService.SendEmailAsync(newUser.Email, "Tài khoản nhà cung cấp đã được tạo", $"""
                <p>Chúc mừng {request.FullName}, yêu cầu của bạn đã được duyệt.</p>
                <p>Bạn đã được cấp tài khoản với thông tin sau:</p>
                <ul>
                    <li>Email: <strong>{newUser.Email}</strong></li>
                    <li>Mật khẩu: <strong>{randomPassword}</strong></li>
                </ul>
                <p>Hãy đăng nhập vào hệ thống và cập nhật hồ sơ spa của bạn.</p>
            """);

            return BaseResponseModel<string>.Success("Đã duyệt yêu cầu và cấp tài khoản mới cho Provider.");
        }


        public async Task<BaseResponseModel<string>> RejectRequestAsync(Guid requestId, string reason)
        {
            var repo = _unitOfWork.GetRepository<RequestBecomeProvider>();
            var userRepo = _unitOfWork.GetRepository<ApplicationUsers>();

            var request = await repo.Entities
                .FirstOrDefaultAsync(r => r.Id == requestId && r.RequestStatus == "pending" && r.DeletedTime == null)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Request not found.");

            var user = await userRepo.GetByIdAsync(request.UserId)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "User not found.");

            request.RequestStatus = "rejected";
            request.RejectedReason = reason;
            request.LastUpdatedBy = CurrentUserId;
            request.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await repo.UpdateAsync(request);
            await _unitOfWork.SaveAsync();

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                var subject = "Yêu cầu trở thành nhà cung cấp đã bị từ chối";
                var body = $@"
                <p>Xin chào <strong>{user.UserName}</strong>,</p>
                <p>Chúng tôi rất tiếc phải thông báo rằng yêu cầu trở thành nhà cung cấp của bạn đã bị <strong>từ chối</strong>.</p>
                <p><strong>Lý do:</strong> {reason}</p>
                <p>Nếu bạn có thắc mắc, vui lòng liên hệ với bộ phận hỗ trợ của chúng tôi.</p>
                <p>Trân trọng,<br/>Đội ngũ ZENORA</p>";

                await _emailService.SendEmailAsync(user.Email, subject, body);
            }

            return BaseResponseModel<string>.Success("Request rejected and email sent.");
        }
    }
}
