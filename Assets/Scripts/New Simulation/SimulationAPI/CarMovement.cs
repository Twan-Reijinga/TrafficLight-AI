namespace SimulationAPI
{
    public class CarMovement
    {
        public static ActionNode[] nodes = new ActionNode[]{
            new ActionNode(new Vector2( 26.5f,   7.5f), 'r', 180, 0.15f),   //'r': right
            new ActionNode(new Vector2( 32.5f, - 7.5f), 'r',   0, 0.15f),
            new ActionNode(new Vector2( 37.0f,   3.0f), 'r', 270, 0.15f),
            new ActionNode(new Vector2( 22.0f, - 3.0f), 'r',  90, 0.15f),

            new ActionNode(new Vector2(-26.5f, - 7.5f), 'r',   0, 0.15f),
            new ActionNode(new Vector2(-32.5f,   7.5f), 'r', 180, 0.15f),
            new ActionNode(new Vector2(-22.0f,   3.0f), 'r', 270, 0.15f),
            new ActionNode(new Vector2(-37.0f, - 3.0f), 'r',  90, 0.15f),

            new ActionNode(new Vector2( 29.5f, - 5.0f), 'l',   0, 0.15f),   //'l': left
            new ActionNode(new Vector2( 34.5f,   0.0f), 'l', 270, 0.15f),
            new ActionNode(new Vector2( 29.5f,   5.0f), 'l', 180, 0.15f),
            new ActionNode(new Vector2( 24.5f,   0.0f), 'l',  90, 0.25f),

            new ActionNode(new Vector2(-29.5f, - 5.0f), 'l',   0, 0.15f),
            new ActionNode(new Vector2(-34.5f,   0.0f), 'l',  90, 0.15f),
            new ActionNode(new Vector2(-29.5f,   5.0f), 'l', 180, 0.15f),
            new ActionNode(new Vector2(-24.5f,   0.0f), 'l', 270, 0.25f),

            new ActionNode(new Vector2(- 3.0f, - 3.0f), 's',  90, 0.25f),   //'s': switch
            new ActionNode(new Vector2(  3.0f,   3.0f), 's', 270, 0.25f)
        };

        public static float nodeSize = 0.15f;
    }
}