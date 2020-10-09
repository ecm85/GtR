using System;

namespace GtR
{
    public enum SaveConfiguration
    {
        Unknown,
        SingleImage,
        Page
    }

    public static class SaveConfigurationExtensions
    {
        public static string GetDisplayValue(this SaveConfiguration saveConfiguration)
        {
            switch(saveConfiguration)
            {
                case SaveConfiguration.Page:
                    return "pages of cards";
                case SaveConfiguration.SingleImage:
                    return "each image individually";
                default:
                    throw new InvalidOperationException($"Invalid save configuration encountered: {saveConfiguration}.");
            }
        }
    }
}
