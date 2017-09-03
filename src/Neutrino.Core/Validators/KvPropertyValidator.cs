using Neutrino.Entities.Model;
using Neutrino.Entities.Response;

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