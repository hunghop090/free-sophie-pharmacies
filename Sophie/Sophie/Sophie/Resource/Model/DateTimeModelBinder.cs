using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Sophie.Resource.Model
{
    public class DateTimeModelBinder : IModelBinder
    {
        public ILoggerFactory _logManager;

        public DateTimeModelBinder(ILoggerFactory config)
        {
            _logManager = config;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            IModelBinder baseBinder = new SimpleTypeModelBinder(typeof(DateTime), _logManager);

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult != ValueProviderResult.None)
            {
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

                var valueAsString = valueProviderResult.FirstValue;

                //  valueAsString will have a string value of your date, e.g. '31/12/2017'
                var dateTime = DateTime.ParseExact(valueAsString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                bindingContext.Result = ModelBindingResult.Success(dateTime);

                return Task.CompletedTask;
            }

            return baseBinder.BindModelAsync(bindingContext);
        }
    }
}
