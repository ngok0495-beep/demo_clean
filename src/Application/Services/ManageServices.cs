using Domain;

namespace Application;

public record ServiceDetailsDto(int Id, string Name, decimal Price, string Description, string Status);
public record SaveServiceRequest(string? Name, decimal Price, string? Description, string? Status);

public interface IServiceCatalogRepository
{
    Task<IReadOnlyList<Service>> ListAsync(string? status);
    Task<Service?> FindAsync(int id);
    Task<bool> NameExistsAsync(string name, int? exceptId);
    Task<bool> HasUsageAsync(int id);
    Task AddAsync(Service service);
    Task SaveAsync();
    Task DeleteAsync(Service service);
}

public interface IManageServicesInputBoundary
{
    Task ListAsync(string? status);
    Task GetAsync(int id);
    Task CreateAsync(SaveServiceRequest request);
    Task UpdateAsync(int id, SaveServiceRequest request);
    Task DeleteAsync(int id);
}

public interface IManageServicesOutputBoundary
{
    void PresentServices(IReadOnlyList<ServiceDetailsDto> services);
    void PresentService(ServiceDetailsDto service, bool created = false);
    void PresentDeleted();
    void PresentError(ErrorKind kind, string message);
}

public class ManageServicesUseCase(IServiceCatalogRepository services, IManageServicesOutputBoundary output)
    : IManageServicesInputBoundary
{
    private static ServiceDetailsDto Map(Service service) =>
        new(service.Id, service.Name, service.Price, service.Description, service.Status);

    private async Task ExecuteAsync(Func<Task> action)
    {
        try { await action(); }
        catch (AppException ex) { output.PresentError(ex.Kind, ex.Message); }
    }

    private static string Validate(SaveServiceRequest request)
    {
        var name = request.Name?.Trim() ?? "";
        if (name.Length is < 1 or > 100)
            throw new AppException(ErrorKind.Validation, "Tên dịch vụ phải có từ 1 đến 100 ký tự.");
        if (request.Price <= 0)
            throw new AppException(ErrorKind.Validation, "Đơn giá dịch vụ phải lớn hơn 0.");
        if ((request.Description?.Length ?? 0) > 500)
            throw new AppException(ErrorKind.Validation, "Mô tả dịch vụ không được vượt quá 500 ký tự.");
        if (request.Status is not ("Active" or "Inactive"))
            throw new AppException(ErrorKind.Validation, "Trạng thái dịch vụ không hợp lệ.");
        return name;
    }

    private async Task<Service> FindAsync(int id) => await services.FindAsync(id)
        ?? throw new AppException(ErrorKind.NotFound, "Không tìm thấy dịch vụ.");

    public Task ListAsync(string? status) => ExecuteAsync(async () =>
    {
        if (status is not null && status is not ("Active" or "Inactive"))
            throw new AppException(ErrorKind.Validation, "Trạng thái dịch vụ không hợp lệ.");
        output.PresentServices((await services.ListAsync(status)).Select(Map).ToList());
    });

    public Task GetAsync(int id) => ExecuteAsync(async () => output.PresentService(Map(await FindAsync(id))));

    public Task CreateAsync(SaveServiceRequest request) => ExecuteAsync(async () =>
    {
        var name = Validate(request);
        if (await services.NameExistsAsync(name, null))
            throw new AppException(ErrorKind.Conflict, "Tên dịch vụ đã tồn tại.");
        var service = new Service
        {
            Name = name,
            Price = request.Price,
            Description = request.Description ?? "",
            Status = request.Status!
        };
        await services.AddAsync(service);
        output.PresentService(Map(await FindAsync(service.Id)), created: true);
    });

    public Task UpdateAsync(int id, SaveServiceRequest request) => ExecuteAsync(async () =>
    {
        var service = await FindAsync(id);
        var name = Validate(request);
        if (await services.NameExistsAsync(name, id))
            throw new AppException(ErrorKind.Conflict, "Tên dịch vụ đã tồn tại.");
        service.Name = name;
        service.Price = request.Price;
        service.Description = request.Description ?? "";
        service.Status = request.Status!;
        await services.SaveAsync();
        output.PresentService(Map(await FindAsync(id)));
    });

    public Task DeleteAsync(int id) => ExecuteAsync(async () =>
    {
        var service = await FindAsync(id);
        if (await services.HasUsageAsync(id))
            throw new AppException(ErrorKind.Conflict, "Không thể xóa dịch vụ đã được sử dụng. Hãy chuyển trạng thái dịch vụ sang Inactive.");
        await services.DeleteAsync(service);
        output.PresentDeleted();
    });
}
