using System.Linq;
using MvvmValidation;

namespace Simple.Wpf.Util
{
    public class ValidationSummaryFormatter : IValidationResultFormatter
    {
        public string Format(ValidationResult validationResult)
        {
            return validationResult.IsValid
                ? string.Empty
                : string.Join("\r\n", validationResult.ErrorList.Select(e => $"{e.Target}: {e.ErrorText}"));
        }
    }
}