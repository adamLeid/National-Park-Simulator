using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace National_Park_Simulator.World.Actions
{
    public class Hike : Action
    {
        private Trail trail;
        public Hike(Visitor v, Trail trail) : base(v)
        {
            this.trail = trail;
        }

        public override void Update(GameTime gameTime)
        {
            /**
             * TODO: Implement A* path search for current location path to trail head 
             */


            // Move toward location every second
            timer += gameTime.ElapsedGameTime.TotalSeconds;
            if (timer >= 1d)
            {
                timer = 0;
            }
        }
    }
}
