using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Day10;

namespace Day10
{
    public class Instruction
    {
        public char character;
        public Vector2 coordinate;
        public Vector2[] connections;

        public enum InstructionType
        {
            None = '.',
            UpRight = 'L',
            UpLeft = 'J',
            DownLeft = '7',
            DownRight = 'F',
            Vertical = '|',
            Horizontal = '-',
            Start = 'S'
        }
        public InstructionType instructionType;

        public Instruction(char character, Vector2 coordinate)
        {
            instructionType = (InstructionType)character;
            this.character = character;

            this.coordinate = coordinate;

            // The instruction type decides where it leads.
            switch (instructionType)
            {
                case InstructionType.None:
                    connections = new Vector2[0];
                    break;
                case InstructionType.UpRight:
                    connections = new Vector2[2];
                    connections[0] = coordinate - new Vector2(0, 1); // up
                    connections[1] = coordinate + new Vector2(1, 0); // right
                    break;
                case InstructionType.UpLeft:
                    connections = new Vector2[2];
                    connections[0] = coordinate - new Vector2(0, 1); // up
                    connections[1] = coordinate - new Vector2(1, 0); // left
                    break;
                case InstructionType.DownLeft:
                    connections = new Vector2[2];
                    connections[0] = coordinate + new Vector2(0, 1); // down
                    connections[1] = coordinate - new Vector2(1, 0); // left
                    break;
                case InstructionType.DownRight:
                    connections = new Vector2[2];
                    connections[0] = coordinate + new Vector2(0, 1); // down
                    connections[1] = coordinate + new Vector2(1, 0); // right
                    break;
                case InstructionType.Vertical:
                    connections = new Vector2[2];
                    connections[0] = coordinate - new Vector2(0, 1); // up
                    connections[1] = coordinate + new Vector2(0, 1); // down
                    break;
                case InstructionType.Horizontal:
                    connections = new Vector2[2];
                    connections[0] = coordinate + new Vector2(1, 0); // right
                    connections[1] = coordinate - new Vector2(1, 0); // left
                    break;
                case InstructionType.Start:
                    connections = new Vector2[4];
                    connections[0] = coordinate - new Vector2(0, 1); // up
                    connections[1] = coordinate + new Vector2(0, 1); // down
                    connections[2] = coordinate + new Vector2(1, 0); // right
                    connections[3] = coordinate - new Vector2(1, 0); // left
                    break;
            }
        }

        // If this is connected to another instruction
        public bool IsConnectedTo(Instruction otherInstruction)
        {
            for (int i = 0; i < otherInstruction.connections.Length; i++)   // otherInstruction.connections
            {
                //Console.WriteLine("Comparing two vectors: " + otherInstruction.connections[i] + " and " + connections[j] + ".   " + (otherInstruction.connections[i] == connections[j]));
                if (otherInstruction.connections[i] == coordinate)
                {
                    //Console.WriteLine($"!!! {otherInstruction.coordinate} is connected to {coordinate}");
                    return true;
                }
            }
            return false;
        }

        public override bool Equals(object? obj)
        {
            return obj is Instruction instruction &&
                   character == instruction.character &&
                   coordinate.Equals(instruction.coordinate) &&
                   EqualityComparer<Vector2[]>.Default.Equals(connections, instruction.connections) &&
                   instructionType == instruction.instructionType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(character, coordinate);
        }
    }
}
