namespace ServiceControl.Plugin.CustomChecks.Sample
{
    using System;
    using CustomChecks;

    class InteractiveCustomCheck : PeriodicCheck
    {
        public InteractiveCustomCheck()
            : base("InteractiveCustomCheck", "CustomCheck",TimeSpan.FromSeconds(5))
        {
        }

        public static bool ShouldFail { get; set; }
        public override CheckResult PerformCheck()
        {
            return ShouldFail ? CheckResult.Failed("User asked me to fail") : CheckResult.Pass;
        }
    }
}