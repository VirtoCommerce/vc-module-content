using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Search;
using VirtoCommerce.SearchModule.Core.Extenstions;
using VirtoCommerce.SearchModule.Core.Model;
using YamlDotNet.RepresentationModel;

namespace VirtoCommerce.ContentModule.Data.Search
{
    public class MarkdownContentItemBuilder : IContentItemBuilder
    {
        private static readonly Regex _headerRegExp = new Regex(@"(?s:^---(.*?)---)");

        public IndexDocument BuildIndexDocument(string storeId, IndexableContentFile file)
        {
            // todo: id should be composed either from storeId and file url or something else
            // todo: should the page depend on theme?
            var result = new IndexDocument(file.RelativeUrl);
            result.AddFilterableAndSearchableValue("StoreId", storeId);

            AddMetadata(result, file);

            var content = RemoveYamlHeader(file.Content);
            result.AddSearchableValue(content);
            return result;
        }

        private void AddMetadata(IndexDocument result, IndexableContentFile file)
        {
            try
            {
                IDictionary<string, IEnumerable<string>> metaHeaders = new Dictionary<string, IEnumerable<string>>();
                ReadYamlHeader(file.Content, metaHeaders);

                foreach(var meta in metaHeaders)
                {
                    if (meta.Value.Count() == 1)
                    {
                        result.AddFilterableAndSearchableValue(meta.Key, meta.Value.First());
                    }
                    else
                    {
                        var index = 0;
                        foreach (var value in meta.Value)
                        {
                            result.AddFilterableAndSearchableValue($"{meta.Key}__{index}", value);
                            index++;
                        }
                    }
                }
            }
            catch
            {
                // todo: log? rethrow?
                throw;
            }
        }

        private static void ReadYamlHeader(string text, IDictionary<string, IEnumerable<string>> metadata)
        {
            var headerMatches = _headerRegExp.Matches(text);

            if (headerMatches.Count > 0)
            {
                var input = new StringReader(headerMatches[0].Groups[1].Value);
                var yaml = new YamlStream();

                yaml.Load(input);

                if (yaml.Documents.Count > 0)
                {
                    var root = yaml.Documents[0].RootNode;
                    if (root is YamlMappingNode collection)
                    {
                        foreach (var entry in collection.Children)
                        {
                            if (entry.Key is YamlScalarNode node)
                            {
                                metadata.Add(node.Value, GetYamlNodeValues(entry.Value));
                            }
                        }
                    }
                }
            }
        }

        private static IEnumerable<string> GetYamlNodeValues(YamlNode value)
        {
            var result = new List<string>();

            if (value is YamlSequenceNode list)
            {
                result.AddRange(list.Children.OfType<YamlScalarNode>().Select(node => node.Value));
            }
            else
            {
                result.Add(value.ToString());
            }

            return result;
        }

        private static string RemoveYamlHeader(string text)
        {
            var result = text;
            var headerMatches = _headerRegExp.Matches(text);

            if (headerMatches.Count > 0)
            {
                result = text.Replace(headerMatches[0].Groups[0].Value, "").Trim();
            }

            return result;
        }
    }
}
