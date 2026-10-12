using Api.Presenters;
using Application;
using Domain;
using Microsoft.AspNetCore.Mvc;
namespace Api.Controllers;

[ApiController]
[Route("api/rooms")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
public class RoomManagementController(IManageRoomsInputBoundary useCase, ManageRoomsPresenter presenter) : ControllerBase
{
    // Includes maintenance rooms. GET /api/rooms remains the team's availability search.
    [HttpGet("management")]
    [ProducesResponseType(typeof(IReadOnlyList<RoomDetailsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] RoomStatus? status, [FromQuery] int? roomTypeId)
    {
        await useCase.ListAsync(status, roomTypeId);
        return presenter.ViewModel!;
    }

    [HttpGet("types")]
    [ProducesResponseType(typeof(IReadOnlyList<RoomTypeOptionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Types()
    {
        await useCase.ListTypesAsync();
        return presenter.ViewModel!;
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RoomDetailsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(int id)
    {
        await useCase.GetAsync(id);
        return presenter.ViewModel!;
    }

    [HttpPost]
    [ProducesResponseType(typeof(RoomDetailsDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(SaveRoomBody request)
    {
        await useCase.CreateAsync(request.ToRequest());
        return presenter.ViewModel!;
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(RoomDetailsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(int id, SaveRoomBody request)
    {
        await useCase.UpdateAsync(id, request.ToRequest());
        return presenter.ViewModel!;
    }

    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(RoomDetailsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ChangeStatus(int id, ChangeRoomStatusBody request)
    {
        await useCase.ChangeStatusAsync(id, new ChangeRoomStatusRequest(request.Status));
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
