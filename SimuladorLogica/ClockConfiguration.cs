using System;

namespace SimuladorLogica
{
    /// <summary>
    /// Classe para configurar o clock da CPU e quantidade de instruções por tipo
    /// </summary>
    public class ClockConfiguration
    {
        // Frequência do clock em Hz
        public int ClockFrequency { get; set; }

        // Ciclos de clock por tipo de instrução
        public int TypeRInstructionCycles { get; set; }
        public int TypeIInstructionCycles { get; set; }
        public int TypeJInstructionCycles { get; set; }

        // Construtor com valores padrão
        public ClockConfiguration()
        {
            ClockFrequency = 1000000; // 1 MHz
            TypeRInstructionCycles = 1;
            TypeIInstructionCycles = 1;
            TypeJInstructionCycles = 1;
        }

        // Construtor com parâmetros
        public ClockConfiguration(int clockFrequency, int typeRCycles, int typeICycles, int typeJCycles)
        {
            ClockFrequency = clockFrequency;
            TypeRInstructionCycles = typeRCycles;
            TypeIInstructionCycles = typeICycles;
            TypeJInstructionCycles = typeJCycles;
        }

        // Calcula o tempo de execução de uma instrução em segundos
        public double CalculateInstructionTime(Instruction.InstructionType type)
        {
            int cycles;

            switch (type)
            {
                case Instruction.InstructionType.R:
                    cycles = TypeRInstructionCycles;
                    break;
                case Instruction.InstructionType.I:
                    cycles = TypeIInstructionCycles;
                    break;
                case Instruction.InstructionType.J:
                    cycles = TypeJInstructionCycles;
                    break;
                default:
                    cycles = 1;
                    break;
            }

            // Tempo = ciclos / frequência
            return (double)cycles / ClockFrequency;
        }

        // Calcula o tempo total de execução em segundos
        public double CalculateTotalTime(int typeRCount, int typeICount, int typeJCount)
        {
            double totalTime = 0;

            totalTime += (double)(typeRCount * TypeRInstructionCycles) / ClockFrequency;
            totalTime += (double)(typeICount * TypeIInstructionCycles) / ClockFrequency;
            totalTime += (double)(typeJCount * TypeJInstructionCycles) / ClockFrequency;

            return totalTime;
        }
    }
}
