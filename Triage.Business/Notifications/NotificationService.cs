using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Triage.Business.Messages;
using Triage.Persistence.Context;

namespace Triage.Business.Notifications
{

    public interface INotificationService
    {
        void Start();
        void Stop();
    }

    public class NotificationService : INotificationService
    {
        private readonly ILogHub _logHub;
        private readonly IEventLogBusiness _eventLogBusiness;
        private Timer _timer;
        private DateTime? _lastNotifyTime;

        public NotificationService(ILogHub logHub, IEventLogBusiness eventLogBusiness)
        {
            _logHub = logHub;
            _eventLogBusiness = eventLogBusiness;
        }

        public void Start()
        {
            _timer = new Timer(Notify, null, 60, 30000);
        }

        private void Notify(object state)
        {
            var errorsSinceLastCheck = _eventLogBusiness.GetErrorMessages(_lastNotifyTime);
            //_logHub.Notify();
            if (errorsSinceLastCheck.Any() == false)
            {
                return;
            }

            var currentHourErrors = _eventLogBusiness.GetErrorsBySource(DateTime.Now.Date, DateTime.Now.Hour);
            _logHub.HourlyErrorUpdate(currentHourErrors);
            
            //var lastHour = DateTime.Now.AddHours(-1);
            //var lastHourErrors = _eventLogBusiness.GetErrorsBySource(lastHour, lastHour.Hour);

            _lastNotifyTime = DateTime.Now;
        }

        public void Stop()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;

            }
        }
    }
}