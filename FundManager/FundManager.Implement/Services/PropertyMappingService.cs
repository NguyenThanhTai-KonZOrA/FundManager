using FundManager.Implement.Services.Interface;
using FundManager.Implement.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace FundManager.Implement.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PropertyMappingService> _logger;

        public PropertyMappingService(IUnitOfWork unitOfWork, ILogger<PropertyMappingService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
    }
}