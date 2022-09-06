using RecruitEveryone.Settings;

namespace RecruitEveryone
{
    internal sealed class RESettings
    {
        private readonly ISettingsProvider _provider;

        public bool ToggleCompanionLimit 
        { 
            get => _provider.ToggleCompanionLimit; 
            set => _provider.ToggleCompanionLimit = value; 
        }

        public int CompanionLimit 
        { 
            get => _provider.CompanionLimit; 
            set => _provider.CompanionLimit = value; 
        }

        public string TemplateCharacter
        {
            get => _provider.TemplateCharacter;
            set => _provider.TemplateCharacter = value;
        }

        public RESettings()
        {
            if (MCMSettings.Instance is not null)
            {
                _provider = MCMSettings.Instance;
                return;
            }
            else if (REConfig.Instance is null)
            {
                new REConfig();
            }
            _provider = REConfig.Instance!;
        }
    }
}