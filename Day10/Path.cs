using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day10
{
    public class Path
    {
        public List<Instruction> steps;
        public bool hasFoundEnd = false;

        public Path()
        {
            steps = new List<Instruction>();
        }
        public Path(List<Instruction> steps)
        {
            this.steps = steps;
        }

        // Add an instruction.
        // Returns false if unsuccessful, i.e. the steps already contains that instruction
        public bool Add(Instruction instruction)
        {
            if (AlreadyContainsInstruction(instruction)) return false;
            else
            {
                steps.Add(instruction);
                return true;
            }
        }

        // Add an instruction.
        // Returns false if unsuccessful, i.e. if either of all the paths already contains that instruction
        public bool Add(Instruction instruction, List<Path> otherPaths)
        {
            for (int i = 0; i < otherPaths.Count; i++)
            {
                if (otherPaths[i].AlreadyContainsInstruction(instruction)) return false;
            }

            steps.Add(instruction);
            return true;
        }

        public bool AlreadyContainsInstruction(Instruction instruction) => steps.Contains(instruction);
        public int Count => steps.Count;
        public Instruction CurrentInstruction => steps[^1];
    }
}
