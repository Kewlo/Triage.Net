using System;

namespace Triage.Api.Domain
{
    public abstract class Event
    {
        protected Event()
        {
            Date = DateTime.Now;
        }
        public string Title { get; set; }
        public DateTime Date { get; set; }
    }
}