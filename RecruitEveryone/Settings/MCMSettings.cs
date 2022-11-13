using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using MCM.Common;

namespace RecruitEveryone.Settings
{
    internal sealed class MCMSettings : AttributeGlobalSettings<MCMSettings>, ISettingsProvider
    {
        public override string Id => "Settings";

        public override string DisplayName => "Recruit Everyone" + $" {typeof(MCMSettings).Assembly.GetName().Version.ToString(3)}";

        public override string FolderName => "RecruitEveryone";

        public override string FormatType => "json2";


        [SettingPropertyBool("{=togglecompanionlimit}Toggle Companion Limit", RequireRestart = false, IsToggle = true)]
        [SettingPropertyGroup("{=companionlimit}Companion Limit")]
        public bool ToggleCompanionLimit { get; set; }

        [SettingPropertyInteger("{=companionlimit}Companion Limit", minValue: 0, maxValue: 500, RequireRestart = false, HintText = "{=companionlimit_desc}Set how many companions you can have in your party.")]
        [SettingPropertyGroup("{=companionlimit}Companion Limit")]
        public int CompanionLimit { get; set; }

        [SettingPropertyDropdown("{=templatechar}Template Character", RequireRestart = false, HintText = "{=templatechar_desc}Set the template character that is used to set things like hero name, skills, and equipment.")]
        [SettingPropertyGroup("{=companion}Companion")]
        public Dropdown<string> TemplateCharacterDropdown { get; set; } = new Dropdown<string>(new string[]
        {
            "Default",
            "Wanderer"
        }, selectedIndex: 0);

        public string TemplateCharacter
        {
            get => TemplateCharacterDropdown.SelectedValue;
            set
            {
                TemplateCharacterDropdown.SelectedValue = value;
            }
        }
    }
}