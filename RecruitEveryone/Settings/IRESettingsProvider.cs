namespace RecruitEveryone.Settings
{
    internal interface IRESettingsProvider
    {
        bool ToggleCompanionLimit { get; set; }

        int CompanionLimit { get; set; }

        string TemplateCharacter { get; set; }
    }
}