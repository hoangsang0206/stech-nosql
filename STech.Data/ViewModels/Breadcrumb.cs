using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STech.Data.ViewModels
{
    public class Breadcrumb
    {
        public string breadcrumbName;
        public string breadcrumbLink;

        public Breadcrumb(string breadcrumbName, string breadcrumbLink)
        {
            this.breadcrumbName = breadcrumbName;
            this.breadcrumbLink = breadcrumbLink;
        }
    }
}
