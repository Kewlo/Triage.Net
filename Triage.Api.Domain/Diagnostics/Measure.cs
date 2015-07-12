namespace Triage.Api.Domain.Diagnostics
{
    public class Measure : Event
    {
        public decimal Value { get; set; }
    }
}