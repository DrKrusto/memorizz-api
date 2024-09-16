using MediatR;
using Memorizz.Host.Controllers.Inputs;
using Memorizz.Host.Controllers.Views;
using Memorizz.Host.Domain.Commands;
using Memorizz.Host.Domain.Queries;
using Memorizz.Host.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Memorizz.Host.Controllers;

/// <summary>
/// Entry controller
/// </summary>
/// <param name="mediator"></param>
[ApiController]
[Route("/entry")]
public class EntryController(ISender mediator, IUserResolver userResolver) : ControllerBase
{
    /// <summary>
    /// Get journal entries of a user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("all/{userId}"), Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EntryListView>> GetEntries([FromRoute] string userId, [FromQuery] GetEntriesInput input, CancellationToken cancellationToken)
    {
        var command = new GetEntriesQuery(userId, input.From, input.To);
        var response = await mediator.Send(command, cancellationToken);
        return response.ToActionResult(x => EntryListView.From(x, userResolver, userId));
    }
    
    /// <summary>
    /// Get a specific journal entry
    /// </summary>
    /// <param name="entryId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{entryId}"), Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EntryView>> GetEntry([FromRoute] string entryId, CancellationToken cancellationToken)
    {
        var command = new GetEntryQuery(entryId);
        var response = await mediator.Send(command, cancellationToken);
        return response.ToActionResult(x => EntryView.From(x, userResolver));
    }
    
    /// <summary>
    /// Upsert a journal entry
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost, Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<EntryView>> UpsertEntry([FromBody] UpsertEntryInput input, CancellationToken cancellationToken)
    {
        var command = new UpsertEntry(input.UserId, input.Date, input.Content);
        var response = await mediator.Send(command, cancellationToken);
        return response.ToActionResult(x => EntryView.From(x, userResolver));
    }
}