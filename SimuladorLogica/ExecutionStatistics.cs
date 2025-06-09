using System;

namespace SimuladorLogica
{
    /// <summary>
    /// Classe para calcular estatísticas de execução
    /// </summary>
    public class ExecutionStatistics
    {
        // Contadores de instruções executadas por tipo
        public int TypeRInstructionsExecuted { get; private set; }
        public int TypeIInstructionsExecuted { get; private set; }
        public int TypeJInstructionsExecuted { get; private set; }

        // Contador de ciclos de clock
        public long ClockCycles { get; private set; }

        // Tempo total de execução em segundos
        public double TotalExecutionTime { get; private set; }

        // Construtor
        public ExecutionStatistics()
        {
            Reset();
        }

        // Reinicia as estatísticas
        public void Reset()
        {
            TypeRInstructionsExecuted = 0;
            TypeIInstructionsExecuted = 0;
            TypeJInstructionsExecuted = 0;
            ClockCycles = 0;
            TotalExecutionTime = 0;
        }

        // Incrementa o contador de instruções do tipo R
        public void IncrementTypeRInstructions()
        {
            TypeRInstructionsExecuted++;
        }

        // Incrementa o contador de instruções do tipo I
        public void IncrementTypeIInstructions()
        {
            TypeIInstructionsExecuted++;
        }

        // Incrementa o contador de instruções do tipo J
        public void IncrementTypeJInstructions()
        {
            TypeJInstructionsExecuted++;
        }

        // Incrementa o contador de ciclos de clock
        public void IncrementClockCycles(long cycles)
        {
            ClockCycles += cycles;
        }

        // Atualiza o tempo total de execução
        public void UpdateExecutionTime(ClockConfiguration clockConfig)
        {
            TotalExecutionTime = clockConfig.CalculateTotalTime(
                TypeRInstructionsExecuted,
                TypeIInstructionsExecuted,
                TypeJInstructionsExecuted);
        }

        // Retorna o número total de instruções executadas
        public int TotalInstructionsExecuted()
        {
            return TypeRInstructionsExecuted + TypeIInstructionsExecuted + TypeJInstructionsExecuted;
        }

        // Formata o tempo de execução para exibição
        public string FormatExecutionTime()
        {
            if (TotalExecutionTime < 0.000001) // Menos de 1 nanossegundo
            {
                return $"{TotalExecutionTime * 1000000000:F2} ns";
            }
            else if (TotalExecutionTime < 0.001) // Menos de 1 milissegundo
            {
                return $"{TotalExecutionTime * 1000000:F2} µs";
            }
            else if (TotalExecutionTime < 1) // Menos de 1 segundo
            {
                return $"{TotalExecutionTime * 1000:F2} ms";
            }
            else // 1 segundo ou mais
            {
                return $"{TotalExecutionTime:F4} s";
            }
        }
    }
}