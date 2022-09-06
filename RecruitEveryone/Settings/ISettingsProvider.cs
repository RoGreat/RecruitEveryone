namespace RecruitEveryone.Settings
{
    internal interface ISettingsProvider
    {
        bool ToggleCompanionLimit { get; set; }

        int CompanionLimit { get; set; }

        string TemplateCharacter { get; set; }
    }
}