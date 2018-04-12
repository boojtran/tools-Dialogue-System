using System;

namespace ScriptEditor.Graph
{
	public class DelayNode : ControlNode
	{
		public DelayNode ()
		{
		}
		public override void Execute() {

        }

        public override void Construct() {
            // set information
            name = "Delay";
            description = "Delays execution for a given number of seconds";
            // Create pins
			execInPins.Add(new ExecInputPin(this));
            valInPins.Add(new ValueInputPin(this, VarType.Float));
            valInPins[0].Name = "Duration";
            valInPins[0].Default = 1.5f;

            execOutPins.Add(new ExecOutputPin(this));
          }
	}
}

