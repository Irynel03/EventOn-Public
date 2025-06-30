namespace EventOn.Interfaces;

public interface IOneSignalService
{
    Task<List<string>> GetPlayerIdsByExternalIdAsync(string externalUserId);
}