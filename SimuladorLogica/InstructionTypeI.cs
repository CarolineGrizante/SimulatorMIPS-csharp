using System;

namespace SimuladorLogica
{
    /// <summary>
    /// Classe para instruções do tipo I (Immediate)
    /// </summary>
    public class InstructionTypeI : Instruction
    {
        // Campos específicos do formato I
        public int OpCode { get; private set; }
        public int Rs { get; private set; }
        public int Rt { get; private set; }
        public int Immediate { get; private set; }

        // Construtor
        public InstructionTypeI(uint rawInstruction) : base(rawInstruction)
        {
            Type = InstructionType.I;

            // Extrai os campos da instrução
            OpCode = (int)((rawInstruction >> 26) & 0x3F);
            Rs = (int)((rawInstruction >> 21) & 0x1F);
            Rt = (int)((rawInstruction >> 16) & 0x1F);
            Immediate = (int)(rawInstruction & 0xFFFF);

            // Define o mnemônico com base no opcode
            switch (OpCode)
            {
                case 0x08: Mnemonic = "addi"; break;
                case 0x0C: Mnemonic = "andi"; break;
                case 0x0D: Mnemonic = "ori"; break;
                case 0x0A: Mnemonic = "slti"; break;
                case 0x0B: Mnemonic = "sltiu"; break;
                case 0x23: Mnemonic = "lw"; break;
                case 0x21: Mnemonic = "lh"; break;
                case 0x20: Mnemonic = "lb"; break;
                case 0x2B: Mnemonic = "sw"; break;
                case 0x29: Mnemonic = "sh"; break;
                case 0x28: Mnemonic = "sb"; break;
                case 0x04: Mnemonic = "beq"; IsBranchInstruction = true; break;
                case 0x05: Mnemonic = "bne"; IsBranchInstruction = true; break;
                default: Mnemonic = "unknown"; break;
            }
        }

        // Executa a instrução
        public override bool Execute(Register registers, Memory memory, PCRegister pc, ALU alu)
        {
            uint result = 0;
            uint rsValue = registers.GetRegister(Rs);
            uint rtValue = registers.GetRegister(Rt);

            // Extensão de sinal para o imediato
            int signExtImm = (Immediate & 0x8000) != 0 ?
                 (Immediate | unchecked((int)0xFFFF0000)) :
                 Immediate;

            uint address;

            switch (OpCode)
            {
                case 0x08: // addi
                    result = alu.Add(rsValue, (uint)signExtImm);
                    registers.SetRegister(Rt, result);
                    break;

                case 0x0C: // andi
                    result = alu.And(rsValue, (uint)Immediate); // Zero-extended
                    registers.SetRegister(Rt, result);
                    break;

                case 0x0D: // ori
                    result = alu.Or(rsValue, (uint)Immediate); // Zero-extended
                    registers.SetRegister(Rt, result);
                    break;

                case 0x0A: // slti
                    result = alu.IsLessThan((int)rsValue, signExtImm) ? 1u : 0u;
                    registers.SetRegister(Rt, result);
                    break;

                case 0x0B: // sltiu
                    result = alu.IsLessThanUnsigned(rsValue, (uint)signExtImm) ? 1u : 0u;
                    registers.SetRegister(Rt, result);
                    break;

                case 0x23: // lw
                    address = alu.Add(rsValue, (uint)signExtImm);
                    result = memory.ReadWord(address);
                    registers.SetRegister(Rt, result);
                    break;

                case 0x21: // lh
                    address = alu.Add(rsValue, (uint)signExtImm);
                    ushort halfWord = memory.ReadHalfWord(address);
                    // Extensão de sinal para half word
                    result = (halfWord & 0x8000) != 0 ?
                             (uint)(halfWord | 0xFFFF0000) :
                             (uint)halfWord;
                    registers.SetRegister(Rt, result);
                    break;

                case 0x20: // lb
                    address = alu.Add(rsValue, (uint)signExtImm);
                    byte byteValue = memory.ReadByte(address);
                    // Extensão de sinal para byte
                    result = (byteValue & 0x80) != 0 ?
                             (uint)(byteValue | 0xFFFFFF00) :
                             (uint)byteValue;
                    registers.SetRegister(Rt, result);
                    break;

                case 0x2B: // sw
                    address = alu.Add(rsValue, (uint)signExtImm);
                    memory.WriteWord(address, rtValue);
                    break;

                case 0x29: // sh
                    address = alu.Add(rsValue, (uint)signExtImm);
                    memory.WriteHalfWord(address, (ushort)(rtValue & 0xFFFF));
                    break;

                case 0x28: // sb
                    address = alu.Add(rsValue, (uint)signExtImm);
                    memory.WriteByte(address, (byte)(rtValue & 0xFF));
                    break;

                case 0x04: // beq
                    if (alu.IsEqual(rsValue, rtValue))
                    {
                        pc.Value = (uint)(pc.Value + ((signExtImm) << 2));
                    }
                    break;

                case 0x05: // bne
                    if (alu.IsNotEqual(rsValue, rtValue))
                    {
                        pc.Value = (uint)(pc.Value + ((signExtImm) << 2));
                    }
                    break;

                default:
                    throw new NotImplementedException($"Instrução tipo I não implementada: opcode={OpCode:X2}");
            }

            return false; // Instrução não finaliza o programa
        }

        // Representação assembly da instrução
        public override string ToString()
        {
            switch (OpCode)
            {
                case 0x23: // lw
                case 0x21: // lh
                case 0x20: // lb
                case 0x2B: // sw
                case 0x29: // sh
                case 0x28: // sb
                    return $"{Mnemonic} {Register.GetRegisterName(Rt)}, {Immediate}({Register.GetRegisterName(Rs)})";

                case 0x04: // beq
                case 0x05: // bne
                    return $"{Mnemonic} {Register.GetRegisterName(Rs)}, {Register.GetRegisterName(Rt)}, {Immediate}";

                default:
                    return $"{Mnemonic} {Register.GetRegisterName(Rt)}, {Register.GetRegisterName(Rs)}, {Immediate}";
            }
        }
    }
}