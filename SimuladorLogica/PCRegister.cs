using System;

namespace SimuladorLogica
{
    /// <summary>
    /// Classe específica para o registrador PC (Program Counter)
    /// </summary>
    public class PCRegister
    {
        // Valor atual do PC
        private uint _value;

        // Construtor
        public PCRegister()
        {
            _value = 0;
        }

        // Propriedade para acessar o valor do PC
        public uint Value
        {
            get { return _value; }
            set { _value = value; }
        }

        // Incrementa o PC pelo valor especificado (padrão: 4 bytes, uma instrução)
        public void Increment(uint increment = 4)
        {
            _value += increment;
        }

        // Reseta o PC para zero
        public void Reset()
        {
            _value = 0;
        }

        // Representação hexadecimal do PC
        public string ToHexString()
        {
            return $"0x{_value:X8}";
        }
    }
}