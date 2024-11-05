using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace National_Park_Simulator.World.Actions
{
    public class Action
    {
        public Visitor visitor;
        public bool active;

        public Action (Visitor v)
        {
            visitor = v;
            active = true;
        }

        public bool IsActionActive()
        {
            return active;
        }

        public virtual void Update(GameTime gameTime) { }
    }
}
