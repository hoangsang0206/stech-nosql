using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STech.Data.ViewModels
{
    public class ExternalRegisterVM
    {
        public string UserId { get; set; } = null!;

        public string? Email { get; set; }
        public bool EmailConfirmed { get; set; } = false;

        public string? Avatar { get; set; } = null!;

        public string? FullName { get; set; } = null!;

        public string? AuthenticationProvider { get; set; } = null!;
    }
}
