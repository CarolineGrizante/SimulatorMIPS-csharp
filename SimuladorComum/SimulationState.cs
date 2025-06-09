using System;

namespace SimuladorComum
{
    // Representa o estado atual da simulação MIPS
    public class SimulationState
    {
        // Estado da CPU
        public uint PC { get; set; }
        public uint CurrentInstruction { get; set; }
        public string CurrentInstructionHex { get; set; }
        public string CurrentInstructionAssembly { get; set; }

        // Registradores
        public uint[] Registers { get; private set; }

        // Memória
        public byte[] Memory { get; private set; }

        // Estatísticas
        public long ElapsedClockCycles { get; set; }
        public double TotalExecutionTime { get; set; }
        public int TypeRInstructionsExecuted { get; set; }
        public int TypeIInstructionsExecuted { get; set; }
        public int TypeJInstructionsExecuted { get; set; }

        // Construtor
        public SimulationState(int memorySize = 4096)
        {
            Registers = new uint[32]; // 32 registradores MIPS
            Memory = new byte[memorySize]; // Tamanho padrão da memória
            PC = 0;
            CurrentInstruction = 0;
            CurrentInstructionHex = "0x00000000";
            CurrentInstructionAssembly = "";
            ElapsedClockCycles = 0;
            TotalExecutionTime = 0;
            TypeRInstructionsExecuted = 0;
            TypeIInstructionsExecuted = 0;
            TypeJInstructionsExecuted = 0;
        }

        // Clone para criar cópias do estado
        public SimulationState Clone()
        {
            SimulationState clone = new SimulationState(Memory.Length);
            clone.PC = this.PC;
            clone.CurrentInstruction = this.CurrentInstruction;
            clone.CurrentInstructionHex = this.CurrentInstructionHex;
            clone.CurrentInstructionAssembly = this.CurrentInstructionAssembly;

            // Clonar registradores
            Array.Copy(this.Registers, clone.Registers, this.Registers.Length);

            // Clonar memória
            Array.Copy(this.Memory, clone.Memory, this.Memory.Length);

            // Clonar estatísticas
            clone.ElapsedClockCycles = this.ElapsedClockCycles;
            clone.TotalExecutionTime = this.TotalExecutionTime;
            clone.TypeRInstructionsExecuted = this.TypeRInstructionsExecuted;
            clone.TypeIInstructionsExecuted = this.TypeIInstructionsExecuted;
            clone.TypeJInstructionsExecuted = this.TypeJInstructionsExecuted;

            return clone;
        }

        // Métodos auxiliares para acessar a memória
        public uint GetWord(uint address)
        {
            if (address + 3 >= Memory.Length)
                throw new IndexOutOfRangeException("Endereço de memória inválido");

            return (uint)(Memory[address] |
                         (Memory[address + 1] << 8) |
                         (Memory[address + 2] << 16) |
                         (Memory[address + 3] << 24));
        }

        public ushort GetHalfWord(uint address)
        {
            if (address + 1 >= Memory.Length)
                throw new IndexOutOfRangeException("Endereço de memória inválido");

            return (ushort)(Memory[address] | (Memory[address + 1] << 8));
        }

        public byte GetByte(uint address)
        {
            if (address >= Memory.Length)
                throw new IndexOutOfRangeException("Endereço de memória inválido");

            return Memory[address];
        }

        public void SetWord(uint address, uint value)
        {
            if (address + 3 >= Memory.Length)
                throw new IndexOutOfRangeException("Endereço de memória inválido");

            Memory[address] = (byte)(value & 0xFF);
            Memory[address + 1] = (byte)((value >> 8) & 0xFF);
            Memory[address + 2] = (byte)((value >> 16) & 0xFF);
            Memory[address + 3] = (byte)((value >> 24) & 0xFF);
        }

        public void SetHalfWord(uint address, ushort value)
        {
            if (address + 1 >= Memory.Length)
                throw new IndexOutOfRangeException("Endereço de memória inválido");

            Memory[address] = (byte)(value & 0xFF);
            Memory[address + 1] = (byte)((value >> 8) & 0xFF);
        }

        public void SetByte(uint address, byte value)
        {
            if (address >= Memory.Length)
                throw new IndexOutOfRangeException("Endereço de memória inválido");

            Memory[address] = value;
        }
    }
}
