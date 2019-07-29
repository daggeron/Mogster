using FFXIV_ACT_Plugin.Common.Models;
using FFXIV_ACT_Plugin.Memory;
using FFXIV_ACT_Plugin.Memory.MemoryProcessors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mogster.Core.Harness.FFXIVPlugin
{

    // Token: 0x02000003 RID: 3
    public class CombatantManager : ICombatantManager
    {
        // Token: 0x06000006 RID: 6 RVA: 0x00002050 File Offset: 0x00000250
        public CombatantManager(ICombatantProcessor combatantProcessor, IMobArrayProcessor mobArrayProcessor, IProcessManager processManager)
        {
            this._combatantProcessor = combatantProcessor;
            this._mobArrayProcessor = mobArrayProcessor;
            this._processManager = processManager;
            this._processManager.ProcessChanged += this.OnProcessChanged;
            this._combatants = new Combatant[0];
            this._readonlyCombatants = Array.AsReadOnly<Combatant>(this._combatants);
            this._lastCombatantRefresh = DateTime.MinValue;
        }

        // Token: 0x06000007 RID: 7 RVA: 0x000020C4 File Offset: 0x000002C4
        private void OnProcessChanged(object sender, EventArgs e)
        {
            object combatantLock = this._combatantLock;
            lock (combatantLock)
            {
                this._combatants = new Combatant[this._mobArrayProcessor.MobArray.Count];
                this._readonlyCombatants = Array.AsReadOnly<Combatant>(this._combatants);
                this.Reset();
            }
        }

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x06000008 RID: 8 RVA: 0x00002130 File Offset: 0x00000330
        public ReadOnlyCollection<Combatant> Combatants
        {
            get
            {
                object combatantLock = this._combatantLock;
                ReadOnlyCollection<Combatant> readonlyCombatants;
                lock (combatantLock)
                {
                    readonlyCombatants = this._readonlyCombatants;
                }
                return readonlyCombatants;
            }
        }

        // Token: 0x06000009 RID: 9 RVA: 0x00002174 File Offset: 0x00000374
        public Combatant GetCombatantById(uint CombatantID)
        {
            object combatantLock = this._combatantLock;
            lock (combatantLock)
            {
                if (this._combatants == null)
                {
                    return null;
                }
                for (int i = 0; i < this._combatants.Length; i++)
                {
                    if (this._combatants[i].ID == CombatantID && this._combatants[i].Pointer != IntPtr.Zero)
                    {
                        return this._combatants[i];
                    }
                }
            }
            return null;
        }

        // Token: 0x0600000A RID: 10 RVA: 0x00002204 File Offset: 0x00000404
        public Combatant GetCombatantByPointer(IntPtr pointer)
        {
            object combatantLock = this._combatantLock;
            lock (combatantLock)
            {
                if (this._combatants == null)
                {
                    return null;
                }
                for (int i = 0; i < this._combatants.Length; i++)
                {
                    if (this._combatants[i].Pointer == pointer && this._combatants[i].ID != 0u && this._combatants[i].ID != 3758096384u)
                    {
                        return this._combatants[i];
                    }
                }
            }
            return null;
        }

        // Token: 0x0600000B RID: 11 RVA: 0x000022A4 File Offset: 0x000004A4
        public void Reset()
        {
            object combatantLock = this._combatantLock;
            lock (combatantLock)
            {
                for (int i = 0; i < this._combatants.Length; i++)
                {
                    this._combatants[i] = new Combatant
                    {
                        Order = i
                    };
                }
                this._lastCombatantRefresh = DateTime.MinValue;
            }
        }

        public void Refresh()
        {
            IntPtr primaryPlayerPointer = this._mobArrayProcessor.PrimaryPlayerPointer;
            bool flag = false;
            if (DateTime.UtcNow.Subtract(this._lastCombatantRefresh).TotalMilliseconds > 10.0)
            {
                this._lastCombatantRefresh = DateTime.UtcNow;
                flag = true;
            }
            for (int i = 0; i < this._mobArrayProcessor.MobArray.Count; i++)
            {
                IntPtr intPtr = this._mobArrayProcessor.MobArray[i];
                Combatant combatant = this.Combatants[i];
                if (flag || intPtr != combatant.Pointer)
                {
                    this._combatantProcessor.RefreshCombatant(combatant, intPtr, flag);
                }
            }
        }

        private object _combatantLock = new object();
        private Combatant[] _combatants;
        private ReadOnlyCollection<Combatant> _readonlyCombatants;
        private DateTime _lastCombatantRefresh;
        private readonly ICombatantProcessor _combatantProcessor;
        private readonly IMobArrayProcessor _mobArrayProcessor;
        private readonly IProcessManager _processManager;
    }
}
