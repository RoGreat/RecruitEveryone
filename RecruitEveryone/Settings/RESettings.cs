namespace RecruitEveryone.Settings
{
    internal class RESettings
    {
        private IRESettingsProvider _provider;

        public bool ToggleCompanionLimit { get => _provider.ToggleCompanionLimit; set => _provider.ToggleCompanionLimit = value; }

        public int CompanionLimit { get => _provider.CompanionLimit; set => _provider.CompanionLimit = value; }

        public string TemplateCharacter { get => _provider.TemplateCharacter; set => _provider.TemplateCharacter = value; }

        public RESettings()
        {
            if (RECustomSettings.Instance is not null)
            {
                _provider = RECustomSettings.Instance;
            }
            else
            {
                _provider = new HardcodedRESettings();
            }
        }
    }
}