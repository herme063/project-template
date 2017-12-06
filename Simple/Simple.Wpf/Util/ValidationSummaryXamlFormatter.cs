using System.Linq;
using MvvmValidation;

namespace Simple.Wpf.Util
{
    public class ValidationSummaryXamlFormatter : IValidationResultFormatter
    {
        public string Format(ValidationResult validationResult)
        {
            return validationResult.IsValid
                ? string.Empty
                : $@"
<Section xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
    <Paragraph Margin=""3"">
        <Run FontWeight=""Bold"">Please correct the following errors: </Run>
    </Paragraph>
{
string.Join("", validationResult.ErrorList.Select(e => $@"
    <Paragraph Margin=""2"">
        <Run FontWeight=""Bold"">{e.Target}</Run>
        <Run>{e.ErrorText}</Run>
    </Paragraph>
"))
}</Section>";
        }
    }
}