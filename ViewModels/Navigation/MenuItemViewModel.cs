public class MenuItemViewModel
{
    public string Text { get; set; } = string.Empty;
    public string? Controller { get; set; }
    public string? Action { get; set; }
    public string IconClass { get; set; } = string.Empty;
    public List<MenuItemViewModel> SubItems { get; set; } = new ();
}