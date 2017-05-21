namespace Neutrino.Core
{
    public class ActionConfirmation
    {
        public bool WasSeccessful { get; protected set; }

        public string Message { get; protected set; }

        public static ActionConfirmation CreateSuccessful()
        {
            return new ActionConfirmation { WasSeccessful = true };
        }

        public static ActionConfirmation CreateError(string message)
        {
            return new ActionConfirmation { WasSeccessful = false, Message = message };
        }
    }

    public class ActionConfirmation<T> : ActionConfirmation
    {
        public T ObjectData { get; private set; }

        public static ActionConfirmation CreateSuccessful(T objectData)
        {
            return new ActionConfirmation<T> { WasSeccessful = true, ObjectData = objectData };
        }
    }
}