using Neutrino.Entities;

namespace Neutrino.Core.Services.Validators
{
    public interface IKvPropertyValidator
    {
        ActionConfirmation Validate(KvProperty kvProperty, ActionType actionType);
    }
}