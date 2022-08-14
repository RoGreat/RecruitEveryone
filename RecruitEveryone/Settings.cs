namespace RecruitEveryone
{
    internal sealed class Settings
    {
        private ISettingsProvider _provider;

        public bool ToggleCompanionLimit { get => _provider.ToggleCompanionLimit; set => _provider.ToggleCompanionLimit = value; }

        public int CompanionLimit { get => _provider.CompanionLimit; set => _provider.CompanionLimit = value; }

        public string TemplateCharacter { get => _provider.TemplateCharacter; set => _provider.TemplateCharacter = value; }

        public Settings()
        {
            if (MCMSettings.Instance is not null)
            {
                _provider = MCMSettings.Instance;
            }
            else if (CustomConfig.Instance is not null)
            {
                _provider = CustomConfig.Instance;
            }
            else
            {
                _provider = new CustomConfig();
            }
        }
    }
}