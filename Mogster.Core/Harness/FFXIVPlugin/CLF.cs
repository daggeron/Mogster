using FFXIV_ACT_Plugin.Common;
using FFXIV_ACT_Plugin.Common.Models;
using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Logfile;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Mogster.Core.Harness.FFXIVPlugin
{
    // FFXIV_ACT_Plugin.Logfile.LogFormat


    public class CLF : ILogFormat
    {
        public string FormatVersion()
        {
            return "FFXIV PLUGIN VERSION: " + typeof(LogOutput).Assembly.GetName().Version;
        }

        public string FormatProcess(FFXIVClientMode clientMode, int? processId)
        {
            return "Process ID: " + (processId ?? 0).ToString() + ", Client Mode: " + clientMode.ToString();
        }

        public string FormatMemorySettings(int ProcessID, string LogFileFolder, bool DisableNetworkScan, bool EnableScanHPPercent, bool LogAllNetworkData, bool DisableCombatLog, string NetworkIP, bool UseWinPCap, bool UseSocketFilter)
        {
            return $"Selected Process ID: {ProcessID}, Disable Network Data: {DisableNetworkScan}, Enable HP Scan: {EnableScanHPPercent}, Dump All Network Data: {LogAllNetworkData}, Disable Combat Log: {DisableNetworkScan}, Selected IP: {NetworkIP}, WinPcap: {UseWinPCap}, Socket Filter: {UseSocketFilter}";
        }

        public string FormatParseSettings(bool DisableDamageShield, bool DisableCombinePets, bool GraphPotency, int LanguageID, ParseFilterMode ParseFilter, bool DisableNetworkScan, bool SimulateIndividualDoTCrits, bool ShowRealDoTTicks, bool ParsePotency)
        {
            return $"Language ID: {LanguageID}, Disable Damage Shield: {DisableDamageShield}, Disable Combine Pets: {DisableCombinePets}, Parse Filter: {ParseFilter}, DoTCrits: {SimulateIndividualDoTCrits}, RealDoTs: {ShowRealDoTTicks}, Parse Potency: {ParsePotency}";
        }

        public string FormatMemoryBuffMessage(uint BuffID, string Actor, uint ActorID, string Target, uint TargetID, float Timer, int Unknown1, uint? TargetMaxHP)
        {
            return Convert.ToString(BuffID, 16) + "|" + Convert.ToString(ActorID, 16) + "|" + Actor + "|" + Convert.ToString(TargetID, 16) + "|" + Target + "|" + Timer.ToString("F") + "|" + Convert.ToString(Unknown1, 16) + "|" + (TargetMaxHP ?? 0).ToString(CultureInfo.InvariantCulture);
        }

        public string FormatIncomingMessage(int type1, int type2, uint SkillID, string Target, uint TargetID, int Amount, int ComboAmount, int unknown1, int unknown2, int unknown3, int unknown4, uint? CurrentHP, uint? MaxHP)
        {
            return Convert.ToString(type1, 16) + "|" + Convert.ToString(type2, 16) + "|" + Convert.ToString(SkillID, 16) + "|" + Convert.ToString(TargetID, 16) + "|" + Target + "|" + Amount.ToString(CultureInfo.InvariantCulture) + "|" + ComboAmount.ToString(CultureInfo.InvariantCulture) + "|" + (CurrentHP ?? 0).ToString(CultureInfo.InvariantCulture) + "|" + (MaxHP ?? 0).ToString(CultureInfo.InvariantCulture);
        }

        public string FormatOutgoingMessage(uint SkillID, uint ActorID, uint TargetID, IList<uint> otherTargets)
        {
            string text = Convert.ToString(SkillID, 16) + "|" + Convert.ToString(ActorID, 16) + "|" + Convert.ToString(TargetID, 16);
            if (otherTargets != null)
            {
                text = text + "|" + string.Join("|", otherTargets);
            }
            return text;
        }

        public string FormatCombatantMessage(uint CombatantID, uint OwnerID, string CombatantName, int JobID, int Level, uint WorldID, string WorldName, uint BNpcNameID, uint BNpcID, uint currentHp, uint maxHp, uint currentMp, uint maxMp, float PosX, float PosY, float PosZ, float Heading)
        {
            return Convert.ToString(CombatantID, 16) + "|" + CombatantName + "|" + Convert.ToString(JobID, 16) + "|" + Convert.ToString(Level, 16) + "|" + Convert.ToString(OwnerID, 16) + "|" + Convert.ToString(WorldID, 16) + "|" + WorldName + "|" + BNpcNameID + "|" + BNpcID + "|" + FormatCombatantProperties(currentHp, maxHp, currentMp, maxMp, 0u, 0u, PosX, PosY, PosZ, Heading);
        }

        public string FormatIncomingAbilityMessage(uint ActorID, string actorName, uint TargetID, string targetName, uint SkillID, string skillname, uint[] effectData, uint? CurrentTargetHP, uint? MaxTargetHP)
        {
            string text = Convert.ToString(ActorID, 16) + "|" + actorName + "|";
            text = text + Convert.ToString(SkillID, 16) + "|" + skillname + "|";
            text = text + Convert.ToString(TargetID, 16) + "|" + targetName + "|";
            for (int i = 0; i < effectData.Length; i++)
            {
                text = text + Convert.ToString(effectData[i], 16) + "|";
            }
            text = text + (CurrentTargetHP ?? 0).ToString(CultureInfo.InvariantCulture) + "|";
            return text + (MaxTargetHP ?? 0).ToString(CultureInfo.InvariantCulture);
        }

        public string FormatChangeZoneMessage(uint ZoneId, string ZoneName)
        {
            return Convert.ToString(ZoneId, 16) + "|" + ZoneName;
        }

        public string FormatChangePrimaryPlayerMessage(uint? PlayerID, string PlayerName)
        {
            return Convert.ToString(PlayerID ?? 0, 16) + "|" + PlayerName;
        }

        public string FormatPartyMessage(int partyCount, ReadOnlyCollection<uint> partyList)
        {
            string text = partyCount.ToString(CultureInfo.InvariantCulture) + "|";
            if (partyList.Count > 0)
            {
                text += string.Join("|", (from x in partyList
                                          select Convert.ToString(x, 16)).ToArray());
            }
            return text;
        }

        public string FormatPlayerStatsMessage(uint JobID, uint Str, uint Dex, uint Vit, uint Intel, uint Mnd, uint Pie, uint Attack, uint DirectHit, uint Crit, uint AttackMagicPotency, uint HealMagicPotency, uint Det, uint SkillSpeed, uint SpellSpeed, uint Tenacity)
        {
            return string.Join("|", new uint[17]
            {
            JobID,
            Str,
            Dex,
            Vit,
            Intel,
            Mnd,
            Pie,
            Attack,
            DirectHit,
            Crit,
            AttackMagicPotency,
            HealMagicPotency,
            Det,
            SkillSpeed,
            SpellSpeed,
            0u,
            Tenacity
            });
        }

        public string FormatChatMessage(uint eventType, string logline)
        {
            string text = "";
            if (logline.IndexOf(":") > 0)
            {
                text = logline.Substring(0, logline.IndexOf(':')).Replace('\r', ' ').Replace('\n', ' ');
            }
            string text2 = logline.Substring(logline.IndexOf(':') + 1).Replace('\r', ' ').Replace('\n', ' ')
                .Replace('|', ':');
            return Convert.ToString(eventType, 16).PadLeft(4, '0') + "|" + text + "|" + text2;
        }

        public string FormatByteArray(byte[] data)
        {
            if (data == null)
            {
                return "";
            }
            StringBuilder stringBuilder = new StringBuilder(data.Length * 3);
            for (int i = 0; i < data.Length / 4; i++)
            {
                stringBuilder.Append(BitConverter.ToUInt32(data, i * 4).ToString("X8") + "|");
            }
            return stringBuilder.ToString();
        }

        public string FormatUIntArray(params uint[] param)
        {
            string text = "";
            for (int i = 0; i < param.Length; i++)
            {
                text = text + param[i].ToString("X2") + ((i < param.Length - 1) ? "|" : "");
            }
            return text;
        }

        public string FormatNetworkBuffMessage(ushort BuffID, string buffName, float Duration, uint ActorID, string ActorName, uint TargetID, string TargetName, ushort BuffExtra, uint? TargetMaxHP, uint? ActorMaxHP)
        {
            return BuffID.ToString("X2").ToLower() + "|" + buffName.ToProperCase() + "|" + Duration.ToString("0.00", CultureInfo.InvariantCulture) + "|" + ActorID.ToString("X4") + "|" + ActorName?.ToProperCase() + "|" + TargetID.ToString("X4") + "|" + TargetName?.ToProperCase() + "|" + BuffExtra.ToString("X2") + "|" + TargetMaxHP?.ToString(CultureInfo.InvariantCulture) + "|" + ActorMaxHP?.ToString(CultureInfo.InvariantCulture) + "|";
        }

        public string FormatNetworkLimitBreakfMessage(byte maxLB, uint limitbreak)
        {
            return limitbreak.ToString("X4") + "|" + maxLB.ToString();
        }

        public string FormatNetworkAoeAbilityMessage(uint actorId, string actorName, uint skillId, string skillName, uint targetId, string targetName, uint? actorCurrentHp, uint? actorMaxHp, uint? actorCurrentMp, uint? actorMaxMp, uint? actorCurrentTp, uint? actorMaxTp, float? actorPosX, float? actorPosY, float? actorPosZ, float? actorHeading, uint? targetCurrentHp, uint? targetMaxHp, uint? targetCurrentMp, uint? targetMaxMp, uint? targetCurrentTp, uint? targetMaxTp, float? targetPosX, float? targetPosY, float? targetPosZ, float? targetHeading, ulong effectData1, ulong effectData2, ulong effectData3, ulong effectData4, ulong effectData5, ulong effectData6, ulong effectData7, ulong effectData8)
        {
            return FormatNetworkAbilityMessage(actorId, actorName, skillId, skillName, targetId, targetName, 
                actorCurrentHp, actorMaxHp, actorCurrentMp, actorMaxMp, actorCurrentTp, actorMaxTp, actorPosX, 
                actorPosY, actorPosZ, actorHeading, targetCurrentHp, targetMaxHp, targetCurrentMp, targetMaxMp, 
                targetCurrentTp, targetMaxTp, targetPosX, targetPosY, targetPosZ, targetHeading, effectData1, 
                effectData2, effectData3, effectData4, effectData5, effectData6, effectData7, effectData8);
        }
        public string FormatNetworkAbilityMessage(uint actorId, string actorName, uint skillId, string skillName, uint targetId, string targetName, uint? actorCurrentHp, uint? actorMaxHp, uint? actorCurrentMp, uint? actorMaxMp, uint? actorCurrentTp, uint? actorMaxTp, float? actorPosX, float? actorPosY, float? actorPosZ, float? actorHeading, uint? targetCurrentHp, uint? targetMaxHp, uint? targetCurrentMp, uint? targetMaxMp, uint? targetCurrentTp, uint? targetMaxTp, float? targetPosX, float? targetPosY, float? targetPosZ, float? targetHeading, ulong effectData1, ulong effectData2, ulong effectData3, ulong effectData4, ulong effectData5, ulong effectData6, ulong effectData7, ulong effectData8)
        {
            string str = actorId.ToString("X2") + "|";
            str = str + actorName + "|";
            str = str + skillId.ToString("X2") + "|";
            str = str + skillName?.ToProperCase() + "|";
            str = str + targetId.ToString("X2") + "|";
            str = str + targetName + "|";
            str = str + (effectData1 & uint.MaxValue).ToString("X1") + "|" + (effectData1 >> 32).ToString("X1") + "|";
            str = str + (effectData2 & uint.MaxValue).ToString("X1") + "|" + (effectData2 >> 32).ToString("X1") + "|";
            str = str + (effectData3 & uint.MaxValue).ToString("X1") + "|" + (effectData3 >> 32).ToString("X1") + "|";
            str = str + (effectData4 & uint.MaxValue).ToString("X1") + "|" + (effectData4 >> 32).ToString("X1") + "|";
            str = str + (effectData5 & uint.MaxValue).ToString("X1") + "|" + (effectData5 >> 32).ToString("X1") + "|";
            str = str + (effectData6 & uint.MaxValue).ToString("X1") + "|" + (effectData6 >> 32).ToString("X1") + "|";
            str = str + (effectData7 & uint.MaxValue).ToString("X1") + "|" + (effectData7 >> 32).ToString("X1") + "|";
            str = str + (effectData8 & uint.MaxValue).ToString("X1") + "|" + (effectData8 >> 32).ToString("X1") + "|";
            str += FormatCombatantProperties(targetCurrentHp, targetMaxHp, targetCurrentMp, targetMaxMp, targetCurrentTp, targetMaxTp, targetPosX, targetPosY, targetPosZ, targetHeading);
            return str + FormatCombatantProperties(actorCurrentHp, actorMaxHp, actorCurrentMp, actorMaxMp, actorCurrentTp, actorMaxTp, actorPosX, actorPosY, actorPosZ, actorHeading);
        }

        public string FormatNetworkCastMessage(uint actorId, string actorName, uint targetId, string targetName, uint skillId, string skillName)
        {
            return actorId.ToString("X2") + "|" + actorName + "|" + skillId.ToString("X2") + "|" + skillName.ToProperCase() + "|" + targetId.ToString("X2") + "|" + targetName + "|";
        }

        public string FormatNetworkMarkerMessage(NetworkMarker.MarkerEventEnum markerEvent, uint markerId, uint targetId, uint actorId)
        {
            return markerEvent.ToString() + "|" + markerId.ToString() + "|" + targetId.ToString("X4") + "|" + actorId.ToString("X4");
        }

        public string FormatNetworkDoTMessage(uint targetId, string targetName, bool isHeal, uint buffId, uint amount, uint? targetCurrentHp, uint? targetMaxHp, uint? targetCurrentMp, uint? targetMaxMp, uint? targetCurrentTp, uint? targetMaxTp, float? targetPosX, float? targetPosY, float? targetPosZ, float? targetHeading)
        {
            return targetId.ToString("X2") + "|" + targetName + "|" + (isHeal ? "HoT|" : "DoT|") + buffId.ToString("X1") + "|" + amount.ToString("X1") + "|" + FormatCombatantProperties(targetCurrentHp, targetMaxHp, targetCurrentMp, targetMaxMp, targetCurrentTp, targetMaxTp, targetPosX, targetPosY, targetPosZ, targetHeading);
        }

        public string FormatNetworkCancelMessage(uint targetId, string targetName, uint skillId, string skillName, bool cancelled, bool interrupted)
        {
            string str = targetId.ToString("X2") + "|";
            str = str + targetName + "|";
            str = str + skillId.ToString("X2") + "|";
            str = str + skillName?.ToProperCase() + "|";
            if (cancelled)
            {
                return str + "Cancelled|";
            }
            if (interrupted)
            {
                return str + "Interrupted|";
            }
            return str + "|";
        }

        public string FormatNetworkDeathMessage(uint targetId, string targetName, uint actorId, string actorName)
        {
            return targetId.ToString("X2") + "|" + targetName + "|" + actorId.ToString("X2") + "|" + actorName + "|";
        }

        public string FormatNetworkTargetIconMessage(uint targetId, string targetName, uint param1, uint param2, uint param3, uint param4, uint param5, uint param6)
        {
            return targetId.ToString("X2") + "|" + targetName + "|" + param1.ToString("X4") + "|" + param2.ToString("X4") + "|" + param3.ToString("X4") + "|" + param4.ToString("X4") + "|" + param5.ToString("X4") + "|" + param6.ToString("X4") + "|";
        }

        public string FormatNetworkTargettableMessage(uint targetId, string targetName, uint actorId, string actorName, byte param)
        {
            return targetId.ToString("X2") + "|" + targetName + "|" + actorId.ToString("X2") + "|" + actorName + "|" + param.ToString("X2");
        }

        public string FormatNetworkTetherMessage(uint targetId, string targetName, uint actorId, string actorName, uint param1, uint param2, uint param3, uint param4, uint param5, uint param6)
        {
            return targetId.ToString("X2") + "|" + targetName + "|" + actorId.ToString("X2") + "|" + actorName + "|" + param1.ToString("X4") + "|" + param2.ToString("X4") + "|" + param3.ToString("X4") + "|" + param4.ToString("X4") + "|" + param5.ToString("X4") + "|" + param6.ToString("X4") + "|";
        }

        private string FormatCombatantProperties(uint? currentHp, uint? maxHp, uint? currentMp, uint? maxMp, uint? currentTp, uint? maxTp, float? posX, float? posY, float? posZ, float? heading)
        {
            return currentHp?.ToString(CultureInfo.InvariantCulture) + "|" + maxHp?.ToString(CultureInfo.InvariantCulture) + "|" + currentMp?.ToString(CultureInfo.InvariantCulture) + "|" + maxMp?.ToString(CultureInfo.InvariantCulture) + "|" + currentTp?.ToString(CultureInfo.InvariantCulture) + "|" + maxTp?.ToString(CultureInfo.InvariantCulture) + "|" + posX?.ToString(CultureInfo.InvariantCulture) + "|" + posY?.ToString(CultureInfo.InvariantCulture) + "|" + posZ?.ToString(CultureInfo.InvariantCulture) + "|" + heading?.ToString(CultureInfo.InvariantCulture) + "|";
        }
    }

}
