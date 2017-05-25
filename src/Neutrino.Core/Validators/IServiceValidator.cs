using Neutrino.Entities;

namespace Neutrino.Core.Services.Validators
{
    public interface IServiceValidator
    {
        ActionConfirmation Validate(Service service, ActionType actionType);
    }
}