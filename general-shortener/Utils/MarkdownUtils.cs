
using Microsoft.AspNetCore.Html;

namespace general_shortener.Utils
{
    public class MarkdownUtils
    {
        // /// <summary>
        // /// Markdown pipeline
        // /// </summary>
        // public static MarkdownPipeline Pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UseEmojiAndSmiley().UseSmartyPants().Build();

        /// <summary>
        /// Parse markdown string to html string
        /// </summary>
        /// <param name="markdown"></param>
        /// <returns></returns>
        public static HtmlString ParseHtmlString(string markdown)
        {
            // return new HtmlString(Markdown.ToHtml(markdown, Pipeline));
            return new HtmlString("<a>This is a test</a>");
        }
    }
}