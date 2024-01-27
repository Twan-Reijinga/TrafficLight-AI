using System;

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
        public event EventHandler<CarPassEventArgs> pass;
        public Vector2 size = new Vector2(2f, 4.8f);
        public int UUID;
        public Vector2 pos;
        public float orientation;
        private float orientationTarget;
        public int exitIndex;
        public CarMovement.Actions nextAction;
        public CarMovement.Actions currentAction = CarMovement.Actions.FORWARD;
        public bool isDestroyed = false;

        private ActionLine lastLine = null;

        public float velocity = 0;
        private float acceleration = 3; // tweak this
        private float maxSpeed = 6;     // also tweak this
        private int switchphase = 0;

        public bool WaitingAtTraficlight;
        public int LastTraficLight;

        public Car(int id, Vector2 pos, float orientation, int exitIndex, CarMovement.Actions nextAction)
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

        public void Move(float dt, Simulator super)
        {
            RayHit hit;
            if (super.Raycast(pos + forward * size.y / 2, forward, 4, out hit, UUID))  //accelerate or decelerate
            {
                Accelerate(dt, -hit.maxDist * 2 / hit.dist);
            }
            else
            {
                Accelerate(dt);
            }

            //movement happens here
            ActionLine currentLine = GetActionLine();
            if (lastLine != null && currentLine != lastLine)
            {
                NodeLogic(lastLine);
            }
            lastLine = currentLine;

            switch (currentAction)
            {
                case CarMovement.Actions.FORWARD:
                    {
                        break;
                    }

                case CarMovement.Actions.LEFT:
                    {
                        CarTurn(8f, dt);
                        break;
                    }

                case CarMovement.Actions.RIGHT:
                    {
                        CarTurn(4.5f, dt);
                        break;
                    }
                case CarMovement.Actions.SWITCH:
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

        private ActionLine GetActionLine()
        {
            foreach (ActionLine line in CarMovement.lines)
            {
                if (Vector2.Distance(Physics.LineLineIntersect(this.pos, this.pos + this.forward, line.p1, line.p2) ?? Vector2.positiveInfinity, this.pos) < 2.0f)
                {
                    return line;
                }
            }
            return null;
        }

        private void NodeLogic(ActionLine line)
        {
            if (currentAction == CarMovement.Actions.FORWARD)
            {
                pos = Vector2.Lerp(line.p1, line.p2, 0.5f);
                orientation = line.orientation;

                if (line.action == nextAction)
                {

                    currentAction = line.action;
                    orientationTarget = orientation + (currentAction == CarMovement.Actions.RIGHT ? 90 : -90);

                    orientationTarget = (orientationTarget + 360) % 360;
                }
                else if (line.action == CarMovement.Actions.SWITCH)
                {
                    switch (exitIndex)
                    {
                        case 1:
                        case 4:
                            {
                                orientationTarget = (orientation - 45 + 360) % 360;
                                //switch logic
                                currentAction = CarMovement.Actions.SWITCH;
                                nextAction = CarMovement.Actions.LEFT;
                                break;
                            }

                        case 2:
                        case 5:
                            {
                                nextAction = CarMovement.Actions.FORWARD;
                                break;
                            }

                        case 3:
                        case 6:
                            {
                                nextAction = CarMovement.Actions.RIGHT;
                                break;
                            }
                    }
                }
            }
            if (line.action == CarMovement.Actions.EXIT)
            {
                // currentAction = 'l';
                isDestroyed = true;
            }
            if (line.action == CarMovement.Actions.IEXIT)
            {
                this.pass?.Invoke(this, new CarPassEventArgs() { intersection = line.intersection });
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
                if (currentAction != CarMovement.Actions.SWITCH || switchphase == 1)
                {
                    currentAction = CarMovement.Actions.FORWARD;
                }
                if (currentAction == CarMovement.Actions.SWITCH)
                {
                    switchphase = 1;
                    orientationTarget = (orientation + 45 + 360) % 360;
                }
            }
        }

        private string DetectTraficLight()
        {
            return "Test";
        }
    }
}