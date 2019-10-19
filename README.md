# Api Localization (Framework 4.7)
Restful api localization


Terminology:

	Localization (l10n): It is the process of customizing an application for a given language and region.
	Globalization (g11n): it is the process of making an application support different languages and regions.
	Internationalization (i18n): It is the combination of both localization and globalization.
	Culture: It is a language and region where region is optional. Aka Locale.
	Neutral Culture: It is a culture that has only language and not region. Like 'en' or 'de'.
	Specific Culture: It is a culture that has both language and region. Like 'en-US' or 'en-IN'.


Note:

	- Date is UTC. So it is same.
	- Culture is case-insensitive.
	- First, Internationalization is attempted using Specific Culture. 
	  If Specific Culture is not found then it chacks for Neutral Culture. 
	  And if, Neutral Culture is also not found then it sets the default.


Code:

1). Create the resource files like Language.resx, Language.en-US.resx and other.

2). Create an Employee model.

	using System;
	using System.ComponentModel.DataAnnotations;
	using Api.Resources;

	namespace Api.Models
	{
		public class Employee
		{
			[Required(ErrorMessageResourceType = typeof(Language), 
				ErrorMessageResourceName = "NameRequired")]
			public string Name { get; set; }
			public string Description { get; set; }
			public DateTime Timestamp { get; set; }
		}
	}
		
3). Add the below keys in Web.config.
		
	<configuration>
	  <appSettings>
		<add key="Languages" value="en-us,en-IN,en-GB"/>
		<add key="DefaultLanguage" value="en-GB"/>

4). Create a class InternationalizationHandler and inherit DelegatingHandler.
		
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

		protected override async Task<HttpResponseMessage> 
			SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var languages = _supportedLanguages.Split(',').ToList();

			if (!IsSpecificCulture(request, languages))
			{
				if (!IsNeutralCulture(request, languages))
				{
					SetCultureInfo(request, _defaultLanguage);
				}
			}

			var response = await base.SendAsync(request, cancellationToken);
			return response;
		}

		private void SetCultureInfo(HttpRequestMessage request, string language)
		{
			request.Headers.AcceptLanguage.Clear();
			request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(language));
			Thread.CurrentThread.CurrentCulture = new CultureInfo(language);
			Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
		}

		private bool IsSpecificCulture(HttpRequestMessage request, IReadOnlyCollection<string> languages)
		{
			foreach (var language in request.Headers.AcceptLanguage)
			{
				if (languages.Contains(language.Value, StringComparer.CurrentCultureIgnoreCase))
				{
					SetCultureInfo(request, language.Value);
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
					SetCultureInfo(request, 
						languages.FirstOrDefault(i => i.StartsWith(neutralCulture)));
					return true;
				}
			}

			return false;
		}
	}
}

5). Put the below code line in WebApiConfig file.

	config.MessageHandlers.Add(new InternationalizationHandler());

6). Put below two action methods in a controller (only for testing purpose).

        [HttpGet]
        public IHttpActionResult Get()
        {
            var employee = new Employee
            {
                Name = Resources.Language.Name,
                Description = Resources.Language.Description,
                Timestamp = DateTime.UtcNow
            };

            return Ok(employee);
        }

        // POST api/values
        [HttpPost]
        public IHttpActionResult Post([FromBody]Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(employee);
        }

Done. Now you can test.
		
Steps:

1. Open Postman
2. Follow the examples...

	Ex 1:
	GET http://localhost:57502/api/Values
	Accept-Language:da,en-US;q=0.8,en;q=0.7
	Output:
	{
		"Name": "Name in en-US",
		"Description": "Description in en-US",
		"Timestamp": "2019-10-19T10:10:56.0184846Z"
	}

	Ex 2:
	GET http://localhost:57502/api/Values
	Accept-Language:en-IN
	Output:
	{
		"Name": "Name in en-IN",
		"Description": "Description in en-IN",
		"Timestamp": "2019-10-19T10:12:12.3115222Z"
	}

	Ex 3:
	POST http://localhost:57502/api/values 
	Content-Type: application/json
	Accept-Language: en-IN
	Body-Raw-JSON-Input:
	{
		"Description": "Private employee"
	}

	Output:
	{
		"Message": "The request is invalid.",
		"ModelState": {
			"employee.Name": [
				"Name is required (en-IN)"
			]
		}
	}

	Ex 4: To test default settings
	GET http://localhost:57502/api/Values
	Accept-Language:xx
	Output:
	{
		"Name": "Name in en-GB",
		"Description": "Description in en-GB",
		"Timestamp": "2019-10-19T10:10:56.0184846Z"
	}
