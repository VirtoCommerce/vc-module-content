using System.Text.RegularExpressions;
using FluentValidation;
using VirtoCommerce.ContentModule.Core.Model;

namespace VirtoCommerce.ContentModule.Web.Validators
{
    public class ContentFolderValidator : AbstractValidator<ContentFolder>
    {
        public ContentFolderValidator()
        {
            RuleFor(context => context.Name)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("Folder name must not be null.")
                .NotEmpty()
                .WithMessage("Folder name must not be empty.")
                .MinimumLength(3)
                .WithMessage(x => $"Minimum length is 3. You entered {x.Name.Length}")
                .MaximumLength(63)
                .WithMessage(x => $"Maximum length is 63. You entered {x.Name.Length}")
                .Must(x => !x.StartsWith("-"))
                .WithMessage("Folder name must not starts with dash symbol.")
                .Must(x => !x.EndsWith("-"))
                .WithMessage("Folder name must not ends with dash symbol.")
                .Must(x => !x.Contains("--"))
                .WithMessage("Folder name must not contain consecutive dash symbols.")
                .Must(x => !new Regex("[^0-9a-z -]").IsMatch(x))
                .WithMessage(@"Folder name must contain alphanumeric lower case, ""-"" and "" "" characters only.");
        }
    }
}
