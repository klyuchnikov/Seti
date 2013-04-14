using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Attribute = System.Attribute;

namespace Klyuchnikov.Seti.TwoSemestr.CommonLibrary
{
    public class Parser
    {
        public static void ParseDocument(byte[] arr, string url)
        {
            var ss = Encoding.Default.GetString(arr);
            var charset = Regex.Match(ss, @"charset=(?<charset>[\w-]*)", RegexOptions.IgnoreCase).Groups["charset"].Value;
            if (!string.IsNullOrEmpty(charset))
                ss = Encoding.GetEncoding(charset).GetString(arr);
            var title = Regex.Match(ss, @"<title>(?<title>[\d\D]*)</title>", RegexOptions.IgnoreCase).Groups["title"].Value;
            var doc = new Document(title, url);
            ParseTags(ss, doc, "h\\d");
            ParseTags(ss, doc, "p");
            ParseKeywords(ss, doc);
            Model.Current.documents.Add(doc);
            Model.Current.Documents = null;
        }
        private static void ParseKeywords(string str, Document doc)
        {
            var match = Regex.Match(str, "<meta name=\"keywords\" content=\"(?<content>.*?)\"\\s*?/>", RegexOptions.IgnoreCase);
            var keywords = match.Groups["content"].Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var keyword in keywords)
            {
                doc.Keywords.Add(keyword.Trim());
            }
        }

        private static void ParseTags(string str, Document doc, string pattern)
        {
            var matches = Regex.Matches(str, string.Format("<{0}\\s*(?<attr>.*?)>(?<value>[\\d\\D]*?)</(?<tag>{0})>", pattern), RegexOptions.IgnoreCase);
            foreach (Match h in matches)
            {
                var tagS = h.Groups["tag"].Value;
                var value = h.Groups["value"].Value.Replace("\n", "").Replace("\r", "");
                var tag = new Tag(doc, tagS, value);
                ParseAttributes(tag, h.Groups["attr"].Value);
                Model.Current.Tags.Add(tag);
            }
        }
        private static void ParseTagsWhitoutBody(string str, Document doc, string pattern)
        {
            var matches = Regex.Matches(str, string.Format("<(?<tag>{0})\\s*(?<attr>.*?)/>", pattern), RegexOptions.IgnoreCase);
            foreach (Match h in matches)
            {
                var tagS = h.Groups["tag"].Value;
                var tag = new Tag(doc, tagS, null);
                ParseAttributes(tag, h.Groups["attr"].Value);
                Model.Current.Tags.Add(tag);
            }
        }

        private static void ParseAttributes(Tag tag, string str)
        {
            var attrsS = str.Replace("\n", "").Replace("\r", "");
            var ms = Regex.Matches(attrsS, "\\s*?(?<name>\\S*?)=\"(?<value>.*?)\"", RegexOptions.IgnoreCase);
            foreach (var newatt in from Match m in ms
                                   let name = m.Groups["name"].Value
                                   let valueA = m.Groups["value"].Value
                                   select new Attribute(tag, name, valueA))
            {
                Model.Current.Attributes.Add(newatt);
            }
        }
    }
}
