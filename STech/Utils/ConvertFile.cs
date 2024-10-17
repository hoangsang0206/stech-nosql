using Microsoft.AspNetCore.Http;

namespace STech.Utils
{
    public static class ConvertFile
    {
        public static byte[] ConvertIFormFileToByteArray(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return [];

            using (MemoryStream memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
