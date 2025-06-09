using System;

namespace SimuladorComum
{
    // Argumentos para eventos de simulação
    public class SimulationEventArgs : EventArgs
    {
        public SimulationState State { get; private set; }
        public string Message { get; private set; }
        public SimulationEventType EventType { get; private set; }

        public SimulationEventArgs(SimulationState state, string message, SimulationEventType eventType)
        {
            State = state;
            Message = message;
            EventType = eventType;
        }
    }

    // Enum para tipos de eventos de simulação
    public enum SimulationEventType
    {
        ClockTick,
        InstructionFetch,
        InstructionDecode,
        InstructionExecute,
        MemoryAccess,
        RegisterWrite,
        ProgramComplete,
        Error
    }
}