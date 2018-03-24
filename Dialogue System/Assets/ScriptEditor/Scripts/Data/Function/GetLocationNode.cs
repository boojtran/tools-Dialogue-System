﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptEditor.Graph {
    class GetLocationNode : FunctionNode {

        public override void Construct() {
            description = "Gets the location of the passed object.";
            name = "Get Location";

            // set input and output pins
            inPins.Add(new ValueInputPin(this, nT));
           outPins.Add(new ValueOutputPin(this, VarType.Vector3));
            Resize();
        }

        public override void Execute() {
            base.Execute();
        }
    }
}
