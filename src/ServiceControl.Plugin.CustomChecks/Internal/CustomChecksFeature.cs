namespace ServiceControl.Plugin.CustomChecks.Internal
{
    using System.Linq;
    using NServiceBus;
    using NServiceBus.Features;
    using NServiceBus.Logging;

    class CustomChecksFeature : Feature
    {
        static ILog Log = LogManager.GetLogger<CustomChecksFeature>();

        public CustomChecksFeature()
        {
            EnableByDefault();
        }

        protected override void Setup(FeatureConfigurationContext context)
        {
            context.Settings.GetAvailableTypes()
                .Where(t => typeof(ICustomCheck).IsAssignableFrom(t) && !(t.IsAbstract || t.IsInterface))
                .ToList()
                .ForEach(t => context.Container.ConfigureComponent(t, DependencyLifecycle.InstancePerCall));

            context.Settings.GetAvailableTypes()
                .Where(t => typeof(IPeriodicCheck).IsAssignableFrom(t) && !(t.IsAbstract || t.IsInterface))
                .ToList()
                .ForEach(t => context.Container.ConfigureComponent(t, DependencyLifecycle.InstancePerCall));

            Log.Warn("The ServiceControl.Plugin.Nsb5.CustomChecks package has been replaced by the NServiceBus.CustomChecks package. See the upgrade guide for more details.");
        }
    }
}