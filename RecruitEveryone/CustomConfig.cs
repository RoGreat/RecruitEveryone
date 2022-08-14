using System;
using System.IO;
using Newtonsoft.Json;

using TaleWorlds.Library;

namespace RecruitEveryone
{
    internal sealed class Config
    {
        public bool ToggleCompanionLimit { get; set; }
        public int CompanionLimit { get; set; }
        public string? TemplateCharacter { get; set; }
    }

    internal sealed class CustomConfig : ISettingsProvider
    {
        public static CustomConfig? Instance;

        private string _filePath = "..\\..\\Modules\\RecruitEveryone\\Config.json";

        public CustomConfig()
        {
            Instance = this;
            ReadConfig();
            WriteConfig();
        }

        private bool _toggleCompanionLimit = false;

        private int _companionLimit = 20;

        private string _templateCharacter = "Default";

        private void WriteConfig()
        {
            try
            {
                var config = new Config
                {
                    ToggleCompanionLimit = _toggleCompanionLimit,
                    CompanionLimit = _companionLimit,
                    TemplateCharacter = _templateCharacter
                };
                string jsonString = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(_filePath, jsonString);
            }
            catch (Exception e)
            {
                InformationManager.DisplayMessage(new InformationMessage($"{e.Message} Error when writing to Recruit Everyone's Config.json."));
            }
        }

        private void ReadConfig()
        {
            try
            {
                string jsonString = File.ReadAllText(_filePath);
                var config = JsonConvert.DeserializeObject<Config>(jsonString);
                _toggleCompanionLimit = config!.ToggleCompanionLimit;
                _companionLimit = config.CompanionLimit;
                _templateCharacter = config.TemplateCharacter!;
            }
            catch (Exception e)
            {
                InformationManager.DisplayMessage(new InformationMessage($"{e.Message} Error when reading from Recruit Everyone's Config.json."));
            }
        }

        public bool ToggleCompanionLimit
        {
            get
            {
                ReadConfig();
                return _toggleCompanionLimit;
            }
            set
            {
                if (_toggleCompanionLimit != value)
                {
                    _toggleCompanionLimit = value;
                    WriteConfig();
                }
            }
        }

        public int CompanionLimit
        {
            get
            {
                ReadConfig();
                return _companionLimit;
            }
            set
            {
                if (_companionLimit != value)
                {
                    _companionLimit = value;
                    WriteConfig();
                }
            }
        }

        public string TemplateCharacter
        {
            get
            {
                ReadConfig();
                return _templateCharacter;
            }
            set
            {
                if (_templateCharacter != value)
                {
                    _templateCharacter = value;
                    WriteConfig();
                }
            }
        }
    }
}