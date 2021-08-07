
using Markdig;
using Microsoft.AspNetCore.Html;

namespace general_shortener.Utils
{
    /// <summary>
    /// Class
    /// </summary>
    public class MarkdownUtils
    {
        /// <summary>
        /// Markdown pipeline
        /// </summary>
        public static MarkdownPipeline Pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UseSoftlineBreakAsHardlineBreak().UseEmojiAndSmiley().UseSmartyPants().Build();

        /// <summary>
        /// Parse markdown string to html string
        /// </summary>
        /// <param name="markdown"></param>
        /// <returns></returns>
        public static HtmlString ParseHtmlString(string markdown)
        {
            return new(Markdown.ToHtml(markdown, Pipeline));
        }
    }
}