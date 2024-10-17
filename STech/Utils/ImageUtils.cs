namespace STech.Utils
{
    public static class ImageUtils
    {
        public static readonly string[] ALLOWED_IMAGE_EXTENSIONS = { ".jpg", ".jpeg", ".png", ".webp" };

        public static bool CheckImageExtension(string extension)
        {
            return ALLOWED_IMAGE_EXTENSIONS.Contains(extension);
        }
    }
}
