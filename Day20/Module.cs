using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day20
{
    abstract class Module
    {
        public static int totalHighPulses = 0;
        public static int totalLowPulses = 0;

        public string name;
        private int queueWeight = 0;

        public static Dictionary<string, Module> moduleDict = new Dictionary<string, Module>();

        public PriorityQueue<(Pulse pulse, Module sender), int> receiveQueue;

        public List<string> destinationModules = new List<string>();

        protected Module(string name, List<string> destinationModules)
        {
            this.name = name;
            this.destinationModules = destinationModules;
            moduleDict.Add(name, this);

            receiveQueue = new PriorityQueue<(Pulse, Module), int>();
        }

        protected abstract void ReceivePulse(Pulse p, Module sender);


        protected virtual void BroadcastPulse(Pulse p)
        {
            foreach (var item in destinationModules)
            {
                Module m = GetOrCreateModule(item);
                m.QueueReceive(p, this);
                //item.ReceivePulse(p, this);
                /*if (p == Pulse.High)
                    ;
                else if (p == Pulse.Low)
                    ;*/
            }
        }

        protected Module GetOrCreateModule(string moduleName)
        {
            if (moduleDict.TryGetValue(moduleName, out Module m))
            {
                return m;
            }
            else
            {
                Module newm = new OutputModule(moduleName, []);
                return newm;
            }
        }

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
            /*foreach (var item in moduleDict.Values)
            {
                if (item.name != "broadcaster" && item != this)
                    rememberedPulses.Add(item, Pulse.Low);
            }*/
        }

        public void InitializeRememberedPulses()
        {
            // Find all connections to This and add a reference to it
            var allModules = moduleDict.Values;
            foreach (var item in allModules.ToArray())
            {
                foreach (var s in item.destinationModules)
                {
                    Module m = GetOrCreateModule(s);
                    if (m == this)
                        rememberedPulses[item] = Pulse.Low;
                }
            }
        }

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

        /*protected override void BroadcastPulse(Pulse p)
        {
            foreach (var item in destinationModules)
            {
                //Module m = moduleDict[item];
                //Pulse rememberedPulse = rememberedPulses[m];

                Module m = moduleDict[item];
                m.QueueReceive(p, this);
            }
        }*/
    }

    class BroadcastModule : Module
    {
        public BroadcastModule(string name, List<string> destinationModules) : base(name, destinationModules)
        {

        }

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

        protected override void ReceivePulse(Pulse p, Module sender)
        {
            Console.WriteLine("Button received a pulse.");
        }

        protected override void BroadcastPulse(Pulse p)
        {
            if (broadcaster != null) broadcaster.QueueReceive(p, this);
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
