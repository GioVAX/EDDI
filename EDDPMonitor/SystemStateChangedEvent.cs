﻿using EddiDataDefinitions;
using EddiEvents;
using System;
using System.Collections.Generic;

namespace EddiEddpMonitor
{
    class SystemStateChangedEvent : Event
    {
        public const string NAME = "System state changed";
        public const string DESCRIPTION = "Triggered when there is a change in the state of a watched system";
        public static Dictionary<string, string> VARIABLES = new Dictionary<string, string>();
        public static SystemStateChangedEvent SAMPLE = new SystemStateChangedEvent(DateTime.UtcNow, "home", "Shinrarta Dezhra", FactionState.FromEDName("CivilUnrest"), FactionState.FromEDName("CivilWar"));

        static SystemStateChangedEvent()
        {
            VARIABLES.Add("match", "The name of the pattern that this event matched");
            VARIABLES.Add("system", "The name of the system");
            VARIABLES.Add("oldstate", "The old state of the system");
            VARIABLES.Add("newstate", "The new state of the system");
        }

        public string match { get; private set; }

        public string system { get; private set; }

        public FactionState oldSystemState { get; private set; }
        public FactionState newSystemState { get; private set; }

        [Obsolete("Please use oldSystemState instead")]
        public string oldstate => oldSystemState.localizedName;
        [Obsolete("Please use newSystemState instead")]
        public string newstate => newSystemState.localizedName;

        public SystemStateChangedEvent(DateTime timestamp, string match, string system, FactionState oldSystemState, FactionState newSystemState) : base(timestamp, NAME)
        {
            this.match = match;
            this.system = system;
            this.oldSystemState = oldSystemState;
            this.newSystemState = newSystemState;
        }
    }
}
