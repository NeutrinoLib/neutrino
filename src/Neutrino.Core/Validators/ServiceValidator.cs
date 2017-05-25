using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Neutrino.Core.Repositories;
using Neutrino.Entities;

namespace Neutrino.Core.Services.Validators
{
    public class ServiceValidator : IServiceValidator
    {
        public readonly IRepository<Service> _serviceRepository;

        public ServiceValidator(IRepository<Service> serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        public ActionConfirmation Validate(Service service, ActionType actionType)
        {
            var errors = new List<ValidationError>();
            
            var idPattern = new Regex("^[a-zA-Z0-9-]*$");
            if(!idPattern.IsMatch(service.Id))
            {
                errors.Add(new ValidationError 
                { 
                    FieldName = nameof(service.Id), 
                    Message = $"Service id contains unacceptable characters (only alphanumeric letters and dash is acceptable)." 
                });
            }

            if(actionType == ActionType.Create && _serviceRepository.Get(service.Id) != null)
            {
                errors.Add(new ValidationError 
                { 
                    FieldName = nameof(service.Id), 
                    Message = $"Service with id '{service.Id}' already exists." 
                });
            }

            if(string.IsNullOrWhiteSpace(service.Id))
            {
                errors.Add(new ValidationError 
                { 
                    FieldName = nameof(service.Id), 
                    Message = "Service id wasn't specified" 
                });
            }

            if(string.IsNullOrWhiteSpace(service.ServiceType))
            {
                errors.Add(new ValidationError 
                { 
                    FieldName = nameof(service.ServiceType), 
                    Message = "Service type wasn't specified" 
                });
            }

            if(string.IsNullOrWhiteSpace(service.Address))
            {
                errors.Add(new ValidationError 
                { 
                    FieldName = nameof(service.Address), 
                    Message = "Service address wasn't specified" 
                });
            }

            if(errors.Any())
            {
                return ActionConfirmation.CreateError("There was an validation errors.", errors.ToArray());
            }

            return ActionConfirmation.CreateSuccessful();
        }
    }
}