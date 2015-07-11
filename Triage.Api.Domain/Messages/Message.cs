namespace Triage.Api.Domain.Messages
{
    public class Message : Event
    {
        public virtual MessageType Type { get; set; }
        public dynamic Data { get; set; }
    }
}