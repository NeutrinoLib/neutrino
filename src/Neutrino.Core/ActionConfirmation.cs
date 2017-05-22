using System.Collections.Generic;
using System.Linq;

namespace Neutrino.Core
{
    public class ActionConfirmation
    {
        public bool WasSuccessful { get; protected set; }

        public string Message { get; protected set; }

        public ValidationError[] Errors { get; protected set; }

        protected ActionConfirmation()
        {
        }

        public static ActionConfirmation CreateSuccessful()
        {
            return new ActionConfirmation { WasSuccessful = true };
        }

        public static ActionConfirmation CreateError(string message)
        {
            return new ActionConfirmation { WasSuccessful = false, Message = message };
        }

        public static ActionConfirmation CreateError(string message, ValidationError[] errors)
        {
            return new ActionConfirmation { WasSuccessful = false, Message = message, Errors = errors };
        }
    }

    public class ActionConfirmation<T> : ActionConfirmation
    {
        public T ObjectData { get; private set; }

        public static ActionConfirmation CreateSuccessful(T objectData)
        {
            return new ActionConfirmation<T> { WasSuccessful = true, ObjectData = objectData };
        }
    }
}