using Neutrino.Entities.Model;
using Neutrino.Entities.Response;

namespace Neutrino.Core.Services.Validators
{
    public interface IKvPropertyValidator
    {
        ActionConfirmation Validate(KvProperty kvProperty, ActionType actionType);
    }
}