using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;
using Triage.Api.Domain.Diagnostics;
using Triage.Api.Domain.Messages;
using Triage.Api.Domain.Messages.Aggregates;
using Triage.Business;

namespace Triage.Web.Api.Controllers
{
    public class LogController : ApiController
    {
        private readonly IEventLogBusiness _eventLogBusiness;

        public LogController(IEventLogBusiness eventLogBusiness)
        {
            _eventLogBusiness = eventLogBusiness;
        }

        [HttpGet]
        public string Index()
        {
            return "Hello";
        }

        [HttpGet]
        public string ClearErrors()
        {
            _eventLogBusiness.ClearErrorMessages();
            return "Done";
        }
        [HttpGet]
        public Response Error(string id)
        {
            _eventLogBusiness.LogError(new ErrorMessage
            {
                Title = id +  " - " + DateTime.Now.Millisecond + " ms",
                StackTrace = id + " - Stack location"
            });

            return new Response { Success = true };
        }

        [HttpGet]
        public Response Warning(string id)
        {
            _eventLogBusiness.LogWarning(new Message
            {
                Title = id,
            });

            return new Response { Success = true };
        }

        [HttpGet]
        public Response Info(string id)
        {
            _eventLogBusiness.LogMessage(new Message
            {
                Title = id,
            });

            return new Response {Success = true};
        }

        [HttpGet]
        public void Measure()
        {
            _eventLogBusiness.LogMeasure(new Measure
            {
                Title = "Test Measure",
                Value = DateTime.Now.Millisecond
            });
        }

        [HttpGet]
        public double Setup(int count = 1000)
        {
            var timer = Stopwatch.StartNew();
            _eventLogBusiness.Setup(count);
            timer.Stop();

            return timer.Elapsed.TotalSeconds;
        }

        [HttpGet]
        public IEnumerable<MeasureSummary> MeasureSummary()
        {
            return _eventLogBusiness.GetSummary();
        }
        [HttpGet]
        public IEnumerable<MessageViewModel> Messages()
        {
            return _eventLogBusiness.GetMessages()
                .Select(message => new MessageViewModel
                {
                    Id = message.Id,
                    Title  = message.Title,
                    Date = message.Date,
                    Type = message.Type
                });
        }

        [HttpGet]
        public IEnumerable<ErrorMessagesBySource> ErrorMessages()
        {
            return _eventLogBusiness.GetErrorsBySource();

        }
        [HttpGet]
        public IList<Measure> Measures()
        {
            return _eventLogBusiness.GetMeasures();
        }
    }

    public class MessageViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public MessageType Type { get; set; }
    }

    public class Response
    {
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
