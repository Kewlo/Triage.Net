namespace Triage.Api.Domain.Messages
{
    public class Message : Event
    {
        public virtual string Id { get; set; }
        public virtual MessageType Type { get; set; }
        public dynamic Data { get; set; }
    }
}