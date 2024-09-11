namespace Memorizz.Host.Controllers.Inputs;

public record UpsertEntryInput(string UserId, string Content, DateOnly Date);