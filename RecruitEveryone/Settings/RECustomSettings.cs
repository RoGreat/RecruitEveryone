using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Dropdown;
using MCM.Abstractions.Settings.Base.Global;

namespace RecruitEveryone.Settings
{
    internal class RECustomSettings : AttributeGlobalSettings<RECustomSettings>, IRESettingsProvider
    {
        public override string Id => "Settings";

        public override string DisplayName => "Recruit Everyone" + $" {typeof(RECustomSettings).Assembly.GetName().Version.ToString(3)}";

        public override string FolderName => "RecruitEveryone";

        public override string FormatType => "json2";

        private bool _toggleCompanionLimit = false;

        private int _companionLimit = 20;

        [SettingPropertyBool("Toggle Companion Limit", RequireRestart = false, IsToggle = true)]
        [SettingPropertyGroup("Companion Limit")]
        public bool ToggleCompanionLimit 
        { 
            get => _toggleCompanionLimit; 
            set
            {
                if (_toggleCompanionLimit != value)
                {
                    _toggleCompanionLimit = value;
                    OnPropertyChanged();
                }
            }
        }

        [SettingPropertyInteger("Companion Limit", minValue: 0, maxValue: 500, RequireRestart = false, HintText = "Set how many companions you can have in your party")]
        [SettingPropertyGroup("Companion Limit")]
        public int CompanionLimit
        {
            get => _companionLimit;
            set
            {
                if (_companionLimit != value)
                {
                    _companionLimit = value;
                    OnPropertyChanged();
                }
            }
        }

        [SettingPropertyDropdown("Template Character", RequireRestart = false, HintText = "Set which template will be used for the character's loadout and skillset")]
        public DropdownDefault<string> TemplateCharacterDropdown { get; set; } = new DropdownDefault<string>(new string[]
        {
            "Default",
            "Wanderer",
            "Lord"
        }, selectedIndex: 0);

        public string TemplateCharacter { get => TemplateCharacterDropdown.SelectedValue; set => TemplateCharacterDropdown.SelectedValue = value; }
    }
}