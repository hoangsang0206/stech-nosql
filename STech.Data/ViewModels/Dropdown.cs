namespace STech.Data.ViewModels
{
    public class Dropdown
    {
        public string? ClassName { get; set; }
        public string? Id { get; set; }
        public string? IconElement { get; set; }
        public string? Text { get; set; }
        public bool IsHasDropdownArrow { get; set; } = false;
        public bool IsDisabled { get; set; } = false;
        public bool IsHasContentAtRight { get; set; } = false;

        public List<DropdownItem> Items { get; set; } = new List<DropdownItem>();

        public class DropdownItem
        {
            public string? IconElement { get; set; }
            public string Value { get; set; }
            public string Text { get; set; }
            public bool IsSelected { get; set; } = false;
        }
    }
}
