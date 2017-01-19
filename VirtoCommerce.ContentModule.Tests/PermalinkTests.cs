using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Utils;
using Xunit;

namespace VirtoCommerce.ContentModule.Tests
{
    [Trait("Category", "CI")]
    public class PermalinkUrlTests
    {

        [Fact]
        public void GetUrl_returns_folder_and_original_value_when_no_timestamp()
        {
            var fmPermalink = new FrontMatterPermalink(":folder/:title")
            {
                FilePath = @"/temp/foobar_baz.md",
            };
        
            var url = fmPermalink.ToUrl();
            Assert.Equal("temp/foobar_baz", url);
        }

        [Fact]
        public void GetUrl_returns_file_name_when_no_folder()
        {
            var fmPermalink = new FrontMatterPermalink(":title")
            {
                FilePath = @"/foobar_baz.en-us.md",
            };
            var url = fmPermalink.ToUrl();
            Assert.Equal("foobar_baz", url);
        }


        [Fact]
        public void GetUrl_returns_strips_timestamp()
        {
            var fmPermalink = new FrontMatterPermalink(":title")
            {
                FilePath = @"/temp/2012-01-03-foobar_baz.md",
            };
            var url = fmPermalink.ToUrl();
            Assert.Equal("foobar_baz", url);
        }

        [Fact]
        public void GetUrl_preserves_dash_separated_values_that_arent_timestamps()
        {
            var fmPermalink = new FrontMatterPermalink(":title")
            {
                FilePath = @"/temp/foo-bar-baz-qak-foobar_baz.md",
            };
            var url = fmPermalink.ToUrl();
            Assert.Equal("foo-bar-baz-qak-foobar_baz", url);
        }

        [InlineData("date", "temp/cat1/cat2/2015/03/09/foobar-baz", "cat1,cat2")]
        [InlineData("date", "temp/2015/03/09/foobar-baz", "")]
        [InlineData("/:dashcategories/:year/:month/:day/:title.html", "/cat1-cat2/2015/03/09/foobar-baz.html", "cat1,cat2")]
        [InlineData("/:dashcategories/:year/:month/:day/:title.html", "/2015/03/09/foobar-baz.html", "")]
        [InlineData("pretty", "temp/cat1/cat2/2015/03/09/foobar-baz/", "cat1,cat2")]
        [InlineData("ordinal", "temp/cat1/cat2/2015/068/foobar-baz", "cat1,cat2")]
        [InlineData("none", "temp/cat1/cat2/foobar-baz", "cat1,cat2")]
        [InlineData("/:categories/:short_year/:i_month/:i_day/:title.html", "/cat1/cat2/15/3/9/foobar-baz.html", "cat1,cat2")]
        [InlineData("/:category/:title.html", "/cat1/foobar-baz.html", "cat1,cat2")]
        [InlineData("/:category/:title.html", "/foobar-baz.html", "")]
        [InlineData("/:category1/:title.html", "/cat1/foobar-baz.html", "cat1,cat2")]
        [InlineData("/:category2/:title.html", "/cat2/foobar-baz.html", "cat1,cat2")]
        [InlineData("/:category3/:title.html", "/foobar-baz.html", "cat1,cat2")]
        [InlineData("/:categories/:title/", "/cat1/cat2/foobar-baz/", "cat1,cat2")]
        [InlineData("/:categories/:title", "/cat1/cat2/foobar-baz", "cat1,cat2")]
        [Theory]
        [CLSCompliant(false)]
        public void EvaluatePermalink_url_is_well_formatted(string permalink, string expectedUrl, string categories)
        {
            var fmPermalink = new FrontMatterPermalink(permalink)
            {
                Categories = categories == null ? new List<string>() : categories.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                Date = new DateTime(2015, 03, 09),
                FilePath = @"/temp/2015-03-09-foobar-baz.md",
            };
            var url = fmPermalink.ToUrl();
            Assert.Equal(expectedUrl, url);
        }
    }
}
