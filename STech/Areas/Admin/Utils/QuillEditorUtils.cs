namespace STech.Areas.Admin.Utils
{
    public static class QuillEditorUtils
    {
        public static string ReplaceClasses(this string html)
        {
            return html
                .Replace("ql-size-large", "heading-size-large")
                .Replace("ql-size-huge", "heading-size-huge")
                .Replace("ql-align-center", "text-center")
                .Replace("ql-align-right", "text-end")
                .Replace("ql-align-justify", "text-justify");
        }
    }
}
