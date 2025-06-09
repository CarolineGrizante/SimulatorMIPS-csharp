using System;

namespace SimuladorLogica
{
    /// <summary>
    /// Classe para decodificar instruções de 32 bits
    /// </summary>
    public class InstructionDecoder
    {
        private InstructionSet _instructionSet;

        // Construtor
        public InstructionDecoder()
        {
            _instructionSet = new InstructionSet();
        }

        // Decodifica uma instrução de 32 bits
        public Instruction Decode(uint rawInstruction)
        {
            // Extrai o opcode (6 bits mais significativos)
            int opcode = (int)((rawInstruction >> 26) & 0x3F);

            // Determina o tipo de instrução com base no opcode
            if (opcode == 0)
            {
                // Tipo R - opcode 0, função determinada pelo campo funct
                return new InstructionTypeR(rawInstruction);
            }
            else if (opcode == 0x02 || opcode == 0x03)
            {
                // Tipo J - opcodes 0x02 (j) e 0x03 (jal)
                return new InstructionTypeJ(rawInstruction);
            }
            else
            {
                // Tipo I - todos os outros opcodes
                return new InstructionTypeI(rawInstruction);
            }
        }

        // Converte uma instrução para representação hexadecimal
        public string ToHexString(uint instruction)
        {
            return $"0x{instruction:X8}";
        }

        // Verifica se a instrução é válida
        public bool IsValidInstruction(uint rawInstruction)
        {
            try
            {
                int opcode = (int)((rawInstruction >> 26) & 0x3F);

                if (opcode == 0)
                {
                    // Tipo R
                    int funct = (int)(rawInstruction & 0x3F);
                    return _instructionSet.IsRTypeSupported(funct);
                }
                else if (opcode == 0x02 || opcode == 0x03)
                {
                    // Tipo J
                    return _instructionSet.IsJTypeSupported(opcode);
                }
                else
                {
                    // Tipo I
                    return _instructionSet.IsITypeSupported(opcode);
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
