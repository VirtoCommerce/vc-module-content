using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Extenstions;
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
            result.AddFilterableAndSearchableValue("StoreId", storeId);

            AddLanguage(result, file);
            AddMetadata(result, file);

            var content = RemoveYamlHeader(file.Content);
            result.AddSearchableValue(content);

            return result;
        }

        private static void AddLanguage(IndexDocument result, IndexableContentFile file)
        {
            var parts = System.IO.Path.GetFileName(file.Name)?.Split('.');
            var name = parts?.FirstOrDefault();
            if (!string.IsNullOrEmpty(name))
            {
                result.AddFilterableAndSearchableValue("Name", name);
            }

            if (parts?.Length == 3)
            {
                try
                {
                    result.AddFilterableAndSearchableValue("CultureName", parts[1]);
                }
                catch (Exception)
                {
                }
            }

        }

        private static void AddMetadata(IndexDocument result, IndexableContentFile file)
        {
            IDictionary<string, IEnumerable<string>> metaHeaders = new Dictionary<string, IEnumerable<string>>();
            ReadYamlHeader(file.Content, metaHeaders);

            foreach (var meta in metaHeaders)
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
