using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Triage.Api.Domain;
using Triage.DomainController;

namespace Triage.Web.Api.Controllers
{
    public class LogController : ApiController
    {
        private readonly IEventLogController _eventLogController;

        public LogController(IEventLogController eventLogController)
        {
            _eventLogController = eventLogController;
        }

        [HttpGet]
        public string Index()
        {
            return "Hello";
        }

        [HttpGet]
        public Response Error(string id)
        {
            _eventLogController.LogError(new ErrorMessage
            {
                Title = id,
            });

            return new Response { Success = true };
        }

        [HttpGet]
        public Response Warning(string id)
        {
            _eventLogController.LogWarning(new Message
            {
                Title = id,
            });

            return new Response { Success = true };
        }

        [HttpGet]
        public Response Info(string id)
        {
            _eventLogController.LogMessage(new Message
            {
                Title = id,
            });

            return new Response {Success = true};
        }

        [HttpGet]
        public void Measure()
        {
            _eventLogController.LogMeasure(new Measure
            {
                Title = "Test Measure",
                Value = DateTime.Now.Millisecond
            });
        }

        [HttpGet]
        public IEnumerable<MessageViewModel> Messages()
        {
            return _eventLogController.GetMessages()
                .Select(message => new MessageViewModel
                {
                    Title  = message.Title,
                    Date = message.Date,
                    Type = message.Type
                });
        }

        [HttpGet]
        public IList<Measure> Measures()
        {
            return _eventLogController.GetMeasures();
        }
    }

    public class MessageViewModel
    {
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
