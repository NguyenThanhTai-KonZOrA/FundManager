using BreakFastCheckIn.Implement.Services.Interface;
using FundManager.Implement.Repositories.Interface;

namespace BreakFastCheckIn.Implement.Services
{
    public class PropertyOutletService : IPropertyOutletService
    {
        private readonly IPropertyOutletRepository _repository;
        public PropertyOutletService(IPropertyOutletRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Resolve full set of propertyIds to query: starts with the main propertyId,
        /// then adds any other propertyIds that share an outlet with the main property.
        /// Example: propertyId=1 maps to outlet 1 and 2; outlet 2 also maps to propertyId=2
        /// → returns [1, 2].
        /// </summary>
        public async Task<IEnumerable<int>> ResolvePropertyIdsAsync(int mainPropertyId)
        {
            var ids = new HashSet<int> { mainPropertyId };

            var mainMappings = await _repository.GetByPropertyIdAsync(mainPropertyId);
            foreach (var mapping in mainMappings)
            {
                var outletMappings = await _repository.GetByOutletIdAsync(mapping.OutletId);
                foreach (var om in outletMappings)
                    ids.Add(om.PropertyId);
            }

            return ids;
        }
    }
}