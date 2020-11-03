//using HarmonyLib;
//using MountAndBlade.CampaignBehaviors;
//using SandBox;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Xml;
//using TaleWorlds.CampaignSystem;
//using TaleWorlds.CampaignSystem.Actions;
//using TaleWorlds.Core;
//using TaleWorlds.Localization;
//using TaleWorlds.MountAndBlade;
//using TaleWorlds.ObjectSystem;
//using TaleWorlds.SaveSystem;

//namespace RecruitEveryone
//{
//    internal class REBasicCharacterObjectPatch : MBObjectBase
//    {
//        pu

//        public override void Deserialize(MBObjectManager objectManager, XmlNode node)
//        {
//            XmlNodeList childNodes = node.ChildNodes;
//            childNodes = node.ChildNodes;
//            Trace.WriteLine("Hello");
//            foreach (object obj in childNodes)
//            {
//                XmlNode xmlNode2 = (XmlNode)obj;
//                if (xmlNode2.Name == "face")
//                {
//                    foreach (object obj2 in xmlNode2.ChildNodes)
//                    {
//                        XmlNode xmlNode3 = (XmlNode)obj2;
//                        if (xmlNode3.Name == "BodyProperties")
//                        {
//                            BodyProperties bodyProperties;
//                            if (BodyProperties.FromXmlNode(xmlNode3, out bodyProperties))
//                            {
//                                dynamicBodyPropertiesMin = bodyProperties.DynamicProperties;
//                            }
//                        }
//                        else if (xmlNode3.Name == "BodyPropertiesMax")
//                        {
//                            BodyProperties bodyProperties2;
//                            if (BodyProperties.FromXmlNode(xmlNode3, out bodyProperties2))
//                            {
//                                dynamicBodyPropertiesMax = bodyProperties2.DynamicProperties;
//                            }
//                        }
//                        else if (xmlNode3.Name == "face_key_template")
//                        {
//                            DynamicBodyProperties dynamicBodyProperties;
//                            BasicCharacterObject basicCharacterObject = objectManager.ReadObjectReferenceFromXml<BasicCharacterObject>("value", xmlNode3);

//                            float Age = MBRandom.RandomFloatRanged(dynamicBodyPropertiesMin.Age, dynamicBodyPropertiesMax.Age);
//                            float Weight = MBRandom.RandomFloatRanged(dynamicBodyPropertiesMin.Weight, dynamicBodyPropertiesMax.Weight);
//                            float Build = MBRandom.RandomFloatRanged(dynamicBodyPropertiesMin.Build, dynamicBodyPropertiesMax.Build);

//                            Trace.WriteLine(Age + " " + Weight + " " + Build);

//                            dynamicBodyProperties = new DynamicBodyProperties(Age, Weight, Build);

//                            AccessTools.Field(typeof(BasicCharacterObject), "_dynamicBodyProperties").SetValue(basicCharacterObject, dynamicBodyProperties);
//                        }
//                    }
//                }
//            }
//        }

//        private DynamicBodyProperties dynamicBodyPropertiesMin;

//        private DynamicBodyProperties dynamicBodyPropertiesMax;
//    }
//}