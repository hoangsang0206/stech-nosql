using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STech.Data.ViewModels
{
    public class CaptchaVerificationResponse
    {
        public bool success { get; set; }
        public string challenge_ts { get; set; } = null!;
        public string hostname { get; set; } = null!;
        public List<string>? error_codes { get; set; }
    }
}
