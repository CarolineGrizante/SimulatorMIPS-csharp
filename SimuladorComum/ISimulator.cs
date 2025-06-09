using System;
using System.Collections.Generic;

namespace SimuladorComum
{
    
    // Interface principal para comunicação entre a lógica de simulação e a interface gráfica
    public interface ISimulator
    {
        // Propriedades
        SimulationState CurrentState { get; }
        bool IsRunning { get; }
        int ClockSpeed { get; set; }
        int TypeRInstructionsCount { get; }
        int TypeIInstructionsCount { get; }
        int TypeJInstructionsCount { get; }

        // Métodos
        void LoadProgram(string assemblyCode);
        void SetClockConfiguration(int clockSpeed, int typeRInstructions, int typeIInstructions, int typeJInstructions);
        bool Step();
        void Run();
        void Pause();
        void Reset();
        string GetInstructionHexRepresentation(uint instruction);

        // Eventos
        event EventHandler<SimulationEventArgs> SimulationStateChanged;
        event EventHandler<SimulationEventArgs> InstructionExecuted;
        event EventHandler SimulationCompleted;
        event EventHandler<SimulationEventArgs> SimulationError;
    }
}
