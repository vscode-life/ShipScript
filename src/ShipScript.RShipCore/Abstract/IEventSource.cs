﻿using Microsoft.ClearScript;

namespace ShipScript.RShipCore
{
    public interface IEventSource
    {
        [ScriptMember("connect")]
        IEventConnection Connect(object callback);
    }
}
