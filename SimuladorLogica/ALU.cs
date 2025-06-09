using System;

namespace SimuladorLogica
{
    /// <summary>
    /// Unidade Lógica Aritmética para operações
    /// </summary>
    public class ALU
    {
        // Construtor
        public ALU()
        {
        }

        // Operações aritméticas
        public uint Add(uint a, uint b)
        {
            return a + b;
        }

        public uint Subtract(uint a, uint b)
        {
            return a - b;
        }

        // Operações lógicas
        public uint And(uint a, uint b)
        {
            return a & b;
        }

        public uint Or(uint a, uint b)
        {
            return a | b;
        }

        public uint Nor(uint a, uint b)
        {
            return ~(a | b);
        }

        // Operações de deslocamento
        public uint ShiftLeft(uint value, int shiftAmount)
        {
            return value << shiftAmount;
        }

        public uint ShiftRight(uint value, int shiftAmount)
        {
            return value >> shiftAmount;
        }

        // Operações de comparação
        public bool IsLessThan(int a, int b)
        {
            return a < b;
        }

        public bool IsLessThanUnsigned(uint a, uint b)
        {
            return a < b;
        }

        public bool IsEqual(uint a, uint b)
        {
            return a == b;
        }

        public bool IsNotEqual(uint a, uint b)
        {
            return a != b;
        }

        // Operação de extensão de sinal
        public uint SignExtend(ushort value)
        {
            // Verifica se o bit mais significativo é 1
            if ((value & 0x8000) != 0)
            {
                // Estende com 1s
                return (uint)(value | 0xFFFF0000);
            }
            else
            {
                // Estende com 0s
                return (uint)value;
            }
        }
    }
}
