using System;
using UnityEngine;

namespace BuildingAnarchy
{
    [Serializable]
    public class SerializableColor
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public SerializableColor()
        {

        }

        public SerializableColor(Color32 color)
        {
            r = (float)color.r;
            g = (float)color.g;
            b = (float)color.b;
            a = (float)color.a;
        }

        public SerializableColor(Color color)
        {
            r = (float)color.r;
            g = (float)color.g;
            b = (float)color.b;
            a = (float)color.a;
        }

        public static implicit operator Color32(SerializableColor color)
        {
            return new Color32((byte)color.r, (byte)color.g, (byte)color.b, (byte)color.a);
        }

        public static implicit operator SerializableColor(Color32 color)
        {
            return new SerializableColor(color);
        }

        public static implicit operator Color(SerializableColor color)
        {
            return new Color(color.r, color.g, color.b, color.a);
        }

        public static implicit operator SerializableColor(Color color)
        {
            return new SerializableColor(color);
        }
    }
}
