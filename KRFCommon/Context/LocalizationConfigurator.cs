namespace KRFCommon.Context
{
    using System;
    using System.Globalization;

    using KRFCommon.Api;

    using Microsoft.AspNetCore.Builder;

    public static class LocalizationConfigurator
    {
        public static IApplicationBuilder UseLocalization( this IApplicationBuilder app, LocalizationConfiguration configuration )
        {
            if ( configuration != null )
            {
                if ( string.IsNullOrEmpty( configuration.LocalizationCode ) )
                {
                    throw new ArgumentNullException( configuration.LocalizationCode );
                }

                var cultureInfo = new CultureInfo( configuration.LocalizationCode );
                if ( !string.IsNullOrEmpty( configuration.CurrencyLocalizationCode ) )
                {
                    var currencyLocalization = new CultureInfo( configuration.CurrencyLocalizationCode );
                    cultureInfo.NumberFormat.CurrencySymbol = currencyLocalization.NumberFormat.CurrencySymbol;
                }

                CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
                CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            }

            return app;
        }
    }
}
