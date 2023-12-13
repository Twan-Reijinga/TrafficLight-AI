using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace SimulationAPI
{
    public class Car
    {
        public Vector2 size = new Vector2(2.5f, 6.5f);
        public int UUID = 0;
        public Vector2 pos;
        public float orientation;
        public int exitIndex;
        public char nextAction;

        private float velocity = 0;
        private float acceleration = 3;
        private float maxSpeed = 6;


        public Vector2 forward
        {
            get
            {
                return new Vector2((float)Math.Sin((Math.PI / 180) * orientation), (float)Math.Cos((Math.PI / 180) * orientation));
            }
        }

        public Vector2 right
        {
            get
            {
                return new Vector2((float)Math.Cos((Math.PI / 180) * -orientation), (float)Math.Sin((Math.PI / 180) * -orientation));
            }
        }

        public void Accelerate(float dt, float mult = 1) //pass mult as -1 for deceleration
        {
            velocity += acceleration * dt * mult;
            velocity = Math.Clamp(velocity, 0, maxSpeed);
        }

        public void Move(float dt)
        {
            Accelerate(dt);
            pos += forward * velocity * dt;
        }

    }
}