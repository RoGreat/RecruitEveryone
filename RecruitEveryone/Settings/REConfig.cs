using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using TaleWorlds.Library;

namespace RecruitEveryone.Settings
{
    // When not using MCM, use RecrutiEveryoneConfig
    internal sealed class RESettingsConfig : ISettingsProvider
    {
        public static RESettingsConfig? Instance { get; private set; }

        public RESettingsConfig()
        {
            Instance = this;
        }

        public string TemplateCharacter
        {
            get { return REConfig.TemplateCharacter; }
            set { REConfig.TemplateCharacter = value; }
        }

        public bool ToggleCompanionLimit
        {
            get { return REConfig.ToggleCompanionLimit; }
            set { REConfig.ToggleCompanionLimit = value; }
        }

        public int CompanionLimit
        {
            get { return REConfig.CompanionLimit; }
            set { REConfig.CompanionLimit = value; }
        }
    }

    // Adapted BannerlordConfig. Mainly for Steam Workshop compatibility.
    internal static class REConfig
    {
        private static string _templateCharacter = "Default";

        private static bool _toggleCompanionLimit = false;

        private static int _companionLimit = 20;

        [ConfigPropertyUnbounded]
        public static string TemplateCharacter
        {
            get
            {
                return _templateCharacter;
            }
            set
            {
                if (_templateCharacter != value)
                {
                    switch (_templateCharacter)
                    {
                        case "Default":
                            _templateCharacter = value;
                            break;
                        case "Wanderer":
                            _templateCharacter = value;
                            break;
                    }
                    Save();
                }
            }
        }

        [ConfigPropertyUnbounded]
        public static bool ToggleCompanionLimit
        {
            get
            {
                return _toggleCompanionLimit;
            }
            set
            {
                if (_toggleCompanionLimit != value)
                {
                    _toggleCompanionLimit = value;
                    Save();
                }
            }
        }

        [ConfigPropertyUnbounded]
        public static int CompanionLimit
        {
            get
            {
                return _companionLimit;
            }
            set
            {
                if (_companionLimit != value)
                {
                    _companionLimit = value;
                    Save();
                }
            }
        }

        public static void Initialize()
        {
            string text = REUtilities.LoadConfigFile();
            if (string.IsNullOrEmpty(text))
            {
                Save();
            }
            else
            {
                bool flag = false;
                string[] array = text.Split(new char[]
                {
                    '\n'
                });
                for (int i = 0; i < array.Length; i++)
                {
                    string[] array2 = array[i].Split(new char[]
                    {
                        '='
                    });
                    PropertyInfo property = typeof(REConfig).GetProperty(array2[0]);
                    if (property is null)
                    {
                        flag = true;
                    }
                    else
                    {
                        string text2 = array2[1];
                        try
                        {
                            if (property.PropertyType == typeof(string))
                            {
                                string value = Regex.Replace(text2, "\\r", "");
                                property.SetValue(null, value);
                            }
                            else if (property.PropertyType == typeof(float))
                            {
                                if (float.TryParse(text2, out float num))
                                {
                                    property.SetValue(null, num);
                                }
                                else
                                {
                                    flag = true;
                                }
                            }
                            else if (property.PropertyType == typeof(int))
                            {
                                if (int.TryParse(text2, out int num2))
                                {
                                    ConfigPropertyInt customAttribute = property.GetCustomAttribute<ConfigPropertyInt>();
                                    if (customAttribute is null || customAttribute.IsValidValue(num2))
                                    {
                                        property.SetValue(null, num2);
                                    }
                                    else
                                    {
                                        flag = true;
                                    }
                                }
                                else
                                {
                                    flag = true;
                                }
                            }
                            else if (property.PropertyType == typeof(bool))
                            {
                                if (bool.TryParse(text2, out bool flag2))
                                {
                                    property.SetValue(null, flag2);
                                }
                                else
                                {
                                    flag = true;
                                }
                            }
                            else
                            {
                                flag = true;
                            }
                        }
                        catch
                        {
                            flag = true;
                        }
                    }
                }
                if (flag)
                {
                    Save();
                }
            }
        }

        public static SaveResult Save()
        {
            Dictionary<PropertyInfo, object> dictionary = new();
            foreach (PropertyInfo propertyInfo in typeof(REConfig).GetProperties())
            {
                if (propertyInfo.GetCustomAttribute<ConfigProperty>() is not null)
                {
                    dictionary.Add(propertyInfo, propertyInfo.GetValue(null, null));
                }
            }
            string text = "";
            foreach (KeyValuePair<PropertyInfo, object> keyValuePair in dictionary)
            {
                text = string.Concat(new string[]
                {
                    text,
                    keyValuePair.Key.Name,
                    "=",
                    keyValuePair.Value.ToString(),
                    "\n"
                });
            }
            SaveResult result = REUtilities.SaveConfigFile(text);
            return result;
        }

        private interface IConfigPropertyBoundChecker<T>
        {
        }

        private abstract class ConfigProperty : Attribute
        {
        }

        private sealed class ConfigPropertyInt : ConfigProperty
        {
            public ConfigPropertyInt(int[] possibleValues, bool isRange = false)
            {
                _possibleValues = possibleValues;
                _isRange = isRange;
                bool isRange2 = _isRange;
            }

            public bool IsValidValue(int value)
            {
                if (_isRange)
                {
                    return value >= _possibleValues[0] && value <= _possibleValues[1];
                }
                int[] possibleValues = _possibleValues;
                for (int i = 0; i < possibleValues.Length; i++)
                {
                    if (possibleValues[i] == value)
                    {
                        return true;
                    }
                }
                return false;
            }

            private int[] _possibleValues;

            private bool _isRange;
        }

        private sealed class ConfigPropertyUnbounded : ConfigProperty
        {
        }
    }
}