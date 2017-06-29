using Neutrino.Entities;

namespace Neutrino.Core.Services.Validators
{
    public class KvPropertyValidator : IKvPropertyValidator
    {
        public ActionConfirmation Validate(KvProperty kvProperty, ActionType actionType)
        {
            return ActionConfirmation.CreateSuccessful();
        }
    }
}