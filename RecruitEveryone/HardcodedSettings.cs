namespace RecruitEveryone
{
    internal sealed class HardcodedSettings : ISettingsProvider
    {
        public bool ToggleCompanionLimit { get; set; } = false;

        public int CompanionLimit { get; set; } = 20;

        public string TemplateCharacter { get; set; } = "Default";
    }
}