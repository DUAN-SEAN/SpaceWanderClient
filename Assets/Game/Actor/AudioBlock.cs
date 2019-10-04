using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Actor
{
    [Serializable]
    public class AudioBlock
    {

        public float[] data;
        public int channels;
        public int samples;
        public int frequency;


        public bool Equals(AudioBlock a)
        {
            if (data.Length != a.data.Length) return false;
            if (channels != a.channels || samples != a.samples || frequency != a.frequency) return false;

            for (int i = 0; i < data.Length; i++)
            {
                if (Mathf.Abs(data[i] - a.data[i]) > 0.001f) return false;
            }

            return true;
        }
    }
}
