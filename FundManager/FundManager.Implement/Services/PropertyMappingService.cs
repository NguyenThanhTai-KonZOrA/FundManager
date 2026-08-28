using DigitalDocumentPlatform.Implement.Services.Interface;
using DigitalDocumentPlatform.Implement.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace DigitalDocumentPlatform.Implement.Services
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