using System;

namespace SimuladorLogica
{
    /// <summary>
    /// Classe base para representar instruções MIPS
    /// </summary>
    public abstract class Instruction
    {
        // Tipo da instrução
        public enum InstructionType
        {
            R, // Instruções do tipo R (Register)
            I, // Instruções do tipo I (Immediate)
            J  // Instruções do tipo J (Jump)
        }

        // Propriedades comuns a todas as instruções
        public uint RawInstruction { get; protected set; }
        public InstructionType Type { get; protected set; }
        public string Mnemonic { get; protected set; }
        public bool IsJumpInstruction { get; protected set; }
        public bool IsBranchInstruction { get; protected set; }

        // Construtor
        protected Instruction(uint rawInstruction)
        {
            RawInstruction = rawInstruction;
            IsJumpInstruction = false;
            IsBranchInstruction = false;
        }

        // Método abstrato para executar a instrução
        public abstract bool Execute(Register registers, Memory memory, PCRegister pc, ALU alu);

        // Converte a instrução para representação hexadecimal
        public string ToHexString()
        {
            return $"0x{RawInstruction:X8}";
        }

        // Método abstrato para obter a representação assembly da instrução
        public abstract override string ToString();
    }
}
