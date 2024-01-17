namespace SimulationAPI
{
    public class CarMovement
    {
        public enum Actions
        {
            FORWARD = 'f',
            RIGHT = 'r',
            LEFT = 'l',
            SWITCH = 's',
            EXIT = 'e'
        }

        public static ActionNode[] nodes = new ActionNode[]{
            new ActionNode(new Vector2( 26.5f,   7.5f), 'r', 180, 0.15f),   //'r': right
            new ActionNode(new Vector2( 32.5f, - 7.5f), 'r',   0, 0.15f),
            new ActionNode(new Vector2( 37.0f,   3.0f), 'r', 270, 0.15f),
            new ActionNode(new Vector2( 22.0f, - 3.0f), 'r',  90, 0.15f),

            new ActionNode(new Vector2(-26.5f, - 7.5f), 'r',   0, 0.15f),
            new ActionNode(new Vector2(-32.5f,   7.5f), 'r', 180, 0.15f),
            new ActionNode(new Vector2(-22.0f,   3.0f), 'r', 270, 0.15f),
            new ActionNode(new Vector2(-37.0f, - 3.0f), 'r',  90, 0.15f),

            new ActionNode(new Vector2( 29.5f, - 5.0f), 'l',   0, 0.25f),   //'l': left
            new ActionNode(new Vector2( 34.5f,   0.0f), 'l', 270, 0.25f),
            new ActionNode(new Vector2( 29.5f,   5.0f), 'l', 180, 0.25f),
            new ActionNode(new Vector2( 24.5f,   0.0f), 'l',  90, 0.25f),

            new ActionNode(new Vector2(-29.5f, - 5.0f), 'l',   0, 0.25f),
            new ActionNode(new Vector2(-34.5f,   0.0f), 'l',  90, 0.25f),
            new ActionNode(new Vector2(-29.5f,   5.0f), 'l', 180, 0.25f),
            new ActionNode(new Vector2(-24.5f,   0.0f), 'l', 270, 0.25f),

            new ActionNode(new Vector2(- 3.0f, - 3.0f), 's',  90, 0.25f),   //'s': switch
            new ActionNode(new Vector2(  3.0f,   3.0f), 's', 270, 0.25f),

            new ActionNode(new Vector2( 32.5f,  30.0f), 'e',   180, 0.25f),   //'e': exit
            new ActionNode(new Vector2( 60.0f, - 3.0f), 'e',   180, 0.25f),
            new ActionNode(new Vector2( 26.5f, -31.0f), 'e',   180, 0.25f),
            new ActionNode(new Vector2(-32.5f, -30.0f), 'e',   180, 0.25f),
            new ActionNode(new Vector2(-60.5f,   3.0f), 'e',   180, 0.25f),
            new ActionNode(new Vector2(-26.5f,  31.0f), 'e',   180, 0.25f)
        };

        public static ActionLine[] lines = new ActionLine[]{
            CreateActionLine(new Vector2(-28.0f, - 7.5f), new Vector2(-25.0f, - 7.5f), Actions.RIGHT,     0),
            CreateActionLine(new Vector2(-31.0f,   7.5f), new Vector2(-34.0f,   7.5f), Actions.RIGHT,   180),
            CreateActionLine(new Vector2(-37.0f, - 1.5f), new Vector2(-37.0f, - 4.5f), Actions.RIGHT,    90),
            CreateActionLine(new Vector2(-22.0f,   1.5f), new Vector2(-22.0f,   4.5f), Actions.RIGHT,   270),

            CreateActionLine(new Vector2( 31.0f, - 7.5f), new Vector2( 34.0f, - 7.5f), Actions.RIGHT,     0),
            CreateActionLine(new Vector2( 28.0f,   7.5f), new Vector2( 25.0f,   7.5f), Actions.RIGHT,   180),
            CreateActionLine(new Vector2( 22.0f, - 1.5f), new Vector2( 22.0f, - 4.5f), Actions.RIGHT,    90),
            CreateActionLine(new Vector2( 37.0f,   1.5f), new Vector2( 37.0f,   4.5f), Actions.RIGHT,   270),

            CreateActionLine(new Vector2(-31.0f,   5.0f), new Vector2(-28.0f,   5.0f), Actions.LEFT,    180),
            CreateActionLine(new Vector2(-31.0f, - 5.0f), new Vector2(-28.0f, - 5.0f), Actions.LEFT,      0),
            CreateActionLine(new Vector2(-24.5f, - 1.5f), new Vector2(-24.5f,   1.5f), Actions.LEFT,    270),
            CreateActionLine(new Vector2(-34.5f, - 1.5f), new Vector2(-34.5f,   1.5f), Actions.LEFT,     90),

            CreateActionLine(new Vector2( 31.0f, - 5.0f), new Vector2( 28.0f, - 5.0f), Actions.LEFT,      0),
            CreateActionLine(new Vector2( 31.0f,   5.0f), new Vector2( 28.0f,   5.0f), Actions.LEFT,    180),
            CreateActionLine(new Vector2( 24.5f,   1.5f), new Vector2( 24.5f, - 1.5f), Actions.LEFT,     90),
            CreateActionLine(new Vector2( 34.5f,   1.5f), new Vector2( 34.5f, - 1.5f), Actions.LEFT,    270),

            CreateActionLine(new Vector2(- 3.0f, - 1.5f), new Vector2(- 3.0f, - 4.5f), Actions.SWITCH,   90),
            CreateActionLine(new Vector2(  3.0f,   1.5f), new Vector2(  3.0f,   4.5f), Actions.SWITCH,  270),

            CreateActionLine(new Vector2( 31.0f,  30.0f), new Vector2( 34.0f,  30.0f), Actions.EXIT,      0),
            CreateActionLine(new Vector2( 25.0f, -30.0f), new Vector2( 28.0f, -30.0f), Actions.EXIT,    180),
            CreateActionLine(new Vector2( 59.5f, - 4.5f), new Vector2( 59.5f, - 1.5f), Actions.EXIT,     90),
            CreateActionLine(new Vector2(-31.0f, -30.0f), new Vector2(-34.0f, -30.0f), Actions.EXIT,    180),
            CreateActionLine(new Vector2(-25.0f,  30.0f), new Vector2(-28.0f,  30.0f), Actions.EXIT,      0),
            CreateActionLine(new Vector2(-59.5f,   4.5f), new Vector2(-59.5f,   1.5f), Actions.EXIT,    270),
        };

        public static float nodeSize = 0.15f;

        static ActionLine CreateActionLine(Vector2 p1, Vector2 p2, CarMovement.Actions action, float orientation)
        {
            return new ActionLine
            {
                p1 = p1,
                p2 = p2,
                action = action,
                orientation = orientation,
            };
        }
    }

    public class ActionLine
    {
        public Vector2 p1;
        public Vector2 p2;
        public float orientation;
        public CarMovement.Actions action;
    }
}