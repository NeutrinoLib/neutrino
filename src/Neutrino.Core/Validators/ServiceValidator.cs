using System.Collections.Generic;
using System.Linq;
using Neutrino.Entities;

namespace Neutrino.Core.Services.Validators
{
    public class ServiceValidator : IServiceValidator
    {
        public ActionConfirmation Validate(Service service)
        {
            var errors = new List<ValidationError>();
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