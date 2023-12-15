using System.Collections.Generic;

namespace SimulationAPI
{
    public class G_Car
    {
        public int UUID = 0;
        public Vector2 pos;
        public float orientation = 0;
    }

    public class G_Lights
    {
        public List<bool> cross1 = new List<bool>();
        public List<bool> cross2 = new List<bool>();

    }


    public class G_sceneState
    {
        public List<Car> cars = new List<Car>();
        public G_Lights lights = new G_Lights();
    }
}