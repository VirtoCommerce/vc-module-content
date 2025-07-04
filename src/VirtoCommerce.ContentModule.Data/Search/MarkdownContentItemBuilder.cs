using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Extensions;
using VirtoCommerce.SearchModule.Core.Model;
using YamlDotNet.RepresentationModel;

namespace VirtoCommerce.ContentModule.Data.Search
{
    public class MarkdownContentItemBuilder : BaseContentItemBuilder
    {
        private static readonly Regex _headerRegExp = new(@"(?s:^---(.*?)---)");

        protected override IndexDocument BuildIndexDocumentInternal(string documentId, string storeId, IndexableContentFile file)
        {
            var result = new IndexDocument(documentId);
            result.AddFilterableStringAndContentString("StoreId", storeId);

            AddMetadata(result, file);

            var content = RemoveYamlHeader(file.Content);
            result.AddContentString(content);

            return result;
        }

        private static void AddMetadata(IndexDocument result, IndexableContentFile file)
        {
            IDictionary<string, IEnumerable<string>> metaHeaders = new Dictionary<string, IEnumerable<string>>();
            ReadYamlHeader(file.Content, metaHeaders);

            foreach (var meta in metaHeaders)
            {
                if (meta.Value.Count() == 1)
                {
                    var value = meta.Value.First();
                    if (meta.Key == "permalink" && !value.StartsWith('/'))
                    {
                        value = "/" + value;
                    }

                    result.AddFilterableStringAndContentString(meta.Key, value);
                }
                else
                {
                    var index = 0;
                    foreach (var value in meta.Value)
                    {
                        result.AddFilterableStringAndContentString($"{meta.Key}__{index}", value);
                        index++;
                    }
                }
            }

            if (result.Fields.All(x => !x.Name.EqualsIgnoreCase("displayname")))
            {
                var name = Path.GetFileNameWithoutExtension(file.Name);
                result.AddFilterableStringAndContentString("displayName", name);
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
