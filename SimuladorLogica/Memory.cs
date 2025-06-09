using System;

namespace SimuladorLogica
{
    /// <summary>
    /// Classe que gerencia a memória do sistema
    /// </summary>
    public class Memory
    {
        // Tamanho padrão da memória em bytes
        private const int DEFAULT_MEMORY_SIZE = 4096;

        // Array de bytes que representa a memória
        private byte[] _memory;

        // Construtor
        public Memory(int size = DEFAULT_MEMORY_SIZE)
        {
            _memory = new byte[size];
        }

        // Carrega um programa na memória
        public void LoadProgram(byte[] program, uint startAddress = 0)
        {
            if (startAddress + program.Length > _memory.Length)
                throw new ArgumentException("Programa muito grande para a memória disponível");

            Array.Copy(program, 0, _memory, startAddress, program.Length);
        }

        // Lê um byte da memória
        public byte ReadByte(uint address)
        {
            ValidateAddress(address);
            return _memory[address];
        }

        // Lê uma half-word (2 bytes) da memória
        public ushort ReadHalfWord(uint address)
        {
            ValidateAddress(address, 2);
            return (ushort)(_memory[address] | (_memory[address + 1] << 8));
        }

        // Lê uma word (4 bytes) da memória
        public uint ReadWord(uint address)
        {
            ValidateAddress(address, 4);
            return (uint)(_memory[address] |
                         (_memory[address + 1] << 8) |
                         (_memory[address + 2] << 16) |
                         (_memory[address + 3] << 24));
        }

        // Escreve um byte na memória
        public void WriteByte(uint address, byte value)
        {
            ValidateAddress(address);
            _memory[address] = value;
        }

        // Escreve uma half-word (2 bytes) na memória
        public void WriteHalfWord(uint address, ushort value)
        {
            ValidateAddress(address, 2);
            _memory[address] = (byte)(value & 0xFF);
            _memory[address + 1] = (byte)((value >> 8) & 0xFF);
        }

        // Escreve uma word (4 bytes) na memória
        public void WriteWord(uint address, uint value)
        {
            ValidateAddress(address, 4);
            _memory[address] = (byte)(value & 0xFF);
            _memory[address + 1] = (byte)((value >> 8) & 0xFF);
            _memory[address + 2] = (byte)((value >> 16) & 0xFF);
            _memory[address + 3] = (byte)((value >> 24) & 0xFF);
        }

        // Limpa a memória (zera todos os bytes)
        public void Clear()
        {
            Array.Clear(_memory, 0, _memory.Length);
        }

        // Valida se o endereço é válido
        private void ValidateAddress(uint address, int size = 1)
        {
            if (address + size - 1 >= _memory.Length)
                throw new IndexOutOfRangeException($"Endereço de memória inválido: {address}");
        }

        // Retorna o tamanho da memória
        public int Size => _memory.Length;

        // Obtém uma cópia da memória
        public byte[] GetMemoryCopy()
        {
            byte[] copy = new byte[_memory.Length];
            Array.Copy(_memory, copy, _memory.Length);
            return copy;
        }
    }
}
