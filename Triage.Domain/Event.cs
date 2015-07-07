using System;

namespace Triage.Domain
{
    public class Event
    {
        public Event()
        {
            Date = DateTime.Now;
        }

        public DateTime Date { get; set; }
        public string Title { get; set; }
    }
}
