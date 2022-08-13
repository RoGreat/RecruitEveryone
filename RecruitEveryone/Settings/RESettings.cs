using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Dropdown;
using MCM.Abstractions.Settings.Base.Global;

namespace MarryAnyone.Settings
{
    internal class RESettings : AttributeGlobalSettings<RESettings>, IRESettingsProvider
    {
        public override string Id => "RESettings";

        public override string DisplayName => "Recruit Everyone" + $" {typeof(RESettings).Assembly.GetName().Version.ToString(3)}";

        public override string FolderName => "RecruitEveryone";

        public override string FormatType => "json2";


        [SettingPropertyBool("Toggle Companion Limit", RequireRestart = false, IsToggle = true)]
        [SettingPropertyGroup("Companion Limit")]
        public bool ToggleCompanionLimit { get; set; } = false;

        [SettingPropertyInteger("Companion Limit", minValue: 0, maxValue: 500, RequireRestart = false, HintText = "Set how many companions you can have in your party")]
        [SettingPropertyGroup("Companion Limit")]
        public int CompanionLimit { get; set; } = 20;

        [SettingPropertyBool("Template Character", RequireRestart = false, HintText = "Set which template will be used for the character's loadout and skillset")]
        public DropdownDefault<string> TemplateCharacterDropdown { get; set; } = new DropdownDefault<string>(new string[]
        {
            "Default",
            "Wanderer",
            "Lord"
        }, selectedIndex: 0);

        public string TemplateCharacter { get => TemplateCharacterDropdown.SelectedValue; set => TemplateCharacterDropdown.SelectedValue = value; }
    }
}