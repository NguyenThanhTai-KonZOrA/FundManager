namespace BreakFastCheckIn.Implement.Services.Interface
{
    public interface IPropertyOutletService
    {
        Task<IEnumerable<int>> ResolvePropertyIdsAsync(int mainPropertyId);
    }
}