using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Dropdown;
using MCM.Abstractions.Settings.Base.Global;

namespace RecruitEveryone
{
    internal sealed class MCMSettings : AttributeGlobalSettings<MCMSettings>, ISettingsProvider
    {
        public override string Id => "Settings";

        public override string DisplayName => "Recruit Everyone" + $" {typeof(MCMSettings).Assembly.GetName().Version.ToString(3)}";

        public override string FolderName => "RecruitEveryone";

        public override string FormatType => "json2";


        [SettingPropertyBool("Toggle Companion Limit", RequireRestart = false, IsToggle = true)]
        [SettingPropertyGroup("Companion Limit")]
        public bool ToggleCompanionLimit { get; set; }

        [SettingPropertyInteger("Companion Limit", minValue: 0, maxValue: 500, RequireRestart = false, HintText = "Set how many companions you can have in your party")]
        [SettingPropertyGroup("Companion Limit")]
        public int CompanionLimit { get; set; }

        [SettingPropertyDropdown("Template Character", RequireRestart = false, HintText = "Set the template character that is used to set things like hero name, skills, and equipment")]
        [SettingPropertyGroup("Hero")]
        public DropdownDefault<string> TemplateCharacterDropdown { get; set; } = new DropdownDefault<string>(new string[]
        {
            "Default",
            "Wanderer"
        }, selectedIndex: 0);

        public string TemplateCharacter 
        { 
            get => TemplateCharacterDropdown.SelectedValue;
            set
            {
                if (TemplateCharacterDropdown.SelectedValue != value)
                {
                    TemplateCharacterDropdown.SelectedValue = value;
                }
            }
        }
    }
}