using Application;
using Microsoft.AspNetCore.Mvc;

namespace Api.Presenters;

public class ManageServicesPresenter : IManageServicesOutputBoundary
{
    public IActionResult? ViewModel { get; private set; }
    public void PresentServices(IReadOnlyList<ServiceDetailsDto> services) => ViewModel = new OkObjectResult(services);
    public void PresentService(ServiceDetailsDto service, bool created = false) => ViewModel = created
        ? new CreatedResult($"/api/services/{service.Id}", service) : new OkObjectResult(service);
    public void PresentDeleted() => ViewModel = new NoContentResult();
    public void PresentError(ErrorKind kind, string message) => ViewModel = ProblemResults.From(kind, message);
}
