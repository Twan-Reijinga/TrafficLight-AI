using System;
using UnityEngine.UIElements;

namespace SimulationAPI
{
    public class ActionNode
    {
        public Vector2 pos;
        public char action;
        public float orientation;
        public float size;

        public ActionNode(Vector2 pos, char action, float orientation, float size)
        {
            this.pos = pos;
            this.action = action;
            this.orientation = orientation;
            this.size = size;
        }
    }

    [Serializable]
    public class Car
    {
        public Vector2 size = new Vector2(2.5f, 6.5f);
        public int UUID;
        public Vector2 pos;
        public float orientation;
        private float orientationTarget;
        public int exitIndex;
        public char nextAction;
        public char currentAction = 'f';

        public float velocity = 0;
        private float acceleration = 3; // tweak this
        private float maxSpeed = 6;     // also tweak this
        private int switchphase = 0;

        public Car(int id, Vector2 pos, float orientation, int exitIndex, char nextAction)
        {
            this.UUID = id;
            this.pos = pos;
            this.orientation = orientation;
            this.exitIndex = exitIndex;
            this.nextAction = nextAction;
        }

        public Car(Car other)
        {
            this.UUID = other.UUID;
            this.pos = other.pos;
            this.orientation = other.orientation;
            this.orientationTarget = other.orientationTarget;
            this.exitIndex = other.exitIndex;
            this.nextAction = other.nextAction;
            this.currentAction = other.currentAction;
            this.velocity = other.velocity;
        }

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

        private void Accelerate(float dt, float mult = 1) //pass mult as -1 for deceleration
        {
            velocity += acceleration * dt * mult;
            velocity = Math.Clamp(velocity, 0, maxSpeed);
        }

        public void Move(float dt, Physics p)
        {
            RayHit hit;
            if (p.Raycast(pos, forward, 7, out hit, UUID))  //accelerate or decelerate
            {
                Accelerate(dt, -hit.maxDist * 2 / hit.dist);
            }
            else
            {
                Accelerate(dt);
            }

            //movement happens here
            ActionNode currentNode = GetActionNode();
            if (currentNode != null)
            {
                NodeLogic(currentNode);
            }

            switch (currentAction)
            {
                case 'f':
                    {
                        break;
                    }

                case 'l':
                    {
                        CarTurn(8f, dt);
                        break;
                    }

                case 'r':
                    {
                        CarTurn(4.5f, dt);
                        break;
                    }
                case 's':
                    {
                        if (switchphase == 0)
                        {
                            CarTurn(6 / (float)Math.Sqrt(2), dt, 45);
                        }
                        else
                        {
                            CarTurn(6 / (float)Math.Sqrt(2), dt, 45);
                        }
                        break;
                    }
            }
            MoveForward(dt);
        }

        private void MoveForward(float dt)
        {
            pos += forward * velocity * dt;
        }

        private ActionNode GetActionNode()
        {
            foreach (ActionNode node in CarMovement.nodes)
            {
                if (Vector2.Distance(pos, node.pos) < node.size)
                {
                    return node;
                }
            }
            return null;
        }

        private void NodeLogic(ActionNode node)
        {
            if (currentAction == 'f')
            {
                pos = node.pos;
                orientation = node.orientation;

                if (node.action == nextAction)
                {

                    currentAction = node.action;
                    orientationTarget = orientation + (currentAction == 'r' ? 90 : -90);

                    orientationTarget = (orientationTarget + 360) % 360;
                }
                else if (node.action == 's')
                {
                    switch (exitIndex)
                    {
                        case 1:
                        case 4:
                            {
                                orientationTarget = (orientation - 45 + 360) % 360;
                                //switch logic
                                currentAction = 's';
                                nextAction = 'l';
                                break;
                            }

                        case 2:
                        case 5:
                            {
                                nextAction = 'f';
                                break;
                            }

                        case 3:
                        case 6:
                            {
                                nextAction = 'r';
                                break;
                            }
                    }
                }
            }
        }

        private float RotateTowards(float current, float target, float step)
        {
            float distance = (target - current + 360) % 360;
            if (distance < step) return target;

            int direction = (distance <= 180) ? 1 : -1;

            return (current + step * direction + 360) % 360;
        }

        private void CarTurn(float radius, float dt, float angle = 90)
        {
            float arcLength = 2 * (float)Math.PI * radius * (angle / 360);
            float angularStepSize = angle / (arcLength / velocity) * dt;
            orientation = RotateTowards(orientation, orientationTarget, angularStepSize);

            if (orientation == RotateTowards(orientation, orientationTarget, angularStepSize) && velocity != 0)
            {
                orientation = orientationTarget;
                if (currentAction != 's' || switchphase == 1)
                {
                    currentAction = 'f';
                }
                if (currentAction == 's')
                {
                    switchphase = 1;
                    orientationTarget = (orientation + 45 + 360) % 360;
                }
            }
        }
    }
}