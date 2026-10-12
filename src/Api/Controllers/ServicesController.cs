using Api.Presenters;
using Application;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/services")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
public class ServicesController(IManageServicesInputBoundary useCase, ManageServicesPresenter presenter) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ServiceDetailsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] string? status)
    {
        await useCase.ListAsync(status);
        return presenter.ViewModel!;
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ServiceDetailsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(int id)
    {
        await useCase.GetAsync(id);
        return presenter.ViewModel!;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ServiceDetailsDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(SaveServiceBody request)
    {
        await useCase.CreateAsync(request.ToRequest());
        return presenter.ViewModel!;
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ServiceDetailsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(int id, SaveServiceBody request)
    {
        await useCase.UpdateAsync(id, request.ToRequest());
        return presenter.ViewModel!;
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id)
    {
        await useCase.DeleteAsync(id);
        return presenter.ViewModel!;
    }
}
