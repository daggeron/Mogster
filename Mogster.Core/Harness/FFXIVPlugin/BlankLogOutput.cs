using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Logfile;
using FFXIV_ACT_Plugin.Resource;
using Mogster.Core.Events;
using Mogster.Core.Helpers;
using Mogster.Core.Models;
using Prism.Events;

namespace Mogster.Core.Harness.FFXIVPlugin
{

    /// <summary>
    /// Implementation of the FFXIV_ACT_Plugin ILogOutput
    /// </summary>
    /// <remarks>
    /// Having issues with the DataSubscription part of FFXIV plugin.
    /// So since we have to override this class anyways I'm double dipping and handling events here for now.
    /// </remarks>
    public class BlankLogOutput : ILogOutput
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IEventAggregator eventAggregator;
        private readonly ISkillList skillList;

        public BlankLogOutput(ISettingsManager settingsManager, IEventAggregator eventAggregator, ISkillList skillList)
        {
            this.eventAggregator = eventAggregator;
            this.skillList = skillList;
        }

        public Thread ScanThread => throw new NotImplementedException();

        public void LogChangePrimaryPlayerEvent(DateTime timestamp, uint PlayerID, string PlayerName)
        {

        }

        public void LogChangeZone(DateTime timestamp, uint ZoneId, string ZoneName)
        {
        }

        public void LogChatMessage(uint eventType, string logline, uint seconds)
        {
        }

        public void LogCombatantEvent(DateTime timestamp, LogMessageType messageType, uint CombatantID, uint OwnerID, string CombatantName, int JobID, int Level, uint MaxHP, uint MaxMP, uint WorldID, string WorldName, uint BNpcNameID, uint BNpcID, float PosX, float PosY, float PosZ)
        {
        }

        public void LogError(DateTime timestamp, string errorText, Exception ex = null)
        {
        }

        public void LogIncomingAbilityEvent(DateTime timestamp, uint ActorID, string actor, uint TargetID, string target, uint SkillID, string skillname, uint[] effectData, uint CurrentTargetHP, uint MaxTargetHP)
        {
        }

        public void LogIncomingEvent(DateTime timestamp, int type1, int type2, uint SkillID, string Target, uint TargetID, int Amount, int ComboAmount, int unknown1, int unknown2, int unknown3, int unknown4, uint CurrentHP, uint MaxHP)
        {
        }

        public void LogMemoryBuffEvent(DateTime timestamp, LogMessageType messageType, uint BuffID, string Actor, uint ActorID, string Target, uint TargetID, float Timer, int Unknown1, uint TargetMaxHP)
        {
        }

        public void LogOutgoingEvent(DateTime timestamp, uint SkillID, uint ActorID, uint TargetID, IList<uint> otherTargets)
        {
        }

        public void LogPartyList(DateTime timestamp, int partyCount, ReadOnlyCollection<uint> partyList)
        {
        }

        public void LogPlayerStats(DateTime timestamp, uint JobID, uint Str, uint Dex, uint Vit, uint Intel, uint Mnd, uint Pie, uint Attack, uint DirectHit, uint Crit, uint AttackMagicPotency, uint HealMagicPotency, uint Det, uint SkillSpeed, uint SpellSpeed, uint Tenacity)
        {
        }

        public void LogTimer(DateTime timestamp, double ms, int combatantCount)
        {
        }

        public void LogVersion(FFXIVClientMode clientMode)
        {
        }

        public void StartWriting()
        {
        }

        public void StopWriting()
        {
        }

        public void WriteLine(LogMessageType messageType, DateTime ServerDate, string line)
        {
            eventAggregator.GetEvent<IPCMessageEvent>().Publish(IPCMessage.Get(MessageType.RawLogLine, String.Join("|", messageType.ToString(), ServerDate.ToString(), line + ":" + messageType.ToString())));

            switch (messageType)
            {
                case LogMessageType.NetworkDeath:
                    CombatDeath combatDeath = BobParser.ParseNetworkDeath(ServerDate, line);

                    eventAggregator.GetEvent<IPCMessageEvent>().Publish(IPCMessage.Get(MessageType.EntityHasDied, combatDeath));
                    break;
                case LogMessageType.NetworkStartsCasting:
                    NetworkCast networkCast = BobParser.ProcessNetworkCastMessage(ServerDate, line);

                    eventAggregator.GetEvent<IPCMessageEvent>().Publish(IPCMessage.Get(MessageType.StartCasting, networkCast));
                    break;
                case LogMessageType.Network6D:
                    ISystemMessage systemEvent = BobParser.ParseNetwork6D(ServerDate, line);

                    Console.WriteLine(systemEvent.MessageType);

                    eventAggregator.GetEvent<IPCMessageEvent>().Publish(IPCMessage.Get(systemEvent.MessageType, systemEvent));
                    break;
                case LogMessageType.NetworkBuff:
                    NetworkBuff networkBuff = BobParser.ParseNetworkEffect(ServerDate, line);
                    
                    eventAggregator.GetEvent<IPCMessageEvent>().Publish(IPCMessage.Get(MessageType.EffectAdded, networkBuff));
                    break;
                case LogMessageType.NetworkBuffRemove:
                    NetworkBuff networkBuffRemove = BobParser.ParseNetworkEffect(ServerDate, line);

                    eventAggregator.GetEvent<IPCMessageEvent>().Publish(IPCMessage.Get(MessageType.EffectRemoved, networkBuffRemove));
                    break;
            }
        }

    }
}
