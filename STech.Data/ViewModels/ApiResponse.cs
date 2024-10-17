using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace STech.Data.ViewModels
{
    public class ApiResponse
    {
        public bool Status { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = null!;
        public IEnumerable<string>? Errors { get; set; }
        public object? Data { get; set; }
    }
}
