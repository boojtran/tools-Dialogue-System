﻿using System;

namespace ScriptEditor.Graph
{
	public class ControlNodeChoice : ControlNode {
		public ControlNodeChoice ()
		{

		}

        // called everyframe
        public override void Execute() {

        }

        public override void Construct() {
            // set information
            name = "Choice";
            description = "Splits execution based on an on-screen decision.";
            // Create pins
			multiplePins = true;
            inPins.Add(new InputPin(this, VarType.Exec));
            inPins.Add(new InputPin(this, VarType.Bool));
            inPins.Add(new InputPin(this, VarType.Bool));
            inPins[1].Name = "Condition 1";
            inPins[1].Default = true;
            inPins[2].Name = "Condition 2";
            inPins[2].Default = true;
          }

	}
}

