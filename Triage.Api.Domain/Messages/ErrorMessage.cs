namespace Triage.Api.Domain.Messages
{
    public class ErrorMessage : Message
    {
        public override MessageType Type { get { return MessageType.Error;} }
        public string Message { get; set; }
        public string ErrorType { get; set; }
        public string StackTrace { get; set; }
        public dynamic Context { get; set; }

    }
}