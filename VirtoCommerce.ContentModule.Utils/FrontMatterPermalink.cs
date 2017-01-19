using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VirtoCommerce.ContentModule.Utils
{
    /// <summary>
    /// http://jekyllrb.com/docs/permalinks/
    /// </summary>
    public class FrontMatterPermalink
    {
        private static readonly Regex _timestampAndTitleFromPathRegex = new Regex(string.Format(@"{0}(?:(?<timestamp>\d+-\d+-\d+)-)?(?<title>[^{0}]*)\.[^\.]+$", Regex.Escape("/")), RegexOptions.Compiled);
        private static readonly Regex _timestampAndTitleAndLanguageFromPathRegex = new Regex(string.Format(@"{0}(?:(?<timestamp>\d+-\d+-\d+)-)?(?<title>[^{0}]*)\.(?<language>[A-z]{{2}}-[A-z]{{2}})\.[^\.]+$", Regex.Escape("/")), RegexOptions.Compiled);
        private static readonly Regex _categoryRegex = new Regex(@":category(\d*)", RegexOptions.Compiled);
        private static readonly Regex _slashesRegex = new Regex(@"/{1,}", RegexOptions.Compiled);

        //http://jekyllrb.com/docs/permalinks/#builtinpermalinkstyles
        private static readonly Dictionary<string, string> _builtInPermalinkStyles = new Dictionary<string, string>
        {
            { "date", ":folder/:categories/:year/:month/:day/:title" },
            { "pretty", ":folder/:categories/:year/:month/:day/:title/" },
            { "ordinal", ":folder/:categories/:year/:y_day/:title" },
            { "default", ":folder/:categories/:title" },
        };

      
        public FrontMatterPermalink(string urlTemplate)
        {
            UrlTemplate = urlTemplate;
            Categories = new List<string>();
        }
        //template-variable pattern.
        public string UrlTemplate { get; private set; }
        public string FilePath { get; set; }
        public DateTime? Date { get; set; }
        public ICollection<string> Categories { get; set; }

        /// <summary>
        /// Build relative url based on permalink  template and other properties
        /// </summary>
        /// <returns></returns>
        public virtual string ToUrl()
        {
            var result = UrlTemplate.Replace("~/", string.Empty);            

            if (_builtInPermalinkStyles.ContainsKey(result))
            {
                result = _builtInPermalinkStyles[result];
            }

            var removeLeadingSlash = !result.StartsWith("/");

            result = result.Replace(":folder", FilePath != null ? Path.GetDirectoryName(FilePath).Replace("\\", "/") : string.Empty);
            result = result.Replace(":categories", string.Join("/", Categories.ToArray()));
            result = result.Replace(":dashcategories", string.Join("-", Categories.ToArray()));
            result = result.Replace(":year", Date != null ? Date.Value.Year.ToString(CultureInfo.InvariantCulture) : string.Empty);
            result = result.Replace(":month", Date != null ? Date.Value.ToString("MM") : string.Empty);
            result = result.Replace(":day", Date != null ? Date.Value.ToString("dd") : string.Empty);
            result = result.Replace(":title", FilePath != null ? GetTitleFromFilePath(FilePath) : string.Empty);
            result = result.Replace(":y_day", Date != null ? Date.Value.DayOfYear.ToString("000") : string.Empty);
            result = result.Replace(":short_year", Date != null ? Date.Value.ToString("yy") : string.Empty);
            result = result.Replace(":i_month", Date != null ? Date.Value.Month.ToString() : string.Empty);
            result = result.Replace(":i_day", Date != null ? Date.Value.Day.ToString() : string.Empty);

            if (result.Contains(":category"))
            {
                var matches = _categoryRegex.Matches(result);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        var replacementValue = string.Empty;
                        if (match.Success)
                        {
                            int categoryIndex;
                            if (int.TryParse(match.Groups[1].Value, out categoryIndex) && categoryIndex > 0)
                            {
                                replacementValue = Categories.Skip(categoryIndex - 1).FirstOrDefault();
                            }
                            else if (Categories.Any())
                            {
                                replacementValue = Categories.First();
                            }
                        }

                        result = result.Replace(match.Value, replacementValue);
                    }
                }
            }

            result = _slashesRegex.Replace(result, "/");

            if (removeLeadingSlash)
                result = result.TrimStart('/');
            return result;
        }

        protected virtual string GetTitleFromFilePath(string filePath)
        {
            // try extracting title when language is specified, if null or empty continue without a language
            var title = _timestampAndTitleAndLanguageFromPathRegex.Match(filePath).Groups["title"].Value;

            if (string.IsNullOrEmpty(title))
                title = _timestampAndTitleFromPathRegex.Match(filePath).Groups["title"].Value;

            return title;
        }

    }
}
