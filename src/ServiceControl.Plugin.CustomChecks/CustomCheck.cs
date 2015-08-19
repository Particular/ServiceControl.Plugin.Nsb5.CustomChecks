namespace ServiceControl.Plugin.CustomChecks
{
    using System;
    using System.Threading;
    using Internal;
    using NServiceBus;
    using Messages;
    using NServiceBus.ObjectBuilder;
    using NServiceBus.Transports;
    using NServiceBus.Unicast;

    public abstract class CustomCheck : ICustomCheck
    {
        public Configure Configure { get; set; }
        public UnicastBus UnicastBus { get; set; }
        public CriticalError CriticalError { get; set; }


        public IBuilder Builder { get; set; }
        
        protected CustomCheck(string id, string category)
        {
            Category = category;
            Id = id;
        }

        public string Category { get; private set; }

        public void ReportPass()
        {
            // Delay execution if builder is null until builder is available.
            DelayExecutionForPass(null);
        }

        void DelayExecutionForPass(object state)
        {
            if (Builder != null)
            {
                ReportToBackend(CheckResult.Pass);
            }
            else
            {
                new Timer(DelayExecutionForPass, null, 2000, -1);
            }
        }

        void DelayExecutionForFail(object state)
        {
            if (Builder != null)
            {
                ReportToBackend(CheckResult.Failed((string)state));
            }
            else
            {
                new Timer(DelayExecutionForFail, state, 2000, -1);
            }
        }

        public void ReportFailed(string failureReason)
        {
            DelayExecutionForFail(failureReason);
        }

        public string Id { get; private set; }

        void ReportToBackend(CheckResult result)
        {
            var sender = Builder.Build<ISendMessages>();

            var serviceControlBackend = new ServiceControlBackend(sender, Configure, CriticalError);
            serviceControlBackend.Send(new ReportCustomCheckResult
            {
                HostId = UnicastBus.HostInformation.HostId,
                Host = UnicastBus.HostInformation.DisplayName,
                EndpointName = Configure.Settings.EndpointName(),
                CustomCheckId = Id,
                Category = Category,
                HasFailed = result.HasFailed,
                FailureReason = result.FailureReason,
                ReportedAt = DateTime.UtcNow
            });
        }

    }
}