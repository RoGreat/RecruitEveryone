namespace RecruitEveryone
{
    internal sealed class HardcodedSettings : ISettingsProvider
    {
        public static HardcodedSettings? Instance;

        public HardcodedSettings()
        {
            Instance = this;
        }

        private bool _toggleCompanionLimit = false;

        private int _companionLimit = 20;

        private string _templateCharacter = "Default";

        public bool ToggleCompanionLimit
        {
            get => _toggleCompanionLimit;
            set
            {
                if (_toggleCompanionLimit != value)
                {
                    _toggleCompanionLimit = value;
                }
            }
        }

        public int CompanionLimit
        {
            get => _companionLimit;
            set
            {
                if (_companionLimit != value)
                {
                    _companionLimit = value;
                }
            }
        }

        public string TemplateCharacter
        {
            get => _templateCharacter;
            set
            {
                if (_templateCharacter != value)
                {
                    _templateCharacter = value;
                }
            }
        }
    }
}