using System.Text.RegularExpressions;
using FluentValidation;
using VirtoCommerce.ContentModule.Core.Model;

namespace VirtoCommerce.ContentModule.Web.Validations
{
    public class ContentFolderValidator : AbstractValidator<ContentFolder>
    {
        public ContentFolderValidator()
        {
            RuleFor(context => context.Name)
                .Cascade(CascadeMode.Continue)
                .NotEmpty()
                .NotNull()
                .MinimumLength(3)
                .MaximumLength(63)
                .Must(x => !x.StartsWith("-") && !x.EndsWith("-") && !x.Contains("--"))
                .Must(x => new Regex("[0-9a-z -]").IsMatch(x));
        }
    }
}
