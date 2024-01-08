using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day20
{
    abstract class Module
    {
        public static int totalHighPulses = 0; // Total high pulses sent for all modules
        public static int totalLowPulses = 0;  // Total low  pulses sent for all modules

        public int SentHighCount { get; protected set; } // How many high pulses THIS module has sent
        public int SentLowCount { get; protected set; }  // How many low  pulses THIS module has sent

        public string name;
        private int queueWeight = 0; // Incremented and used when adding to PriorityQueue to ensure each new entry is in the back of the queue

        public static Dictionary<string, Module> moduleDict = new Dictionary<string, Module>();

        public PriorityQueue<(Pulse pulse, Module sender), int> receiveQueue;

        public List<string> destinationModules = new List<string>();
        public List<Module> connectedModules = new List<Module>();

        protected Module(string name, List<string> destinationModules)
        {
            this.name = name;
            this.destinationModules = destinationModules;
            moduleDict.Add(name, this);

            receiveQueue = new PriorityQueue<(Pulse, Module), int>();
        }

        // What happens on receive pulse
        protected abstract void ReceivePulse(Pulse p, Module sender);

        // How broadcasting is done
        protected virtual void BroadcastPulse(Pulse p)
        {
            foreach (var item in destinationModules)
            {
                Module m = GetOrCreateModule(item);
                switch (p) 
                {
                    case Pulse.High:
                        SentHighCount++;
                        break;
                    case Pulse.Low:
                        SentLowCount++;
                        break;
                    default:
                        break;
                }
                m.QueueReceive(p, this);
            }
        }

        // If not found in the moduledict, it creates a new OutputModule (empty that only receives signals)
        protected Module GetOrCreateModule(string moduleName)
        {
            if (moduleDict.TryGetValue(moduleName, out Module m))
            {
                return m;
            }
            else
            {
                Module newm = new OutputModule(moduleName, []);
                newm.InitializeConnections();
                return newm;
            }
        }

        // Add a receive pulse to its queue
        public void QueueReceive(Pulse p, Module sender)
        {
            receiveQueue.Enqueue((p,sender), queueWeight);
            queueWeight++;

            switch (p) 
            {
                case Pulse.High:
                    totalHighPulses++;
                    break;
                case Pulse.Low:
                    totalLowPulses++;
                    break;
                default:
                    break;
            }
        }

        // Initialization that runs after ALL modules have been created
        public virtual void InitializeConnections()
        {
            connectedModules.Clear();

            // Find all connections to This and add a reference to it
            var allModules = moduleDict.Values;
            foreach (var item in allModules.ToArray())
            {
                foreach (var s in item.destinationModules)
                {
                    Module m = GetOrCreateModule(s);
                    if (m == this)
                        connectedModules.Add(item);
                }
            }
        }

        // Used to advance through the queue
        public void Step(bool writeResult)
        {
            var inst = receiveQueue.Dequeue();
            if (writeResult)
            {
                string pulse = inst.pulse == Pulse.High ? "-high" : "-low";
                Console.WriteLine($"{inst.sender} {pulse} -> {name}");
            }
            ReceivePulse(inst.pulse, inst.sender);
        }

        public override bool Equals(object? obj)
        {
            return obj is Module module &&
                   name == module.name;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(name);
        }
        public override string? ToString()
        {
            return name;
        }
        public enum Pulse
        {
            High,
            Low
        }
        public enum Instruction
        {
            ReceivePulse,
            BroadcastPulse
        }
    }

    class FlipFlopModule : Module
    {
        bool flipFlopState;

        public FlipFlopModule(string name, List<string> destinationModules) : base(name, destinationModules)
        {
            this.flipFlopState = false;
        }

        // Override receive logic
        protected override void ReceivePulse(Pulse p, Module sender)
        {
            //base.ReceivePulse(p);
            if (p == Pulse.Low)
            {
                flipFlopState = !flipFlopState;

                if (flipFlopState == true)
                    BroadcastPulse(Pulse.High);
                else if (flipFlopState == false)
                    BroadcastPulse(Pulse.Low);
            }
        }
    }

    class ConjunctionModule : Module
    {
        public Dictionary<Module, Pulse> rememberedPulses;

        public ConjunctionModule(string name, List<string> destinationModules) : base(name, destinationModules)
        {
            rememberedPulses = new Dictionary<Module, Pulse>();
        }

        // Override initialization such that all connections to this has a remembered pulse associated with it
        public override void InitializeConnections()
        {
            rememberedPulses.Clear();
            connectedModules.Clear();

            // Find all connections to This and add a reference to it
            var allModules = moduleDict.Values;
            foreach (var item in allModules.ToArray())
            {
                foreach (var s in item.destinationModules)
                {
                    Module m = GetOrCreateModule(s);
                    if (m == this)
                    {
                        rememberedPulses[item] = Pulse.Low;
                        connectedModules.Add(item);
                    }
                }
            }
        }


        // Override receive logic
        protected override void ReceivePulse(Pulse p, Module sender)
        {
            rememberedPulses[sender] = p;

            bool allIsHigh = true;
            foreach (var item in rememberedPulses)
            {
                if (item.Value == Pulse.Low) allIsHigh &= false;
            }

            if (allIsHigh) BroadcastPulse(Pulse.Low);
            else BroadcastPulse(Pulse.High);
        }
    }

    class BroadcastModule : Module
    {
        public BroadcastModule(string name, List<string> destinationModules) : base(name, destinationModules)
        {

        }

        // Override receive logic
        protected override void ReceivePulse(Pulse p, Module sender)
        {
            BroadcastPulse(p);
        }
    }

    class ButtonModule : Module
    {
        Module? broadcaster;

        public ButtonModule(string name, List<string> destinationModules) : base(name, destinationModules)
        {
            foreach (var item in destinationModules)
            {
                Module m = moduleDict[item];
                if (m is BroadcastModule)
                {
                    broadcaster = m;
                    break;
                }
            }
        }

        public void PushButton()
        {
            BroadcastPulse(Pulse.Low);
        }

        // Override receive logic
        protected override void ReceivePulse(Pulse p, Module sender)
        {
            Console.WriteLine("Button received a pulse. Should this happen?");
        }

        // Override broadcast logic
        protected override void BroadcastPulse(Pulse p)
        {
            if (broadcaster != null)
            {
                switch (p)
                {
                    case Pulse.High:
                        SentHighCount++;
                        break;
                    case Pulse.Low:
                        SentLowCount++;
                        break;
                    default:
                        break;
                }
                broadcaster.QueueReceive(p, this);
            }
            else Console.WriteLine("Tried to press button but failed.");
        }
    }

    class OutputModule : Module
    {
        public OutputModule(string name, List<string> destinationModules) : base(name, destinationModules)
        {
        }

        protected override void ReceivePulse(Pulse p, Module sender)
        {

        }

        protected override void BroadcastPulse(Pulse p)
        {

        }
    }
}
