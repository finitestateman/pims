namespace Pims.App.ViewModels
{
    public sealed class MenuDefinition
    {
        public MenuDefinition(string key, string displayName)
        {
            Key = key;
            DisplayName = displayName;
        }

        public string Key { get; }

        public string DisplayName { get; }
    }
}
