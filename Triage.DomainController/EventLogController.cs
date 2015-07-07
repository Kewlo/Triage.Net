using System.Collections.Generic;
using System.Linq;
using Triage.Api.Domain;
using Triage.Persistence.Context;

namespace Triage.DomainController
{
    public interface IEventLogController
    {
        void LogError(ErrorMessage errorMessage);
        void LogWarning(Message message);
        void LogMessage(Message message);
        void LogMeasure(Measure measure);
        IList<Message> GetMessages();
        IList<Measure> GetMeasures();
    }

    public class EventLogController: IEventLogController
    {
        private readonly ITriageDbContextFactory _triageDbContextFactory;

        public EventLogController(ITriageDbContextFactory triageDbContextFactory)
        {
            _triageDbContextFactory = triageDbContextFactory;
        }

        public void LogError(ErrorMessage errorMessage)
        {
            errorMessage.Type = MessageType.Error;
            using (var dbContext = _triageDbContextFactory.CreateTriageDbContext())
            {
                dbContext.AddEntity((Message)errorMessage);
                dbContext.SaveChanges();
            }
        }

        public void LogWarning(Message message)
        {
            message.Type = MessageType.Warning;
            using (var dbContext = _triageDbContextFactory.CreateTriageDbContext())
            {
                dbContext.AddEntity(message);
                dbContext.SaveChanges();
            }
        }

        public void LogMessage(Message message)
        {
            message.Type = MessageType.Info;
            using (var dbContext = _triageDbContextFactory.CreateTriageDbContext())
            {
                dbContext.AddEntity(message);
                dbContext.SaveChanges();
            }
        }

        public void LogMeasure(Measure measure)
        {
            using (var dbContext = _triageDbContextFactory.CreateTriageDbContext())
            {
                dbContext.AddEntity(measure);
                dbContext.SaveChanges();
            }
        }

        public IList<Message> GetMessages()
        {
            using (var dbContext = _triageDbContextFactory.CreateTriageDbContext())
            {
                return dbContext
                    .Query<Message>()
                    .ToList();
            }
        }

        public IList<Measure> GetMeasures()
        {
            using (var dbContext = _triageDbContextFactory.CreateTriageDbContext())
            {
                return dbContext
                    .Query<Measure>()
                    .ToList();
            }
        }
    }

}