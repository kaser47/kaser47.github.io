using System.Linq;
using HtmlAgilityPack;

namespace RecentlyAddedShows.Web.Classes
{
    public static class HtmlNodeExtensions
    {
        private static string GetAttributeValue(this HtmlNode node, string attribute, params int[] path)
        {
            if (path.Length <= 0) return node.Attributes[attribute].Value;

            var i = path.First();
            var newNode = node.ChildNodes[i];
            path = path.Skip(1).ToArray();

            return newNode.GetAttributeValue(attribute: attribute, path);
        }

        public static string GetText(this HtmlNode node, params int[] path)
        {
            if (path.Length <= 0) return node.InnerText;

            var i = path.First();
            var newNode = node.ChildNodes[i];
            path = path.Skip(1).ToArray();

            return newNode.GetText(path);
        }

        public static string GetImage(this HtmlNode node, params int[] path)
        {
            var attribute = "src";
            if (path.Length <= 0) return node.Attributes[attribute].Value;

            var i = path.First();
            var newNode = node.ChildNodes[i];
            path = path.Skip(1).ToArray();

            return newNode.GetAttributeValue(attribute: attribute, path);
        }

        public static string GetActualImage(this HtmlNode node, params int[] path)
        {
            var attribute = "data-original";
            if (path.Length <= 0) return node.Attributes[attribute].Value;

            var i = path.First();
            var newNode = node.ChildNodes[i];
            path = path.Skip(1).ToArray();

            return newNode.GetAttributeValue(attribute: attribute, path);
        }

        public static string GetUrl(this HtmlNode node, params int[] path)
        {
            var attribute = "href";
            if (path.Length <= 0) return node.Attributes[attribute].Value;

            var i = path.First();
            var newNode = node.ChildNodes[i];
            path = path.Skip(1).ToArray();

            return newNode.GetAttributeValue(attribute: attribute, path);
        }

        public static string GetDateAdded(this HtmlNode node, params int[] path)
        {
            var attribute = "data-added";
            if (path.Length <= 0) return node.Attributes[attribute].Value;

            var i = path.First();
            var newNode = node.ChildNodes[i];
            path = path.Skip(1).ToArray();

            return newNode.GetAttributeValue(attribute: attribute, path);
        }

        public static string GetDate(this HtmlNode node, params int[] path)
        {
            var attribute = "data-date";
            if (path.Length <= 0) return node.Attributes[attribute].Value;

            var i = path.First();
            var newNode = node.ChildNodes[i];
            path = path.Skip(1).ToArray();

            return newNode.GetAttributeValue(attribute: attribute, path);
        }
    }
}