using System.Collections.Generic;
using System.Linq;
using Neutrino.Core.Repositories;
using Neutrino.Entities.Model;
using Neutrino.Entities.Response;

namespace Neutrino.Core.Services.Validators
{
    public class KvPropertyValidator : IKvPropertyValidator
    {
        private readonly IRepository<KvProperty> _kvPropertyRepository;

        public KvPropertyValidator(IRepository<KvProperty> kvPropertyRepository)
        {
            _kvPropertyRepository = kvPropertyRepository;
        }

        public ActionConfirmation Validate(KvProperty kvProperty, ActionType actionType)
        {
            var errors = new List<ValidationError>();

            if(string.IsNullOrWhiteSpace(kvProperty.Key))
            {
                errors.Add(new ValidationError 
                { 
                    FieldName = nameof(kvProperty.Id), 
                    Message = "K/V property key wasn't specified" 
                });
            }

            if(actionType == ActionType.Create && _kvPropertyRepository.Get(kvProperty.Key) != null)
            {
                errors.Add(new ValidationError 
                { 
                    FieldName = nameof(kvProperty.Key), 
                    Message = $"K/V property with id '{kvProperty.Key}' already exists." 
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