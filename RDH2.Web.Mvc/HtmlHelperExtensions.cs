using System;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace RDH2.Web.Mvc
{
    /// <summary>
    /// HtmlHelperExtensions extends the System.Web.Mvc.HtmlHelper
    /// class with some more utility methods.
    /// </summary>
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// StripTags removes any HTML tags from the input String.
        /// </summary>
        /// <param name="html">The HtmlHelper to extend</param>
        /// <param name="input">The String from which to strip tags</param>
        /// <returns>String of stripped data</returns>
        public static String StripTags(this HtmlHelper html, String input)
        {
            return Regex.Replace(input, @"<(.|\n)*?>", String.Empty);
        }


        /// <summary>
        /// StripNbsp removes any &nbsp; characters from the input String.
        /// </summary>
        /// <param name="html">The HtmlHelper to extend</param>
        /// <param name="input">The String from which to strip NBSPs</param>
        /// <returns>String of stripped data</returns>
        public static String StripNbsp(this HtmlHelper html, String input)
        {
            return input.Replace("&nbsp;", String.Empty);
        }
    }
}
