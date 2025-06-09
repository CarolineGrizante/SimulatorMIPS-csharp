using System;

namespace SimuladorLogica
{
    /// <summary>
    /// Classe para instruções do tipo J (Jump)
    /// </summary>
    public class InstructionTypeJ : Instruction
    {
        // Campos específicos do formato J
        public int OpCode { get; private set; }
        public int Target { get; private set; }

        // Construtor
        public InstructionTypeJ(uint rawInstruction) : base(rawInstruction)
        {
            Type = InstructionType.J;

            // Extrai os campos da instrução
            OpCode = (int)((rawInstruction >> 26) & 0x3F);
            Target = (int)(rawInstruction & 0x03FFFFFF);

            // Define o mnemônico com base no opcode
            switch (OpCode)
            {
                case 0x02: Mnemonic = "j"; IsJumpInstruction = true; break;
                case 0x03: Mnemonic = "jal"; IsJumpInstruction = true; break;
                default: Mnemonic = "unknown"; break;
            }
        }

        // Executa a instrução
        public override bool Execute(Register registers, Memory memory, PCRegister pc, ALU alu)
        {
            // Calcula o endereço de destino
            // Os 4 bits mais significativos são os mesmos do PC atual
            uint jumpAddress = (pc.Value & 0xF0000000) | ((uint)Target << 2);

            switch (OpCode)
            {
                case 0x02: // j
                    pc.Value = jumpAddress;
                    break;

                case 0x03: // jal
                    // Salva o endereço de retorno em $ra (registrador 31)
                    registers.SetRegister(31, pc.Value + 4);
                    pc.Value = jumpAddress;
                    break;

                default:
                    throw new NotImplementedException($"Instrução tipo J não implementada: opcode={OpCode:X2}");
            }

            return false; // Instrução não finaliza o programa
        }

        // Representação assembly da instrução
        public override string ToString()
        {
            return $"{Mnemonic} 0x{Target:X7}";
        }
    }
}