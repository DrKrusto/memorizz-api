﻿using MediatR;
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
    [HttpGet("{userId}"), Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<EntryView>>> GetEntries([FromRoute] string userId, [FromQuery] GetEntriesInput input, CancellationToken cancellationToken)
    {
        var command = new GetEntriesQuery(userId, input.From, input.To);
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response.Select(e => EntryView.From(e, userResolver)).OrderBy(x => x.Date));
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
        return Ok(EntryView.From(response, userResolver));
    }
}