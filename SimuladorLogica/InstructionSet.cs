using System;
using System.Collections.Generic;

namespace SimuladorLogica
{
    /// <summary>
    /// Classe que contém o conjunto de instruções suportadas
    /// </summary>
    public class InstructionSet
    {
        // Dicionário de instruções suportadas
        private Dictionary<int, string> _rTypeInstructions;
        private Dictionary<int, string> _iTypeInstructions;
        private Dictionary<int, string> _jTypeInstructions;

        // Construtor
        public InstructionSet()
        {
            InitializeInstructions();
        }

        // Inicializa as tabelas de instruções
        private void InitializeInstructions()
        {
            // Instruções do tipo R (baseadas no campo funct)
            _rTypeInstructions = new Dictionary<int, string>
            {
                { 0x20, "add" },
                { 0x22, "sub" },
                { 0x24, "and" },
                { 0x25, "or" },
                { 0x27, "nor" },
                { 0x00, "sll" },
                { 0x02, "srl" },
                { 0x2A, "slt" },
                { 0x2B, "sltu" },
                { 0x08, "jr" }
            };

            // Instruções do tipo I (baseadas no opcode)
            _iTypeInstructions = new Dictionary<int, string>
            {
                { 0x08, "addi" },
                { 0x0C, "andi" },
                { 0x0D, "ori" },
                { 0x0A, "slti" },
                { 0x0B, "sltiu" },
                { 0x23, "lw" },
                { 0x21, "lh" },
                { 0x20, "lb" },
                { 0x2B, "sw" },
                { 0x29, "sh" },
                { 0x28, "sb" },
                { 0x04, "beq" },
                { 0x05, "bne" }
            };

            // Instruções do tipo J (baseadas no opcode)
            _jTypeInstructions = new Dictionary<int, string>
            {
                { 0x02, "j" },
                { 0x03, "jal" }
            };
        }

        // Verifica se uma instrução do tipo R é suportada
        public bool IsRTypeSupported(int funct)
        {
            return _rTypeInstructions.ContainsKey(funct);
        }

        // Verifica se uma instrução do tipo I é suportada
        public bool IsITypeSupported(int opcode)
        {
            return _iTypeInstructions.ContainsKey(opcode);
        }

        // Verifica se uma instrução do tipo J é suportada
        public bool IsJTypeSupported(int opcode)
        {
            return _jTypeInstructions.ContainsKey(opcode);
        }

        // Obtém o mnemônico de uma instrução do tipo R
        public string GetRTypeMnemonic(int funct)
        {
            return _rTypeInstructions.TryGetValue(funct, out string mnemonic) ? mnemonic : "unknown";
        }

        // Obtém o mnemônico de uma instrução do tipo I
        public string GetITypeMnemonic(int opcode)
        {
            return _iTypeInstructions.TryGetValue(opcode, out string mnemonic) ? mnemonic : "unknown";
        }

        // Obtém o mnemônico de uma instrução do tipo J
        public string GetJTypeMnemonic(int opcode)
        {
            return _jTypeInstructions.TryGetValue(opcode, out string mnemonic) ? mnemonic : "unknown";
        }

        // Executa uma instrução
        public bool Execute(Instruction instruction, Register registers, Memory memory, PCRegister pc, ALU alu)
        {
            return instruction.Execute(registers, memory, pc, alu);
        }
    }
}
