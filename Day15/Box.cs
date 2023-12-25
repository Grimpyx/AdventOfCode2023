using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Day15
{
    public class Box
    {
        //private int lenses = 0b0000_0000;
        //List<Lens> lenses = new List<Lens>();

        //Dictionary<string, byte> lenses = new Dictionary<string, byte>(); // string label,  byte focalLength

        public List<Lens> Lenses { get; private set; }

        public Box()
        {
            Lenses = [];
        }

        //public int Lenses { get => lenses; private set => lenses = value; }

        /*public void SetLens(byte lensNr, bool value)
        {
            if (lensNr < 1 || lensNr > 9) throw new IndexOutOfRangeException();

            if (value)
                Lenses |= 1 << (lensNr - 1);
            else
                Lenses &= ~(1 << (lensNr - 1));
        }*/

        public void AddLens(string label, byte focalLength)
        {
            string[] allLabels = [.. Lenses.Select(l => l.label)];

            if (allLabels.Contains(label)) // remove old one and insert new one at the same spot
            {
                int index = Lenses.IndexOf(Lenses.Where(l => l.label == label).First());
                Lenses[index].label = label;
                Lenses[index].focalLength = focalLength;
                //Lenses.RemoveAt(index);
                //Lenses.Insert(index, new Lens(label, focalLength));
            }
            else
            {
                Lenses.Add(new Lens(label, focalLength));
            }


            // Case 1, lens already exists so we replace it
            /*string[] matches = [.. lenses.Keys.Where(x => x == label)];
            if (matches )*/
        }

        public void RemoveLens(string label)
        {
            var matches = Lenses.Where(x => x.label == label).ToArray();
            if (!matches.Any()) return;

            int index = Lenses.IndexOf(matches[0]);
            //Lenses.RemoveAll(x => x.label == label);
            //Lenses[index] = Lens.Empty;
            Lenses.RemoveAt(index);

            //lenses = [.. lenses.OrderBy(x => x.focalLength)];

            /*if (!lenses.ContainsKey(label)) return;

            byte pos = lenses[label];

            lenses.Remove(label);
            //            lenses = [.. lenses.OrderBy(x => x.focalLength)];
            foreach (var key in lenses.Keys)
            {
                if (lenses[key] > pos) lenses[key]--;
            }*/



            /*for (byte i = 0; i < lenses.Count; i++)
            {
                lenses[i].focalLength = (byte)(i + 1); // move all focal lengths forward, range 1-9
            }*/
        }
    }

    public class Lens
    {
        public string label;
        public byte focalLength;
        public bool IsEmpty { get; private set; }

        public static Lens Empty = new Lens();

        public Lens(string label, byte focalLength)
        {
            this.label = label;
            this.focalLength = focalLength;
        }

        private Lens() => IsEmpty = true;

        public override bool Equals(object? obj)
        {
            if (obj is not Lens l) return false;
            return (l.label == label) && (l.focalLength == focalLength) && (l.IsEmpty == IsEmpty);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(label, focalLength);
        }

        public override string? ToString()
        {
            return $"(Label:{label}, Focal:{focalLength})";
        }
    }
}
