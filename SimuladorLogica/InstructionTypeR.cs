using System;

namespace SimuladorLogica
{
    /// <summary>
    /// Classe para instruções do tipo R (Register)
    /// </summary>
    public class InstructionTypeR : Instruction
    {
        // Campos específicos do formato R
        public int OpCode { get; private set; }
        public int Rs { get; private set; }
        public int Rt { get; private set; }
        public int Rd { get; private set; }
        public int Shamt { get; private set; }
        public int Funct { get; private set; }

        // Construtor
        public InstructionTypeR(uint rawInstruction) : base(rawInstruction)
        {
            Type = InstructionType.R;

            // Extrai os campos da instrução
            OpCode = (int)((rawInstruction >> 26) & 0x3F);
            Rs = (int)((rawInstruction >> 21) & 0x1F);
            Rt = (int)((rawInstruction >> 16) & 0x1F);
            Rd = (int)((rawInstruction >> 11) & 0x1F);
            Shamt = (int)((rawInstruction >> 6) & 0x1F);
            Funct = (int)(rawInstruction & 0x3F);

            // Define o mnemônico com base no campo funct
            switch (Funct)
            {
                case 0x20: Mnemonic = "add"; break;
                case 0x22: Mnemonic = "sub"; break;
                case 0x24: Mnemonic = "and"; break;
                case 0x25: Mnemonic = "or"; break;
                case 0x27: Mnemonic = "nor"; break;
                case 0x00: Mnemonic = "sll"; break;
                case 0x02: Mnemonic = "srl"; break;
                case 0x2A: Mnemonic = "slt"; break;
                case 0x2B: Mnemonic = "sltu"; break;
                case 0x08: Mnemonic = "jr"; IsJumpInstruction = true; break;
                default: Mnemonic = "unknown"; break;
            }
        }

        // Executa a instrução
        public override bool Execute(Register registers, Memory memory, PCRegister pc, ALU alu)
        {
            uint result = 0;
            uint rsValue = registers.GetRegister(Rs);
            uint rtValue = registers.GetRegister(Rt);

            switch (Funct)
            {
                case 0x20: // add
                    result = alu.Add(rsValue, rtValue);
                    registers.SetRegister(Rd, result);
                    break;

                case 0x22: // sub
                    result = alu.Subtract(rsValue, rtValue);
                    registers.SetRegister(Rd, result);
                    break;

                case 0x24: // and
                    result = alu.And(rsValue, rtValue);
                    registers.SetRegister(Rd, result);
                    break;

                case 0x25: // or
                    result = alu.Or(rsValue, rtValue);
                    registers.SetRegister(Rd, result);
                    break;

                case 0x27: // nor
                    result = alu.Nor(rsValue, rtValue);
                    registers.SetRegister(Rd, result);
                    break;

                case 0x00: // sll
                    result = alu.ShiftLeft(rtValue, Shamt);
                    registers.SetRegister(Rd, result);
                    break;

                case 0x02: // srl
                    result = alu.ShiftRight(rtValue, Shamt);
                    registers.SetRegister(Rd, result);
                    break;

                case 0x2A: // slt
                    result = alu.IsLessThan((int)rsValue, (int)rtValue) ? 1u : 0u;
                    registers.SetRegister(Rd, result);
                    break;

                case 0x2B: // sltu
                    result = alu.IsLessThanUnsigned(rsValue, rtValue) ? 1u : 0u;
                    registers.SetRegister(Rd, result);
                    break;

                case 0x08: // jr
                    pc.Value = rsValue;
                    break;

                default:
                    throw new NotImplementedException($"Instrução tipo R não implementada: funct={Funct:X2}");
            }

            return false; // Instrução não finaliza o programa
        }

        // Representação assembly da instrução
        public override string ToString()
        {
            switch (Funct)
            {
                case 0x00: // sll
                case 0x02: // srl
                    return $"{Mnemonic} {Register.GetRegisterName(Rd)}, {Register.GetRegisterName(Rt)}, {Shamt}";

                case 0x08: // jr
                    return $"{Mnemonic} {Register.GetRegisterName(Rs)}";

                default:
                    return $"{Mnemonic} {Register.GetRegisterName(Rd)}, {Register.GetRegisterName(Rs)}, {Register.GetRegisterName(Rt)}";
            }
        }
    }
}