using System;

namespace SimuladorLogica
{
    /// <summary>
    /// Classe que representa os registradores do MIPS
    /// </summary>
    public class Register
    {
        // MIPS possui 32 registradores de 32 bits cada
        private uint[] _registers;

        // Nomes dos registradores MIPS
        private static readonly string[] RegisterNames = new string[]
        {
            "$zero", "$at", "$v0", "$v1", "$a0", "$a1", "$a2", "$a3",
            "$t0", "$t1", "$t2", "$t3", "$t4", "$t5", "$t6", "$t7",
            "$s0", "$s1", "$s2", "$s3", "$s4", "$s5", "$s6", "$s7",
            "$t8", "$t9", "$k0", "$k1", "$gp", "$sp", "$fp", "$ra"
        };

        // Construtor
        public Register()
        {
            _registers = new uint[32];
            Reset();
        }

        // Reseta todos os registradores para zero
        public void Reset()
        {
            Array.Clear(_registers, 0, _registers.Length);
        }

        // Obtém o valor de um registrador pelo índice
        public uint GetRegister(int index)
        {
            ValidateRegisterIndex(index);
            return _registers[index];
        }

        // Define o valor de um registrador pelo índice
        public void SetRegister(int index, uint value)
        {
            ValidateRegisterIndex(index);

            // O registrador $zero (índice 0) sempre deve ser zero
            if (index == 0)
                return;

            _registers[index] = value;
        }

        // Obtém o valor de um registrador pelo nome
        public uint GetRegister(string name)
        {
            int index = GetRegisterIndex(name);
            return GetRegister(index);
        }

        // Define o valor de um registrador pelo nome
        public void SetRegister(string name, uint value)
        {
            int index = GetRegisterIndex(name);
            SetRegister(index, value);
        }

        // Obtém o índice de um registrador pelo nome
        public static int GetRegisterIndex(string name)
        {
            // Remove o $ se presente
            if (name.StartsWith("$"))
                name = name.Substring(1);

            // Verifica se é um número direto
            if (int.TryParse(name, out int index))
            {
                if (index >= 0 && index < 32)
                    return index;
            }

            // Procura pelo nome do registrador
            for (int i = 0; i < RegisterNames.Length; i++)
            {
                if (RegisterNames[i].Substring(1) == name) // Remove o $ para comparação
                    return i;
            }

            throw new ArgumentException($"Registrador inválido: {name}");
        }

        // Obtém o nome de um registrador pelo índice
        public static string GetRegisterName(int index)
        {
            ValidateRegisterIndex(index);
            return RegisterNames[index];
        }

        // Valida se o índice do registrador é válido
        private static void ValidateRegisterIndex(int index)
        {
            if (index < 0 || index >= 32)
                throw new IndexOutOfRangeException($"Índice de registrador inválido: {index}");
        }

        // Obtém uma cópia dos registradores
        public uint[] GetRegistersCopy()
        {
            uint[] copy = new uint[32];
            Array.Copy(_registers, copy, 32);
            return copy;
        }
    }
}