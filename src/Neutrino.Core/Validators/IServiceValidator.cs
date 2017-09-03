using Neutrino.Entities.Model;
using Neutrino.Entities.Response;

namespace Neutrino.Core.Services.Validators
{
    public interface IServiceValidator
    {
        ActionConfirmation Validate(Service service, ActionType actionType);
    }
}