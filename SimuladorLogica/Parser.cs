using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SimuladorLogica
{
    /// <summary>
    /// Classe para analisar o código assembly MIPS
    /// </summary>
    public class Parser
    {
        // Dicionário para mapear rótulos para endereços
        private Dictionary<string, uint> _labels;

        // Construtor
        public Parser()
        {
            _labels = new Dictionary<string, uint>();
        }

        // Analisa um arquivo de código assembly MIPS
        public List<uint> ParseFile(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            return ParseAssembly(lines);
        }

        // Analisa uma string de código assembly MIPS
        public List<uint> ParseString(string assemblyCode)
        {
            string[] lines = assemblyCode.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return ParseAssembly(lines);
        }

        // Analisa um array de linhas de código assembly MIPS
        private List<uint> ParseAssembly(string[] lines)
        {
            // Primeira passagem: coleta rótulos
            CollectLabels(lines);

            // Segunda passagem: converte instruções para código de máquina
            List<uint> machineCode = new List<uint>();

            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();

                // Ignora linhas vazias e comentários
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("#"))
                    continue;

                // Remove comentários no final da linha
                int commentIndex = trimmedLine.IndexOf('#');
                if (commentIndex >= 0)
                    trimmedLine = trimmedLine.Substring(0, commentIndex).Trim();

                // Ignora linhas que são apenas rótulos
                if (trimmedLine.EndsWith(":"))
                    continue;

                // Remove rótulos no início da linha
                int labelEnd = trimmedLine.IndexOf(':');
                if (labelEnd >= 0)
                    trimmedLine = trimmedLine.Substring(labelEnd + 1).Trim();

                // Converte a instrução para código de máquina
                uint instruction = ParseInstruction(trimmedLine);
                machineCode.Add(instruction);
            }

            return machineCode;
        }

        // Coleta rótulos e seus endereços
        private void CollectLabels(string[] lines)
        {
            _labels.Clear();
            uint address = 0;

            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();

                // Ignora linhas vazias e comentários
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("#"))
                    continue;

                // Remove comentários no final da linha
                int commentIndex = trimmedLine.IndexOf('#');
                if (commentIndex >= 0)
                    trimmedLine = trimmedLine.Substring(0, commentIndex).Trim();

                // Processa rótulos
                int labelEnd = trimmedLine.IndexOf(':');
                if (labelEnd >= 0)
                {
                    string label = trimmedLine.Substring(0, labelEnd).Trim();
                    _labels[label] = address;

                    // Se a linha contém apenas o rótulo, não incrementa o endereço
                    if (trimmedLine.Length <= labelEnd + 1)
                        continue;
                }

                // Incrementa o endereço (cada instrução ocupa 4 bytes)
                address += 4;
            }
        }

        // Analisa uma instrução assembly e converte para código de máquina
        private uint ParseInstruction(string instruction)
        {
            // Divide a instrução em tokens
            string[] tokens = instruction.Split(new[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length == 0)
                throw new ArgumentException("Instrução vazia");

            string mnemonic = tokens[0].ToLower();

            // Implementação simplificada para algumas instruções comuns
            switch (mnemonic)
            {
                case "add":
                    return ParseRTypeInstruction(0, GetRegisterNumber(tokens[2]), GetRegisterNumber(tokens[3]), GetRegisterNumber(tokens[1]), 0, 0x20);

                case "sub":
                    return ParseRTypeInstruction(0, GetRegisterNumber(tokens[2]), GetRegisterNumber(tokens[3]), GetRegisterNumber(tokens[1]), 0, 0x22);

                case "and":
                    return ParseRTypeInstruction(0, GetRegisterNumber(tokens[2]), GetRegisterNumber(tokens[3]), GetRegisterNumber(tokens[1]), 0, 0x24);

                case "or":
                    return ParseRTypeInstruction(0, GetRegisterNumber(tokens[2]), GetRegisterNumber(tokens[3]), GetRegisterNumber(tokens[1]), 0, 0x25);

                case "nor":
                    return ParseRTypeInstruction(0, GetRegisterNumber(tokens[2]), GetRegisterNumber(tokens[3]), GetRegisterNumber(tokens[1]), 0, 0x27);

                case "slt":
                    return ParseRTypeInstruction(0, GetRegisterNumber(tokens[2]), GetRegisterNumber(tokens[3]), GetRegisterNumber(tokens[1]), 0, 0x2A);

                case "sltu":
                    return ParseRTypeInstruction(0, GetRegisterNumber(tokens[2]), GetRegisterNumber(tokens[3]), GetRegisterNumber(tokens[1]), 0, 0x2B);

                case "sll":
                    return ParseRTypeInstruction(0, 0, GetRegisterNumber(tokens[2]), GetRegisterNumber(tokens[1]), GetShiftAmount(tokens[3]), 0x00);

                case "srl":
                    return ParseRTypeInstruction(0, 0, GetRegisterNumber(tokens[2]), GetRegisterNumber(tokens[1]), GetShiftAmount(tokens[3]), 0x02);

                case "jr":
                    return ParseRTypeInstruction(0, GetRegisterNumber(tokens[1]), 0, 0, 0, 0x08);

                case "addi":
                    return ParseITypeInstruction(0x08, GetRegisterNumber(tokens[2]), GetRegisterNumber(tokens[1]), GetImmediate(tokens[3]));

                case "andi":
                    return ParseITypeInstruction(0x0C, GetRegisterNumber(tokens[2]), GetRegisterNumber(tokens[1]), GetImmediate(tokens[3]));

                case "ori":
                    return ParseITypeInstruction(0x0D, GetRegisterNumber(tokens[2]), GetRegisterNumber(tokens[1]), GetImmediate(tokens[3]));

                case "slti":
                    return ParseITypeInstruction(0x0A, GetRegisterNumber(tokens[2]), GetRegisterNumber(tokens[1]), GetImmediate(tokens[3]));

                case "sltiu":
                    return ParseITypeInstruction(0x0B, GetRegisterNumber(tokens[2]), GetRegisterNumber(tokens[1]), GetImmediate(tokens[3]));

                case "lw":
                    return ParseLoadStoreInstruction(0x23, tokens);

                case "lh":
                    return ParseLoadStoreInstruction(0x21, tokens);

                case "lb":
                    return ParseLoadStoreInstruction(0x20, tokens);

                case "sw":
                    return ParseLoadStoreInstruction(0x2B, tokens);

                case "sh":
                    return ParseLoadStoreInstruction(0x29, tokens);

                case "sb":
                    return ParseLoadStoreInstruction(0x28, tokens);

                case "beq":
                    return ParseBranchInstruction(0x04, tokens);

                case "bne":
                    return ParseBranchInstruction(0x05, tokens);

                case "j":
                    return ParseJTypeInstruction(0x02, tokens[1]);

                case "jal":
                    return ParseJTypeInstruction(0x03, tokens[1]);

                default:
                    throw new NotImplementedException($"Instrução não implementada: {mnemonic}");
            }
        }

        // Analisa uma instrução do tipo R
        private uint ParseRTypeInstruction(int opcode, int rs, int rt, int rd, int shamt, int funct)
        {
            return (uint)((opcode << 26) | (rs << 21) | (rt << 16) | (rd << 11) | (shamt << 6) | funct);
        }

        // Analisa uma instrução do tipo I
        private uint ParseITypeInstruction(int opcode, int rs, int rt, int immediate)
        {
            // Garante que o imediato tenha apenas 16 bits
            immediate &= 0xFFFF;

            return (uint)((opcode << 26) | (rs << 21) | (rt << 16) | immediate);
        }

        // Analisa uma instrução do tipo J
        private uint ParseJTypeInstruction(int opcode, string target)
        {
            uint address;

            // Verifica se o alvo é um rótulo
            if (_labels.TryGetValue(target, out address))
            {
                // Converte o endereço para o formato de instrução J (26 bits, sem os 2 bits menos significativos)
                address = (address >> 2) & 0x03FFFFFF;
            }
            else
            {
                // Tenta converter o alvo para um número
                if (target.StartsWith("0x"))
                {
                    address = Convert.ToUInt32(target.Substring(2), 16);
                }
                else
                {
                    address = Convert.ToUInt32(target);
                }

                // Converte o endereço para o formato de instrução J
                address &= 0x03FFFFFF;
            }

            return (uint)((opcode << 26) | address);
        }

        // Analisa uma instrução de carga/armazenamento (lw, sw, etc.)
        private uint ParseLoadStoreInstruction(int opcode, string[] tokens)
        {
            int rt = GetRegisterNumber(tokens[1]);

            // Extrai o offset e o registrador base
            string offsetBase = tokens[2];
            int openParen = offsetBase.IndexOf('(');
            int closeParen = offsetBase.IndexOf(')');

            if (openParen < 0 || closeParen < 0 || closeParen <= openParen)
                throw new ArgumentException($"Formato inválido para instrução de carga/armazenamento: {string.Join(" ", tokens)}");

            string offsetStr = offsetBase.Substring(0, openParen);
            string baseStr = offsetBase.Substring(openParen + 1, closeParen - openParen - 1);

            int offset = GetImmediate(offsetStr);
            int rs = GetRegisterNumber(baseStr);

            return ParseITypeInstruction(opcode, rs, rt, offset);
        }

        // Analisa uma instrução de desvio condicional (beq, bne)
        private uint ParseBranchInstruction(int opcode, string[] tokens)
        {
            int rs = GetRegisterNumber(tokens[1]);
            int rt = GetRegisterNumber(tokens[2]);

            int offset;
            string target = tokens[3];

            // Verifica se o alvo é um rótulo
            if (_labels.TryGetValue(target, out uint address))
            {
                // Calcula o offset relativo ao PC+4
                offset = (int)((address - (address + 4)) / 4);
            }
            else
            {
                // Tenta converter o alvo para um número
                offset = GetImmediate(target);
            }

            return ParseITypeInstruction(opcode, rs, rt, offset);
        }

        // Obtém o número do registrador a partir de sua representação textual
        private int GetRegisterNumber(string register)
        {
            return Register.GetRegisterIndex(register);
        }

        // Obtém o valor imediato a partir de sua representação textual
        private int GetImmediate(string immediate)
        {
            if (immediate.StartsWith("0x"))
            {
                return Convert.ToInt32(immediate.Substring(2), 16);
            }
            else
            {
                return Convert.ToInt32(immediate);
            }
        }

        // Obtém o valor de deslocamento a partir de sua representação textual
        private int GetShiftAmount(string shamt)
        {
            return GetImmediate(shamt) & 0x1F; // Apenas 5 bits
        }
    }
}