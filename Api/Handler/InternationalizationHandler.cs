using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Handler
{
    public class InternationalizationHandler : DelegatingHandler
    {
        private readonly string _supportedLanguages = ConfigurationManager.AppSettings["Languages"];
        private readonly string _defaultLanguage = ConfigurationManager.AppSettings["DefaultLanguage"];

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var languages = _supportedLanguages.Split(',').ToList();

            if (!IsSpecificCulture(request, languages))
            {
                if (!IsNeutralCulture(request, languages))
                {
                    SetCurrentCulture(request, _defaultLanguage);
                }
            }

            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }

        private void SetCurrentCulture(HttpRequestMessage request, string language)
        {
            request.Headers.AcceptLanguage.Clear();
            request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(language));

            //Thread.CurrentThread.CurrentCulture = new CultureInfo(language);
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);

            var culture = new CultureInfo(language);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

        private bool IsSpecificCulture(HttpRequestMessage request, IReadOnlyCollection<string> languages)
        {
            foreach (var language in request.Headers.AcceptLanguage)
            {
                if (languages.Contains(language.Value, StringComparer.CurrentCultureIgnoreCase))
                {
                    SetCurrentCulture(request, language.Value);
                    return true;
                }
            }

            return false;
        }

        private bool IsNeutralCulture(HttpRequestMessage request, IReadOnlyCollection<string> languages)
        {
            foreach (var language in request.Headers.AcceptLanguage)
            {
                var neutralCulture = language.Value.Substring(0, 2);
                if (languages.Any(t => t.StartsWith(neutralCulture)))
                {
                    SetCurrentCulture(request, languages.FirstOrDefault(i => i.StartsWith(neutralCulture)));
                    return true;
                }
            }

            return false;
        }
    }
}