using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using National_Park_Simulator.World;
using System.Diagnostics;
using National_Park_Simulator.Engine;
using National_Park_Simulator.Engine.Managers;
using System.Collections.Generic;
using National_Park_Simulator.World.Actions;
using Action = National_Park_Simulator.World.Actions.Action;

namespace National_Park_Simulator.World
{
    public enum Personality
    {
        Adventurer,
        Thrill_Seeker,
        Naturalist,
        Photographer,
        Cozy_Camper
    }

    public enum Fitness
    {
        Athletic,
        Average,
        Couch_Potato
    }

    public enum Gender
    {
        Male,
        Female
    }

    public class Visitor
    {
        public Point Location;
        private double timer;
        public int Age;
        public Gender Gender;
        public Personality PrimaryPersonality;
        public Personality SecondaryPersonality;
        public Fitness Fitness;
        public Action currentAction;
        Random rng;

        public Visitor()
        {
            rng = new Random();
            Age = rng.Next(5, 60);
            Gender = (Gender)rng.Next((int)Gender.Male, (int)Gender.Female);
            Fitness = (Fitness)rng.Next((int)Fitness.Athletic, (int)Fitness.Couch_Potato);
            PrimaryPersonality = (Personality)rng.Next((int)Personality.Adventurer, (int)Personality.Cozy_Camper);
            SecondaryPersonality = (Personality)rng.Next((int)Personality.Adventurer, (int)Personality.Cozy_Camper);
        }

        public void Update(GameTime gameTime, Map map)
        {
            // Decide on what to do
            if (currentAction.IsActionActive())
                currentAction = decideAction(map);
            else
                currentAction.Update(gameTime, map);

        }

        private Action decideAction(Map map)
        {
            //For now default to hiking until we build camp grounds
            return new Hike(this, map.Trails[rng.Next(map.Trails.Count)]);
        }
    }
}
