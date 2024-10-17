using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class Menu
{
    public ObjectId _id { get; set; }

    public int MId { get; set; }

    public string MenuName { get; set; } = null!;

    public string RedirectUrl { get; set; } = null!;

    public string MenuIcon { get; set; } = null!;

    public List<MenuLevel1> MenuLevel1s { get; set; } = new List<MenuLevel1>();
}
