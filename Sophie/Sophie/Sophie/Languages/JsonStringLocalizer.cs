using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Sophie.Units;

namespace Sophie.Languages
{
    public enum EnumLanguage
    {
        EN,
        VI
    }

    public class JsonStringLocalizer : IStringLocalizer
    {
        List<JsonLocalization> localization_en = new List<JsonLocalization>();
        List<JsonLocalization> localization_vi = new List<JsonLocalization>();

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new JsonStringLocalizer();
        }

        public JsonStringLocalizer()
        {
            localization_en = Newtonsoft.Json.JsonConvert.DeserializeObject<List<JsonLocalization>>(File.ReadAllText(@"Languages/localization.en-US.json"));
            localization_vi = Newtonsoft.Json.JsonConvert.DeserializeObject<List<JsonLocalization>>(File.ReadAllText(@"Languages/localization.vi-VN.json"));
        }

        /*
         * Function: get value by key with language default
         */
        public LocalizedString this[string name]
        {
            get
            {
                string currentCulture = "en-US";//[en-US, vi-VN]
                if (!string.IsNullOrEmpty(CultureInfo.CurrentCulture.Name)) currentCulture = CultureInfo.CurrentCulture.Name;
                var value = GetString(name, currentCulture);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
            }
        }

        /*
         * Function: get value by key with language custom
         */
        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                string currentCulture = "en-US";//[en-US, vi-VN]
                if (!string.IsNullOrEmpty(CultureInfo.CurrentCulture.Name)) currentCulture = CultureInfo.CurrentCulture.Name;
                if (arguments[0] is HttpContext)
                {
                    HttpContext httpContext = (HttpContext)arguments[0];
                    string deviceLanguage = "VI";
                    try
                    {
                        if (httpContext.Items != null && httpContext.Items.ContainsKey("deviceLanguage"))
                        {
                            deviceLanguage = (httpContext.Items["deviceLanguage"]?.ToString()) ?? "VI";
                        }
                    }
                    catch (Exception ex)
                    {
                        Logs.debug($"[JsonStringLocalizer] '{name}' Exception: {ex.StackTrace}");
                    }
                    EnumLanguage language;
                    try
                    {
                        if (!Enum.TryParse(deviceLanguage, true, out language))
                        {
                            language = EnumLanguage.EN;
                        }
                    }
                    catch (Exception)
                    {
                        language = EnumLanguage.EN;
                    }
                    if (language == EnumLanguage.VI) currentCulture = "vi-VN";
                }
                else if (arguments[0] is EnumLanguage)
                {
                    EnumLanguage language = (EnumLanguage)arguments[0];
                    if (language == EnumLanguage.VI) currentCulture = "vi-VN";
                }
                var value_format = GetString(name, currentCulture);
                var value = string.Format(value_format ?? name, arguments);
                return new LocalizedString(name, value, resourceNotFound: value_format == null);
            }
        }

        /*
         * Function: get all object with language
         */
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            string currentCulture = "en-US";//[en-US, vi-VN]
            if (!string.IsNullOrEmpty(CultureInfo.CurrentCulture.Name)) currentCulture = CultureInfo.CurrentCulture.Name;
            if (currentCulture.Equals("vi-VN", StringComparison.CurrentCultureIgnoreCase))
            {
                return localization_vi.Select(item => new LocalizedString(item.Localized_Key, item.Localized_Value, true));
            }
            else
            {
                return localization_vi.Select(item => new LocalizedString(item.Localized_Key, item.Localized_Value, true));
            }
        }

        /*
         * Function: get value by key with language parameter
         */
        private string GetString(string name, string currentCulture)
        {
            if (currentCulture.Equals("vi-VN", StringComparison.CurrentCultureIgnoreCase))
            {
                var value = localization_vi.Where(item => item.Localized_Key == name).FirstOrDefault().Localized_Value;
                return value;
            }
            else
            {
                var value = localization_en.Where(item => item.Localized_Key == name).FirstOrDefault().Localized_Value;
                return value;
            }
        }

    }
}
