using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class MenuLevel1
{
    public string MenuName { get; set; } = null!;

    public string RedirectUrl { get; set; } = null!;

    public List<MenuLevel2> MenuLevel2s { get; set; } = new List<MenuLevel2>();
}
