using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SimulationAPI
{
    [Serializable]
    public class Replay
    {
        List<ReplayCar> carList { get; set; }
        List<ReplayLight> lightList { get; set; }

        public Replay()
        {
            carList = new List<ReplayCar>();
            lightList = new List<ReplayLight>();
            carList.Add(new ReplayCar { frame = 0, startIndex = 1, endIndex = 0 });
            carList.Add(new ReplayCar { frame = 4, startIndex = 12, endIndex = 2 });
            carList.Add(new ReplayCar { frame = 2, startIndex = 3, endIndex = 3 });
            lightList.Add(new ReplayLight { frame = 0, isOn = false });
        }

        public void BinarySave(string filePath)
        {
            BinarySerialization.WriteToBinaryFile(filePath, this);
        }

        public void BinaryLoad(string filePath)
        {
            Replay replay = BinarySerialization.ReadFromBinaryFile<Replay>(filePath);
            carList = replay.carList;
            lightList = replay.lightList;
        }
    }

    [Serializable]
    class ReplayCar
    {
        public int frame { get; set; }
        public int startIndex { get; set; }
        public int endIndex { get; set; }
    }

    [Serializable]
    class ReplayLight
    {
        public int frame { get; set; }
        public bool isOn { get; set; }
    }
}