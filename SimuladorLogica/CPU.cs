using System;
using SimuladorComum;

namespace SimuladorLogica
{
    /// <summary>
    /// Classe que representa a CPU MIPS e coordena a execução
    /// </summary>
    public class CPU
    {
        // Componentes da CPU
        private Memory _memory;
        private Register _registers;
        private PCRegister _pc;
        private ALU _alu;
        private InstructionDecoder _decoder;
        private InstructionSet _instructionSet;

        // Configuração de clock
        private ClockConfiguration _clockConfig;

        // Estado atual
        private bool _isRunning;

        // Construtor
        public CPU()
        {
            _memory = new Memory();
            _registers = new Register();
            _pc = new PCRegister();
            _alu = new ALU();
            _decoder = new InstructionDecoder();
            _instructionSet = new InstructionSet();
            _clockConfig = new ClockConfiguration();
            _isRunning = false;
        }

        // Inicializa a CPU
        public void Initialize()
        {
            _pc.Value = 0;
            _registers.Reset();
            _isRunning = false;
        }

        // Configura o clock
        public void SetClockConfiguration(ClockConfiguration config)
        {
            _clockConfig = config;
        }

        // Carrega programa na memória
        public void LoadProgram(byte[] program, uint startAddress = 0)
        {
            _memory.LoadProgram(program, startAddress);
            _pc.Value = startAddress;
        }

        // Executa um ciclo de clock
        public bool ExecuteCycle()
        {
            if (!_isRunning)
                return false;

            // 1. Busca da instrução (Instruction Fetch)
            uint instructionAddress = _pc.Value;
            uint instruction = _memory.ReadWord(instructionAddress);

            // 2. Decodificação da instrução (Instruction Decode)
            var decodedInstruction = _decoder.Decode(instruction);

            // 3. Execução da instrução (Execute)
            bool programComplete = _instructionSet.Execute(decodedInstruction, _registers, _memory, _pc, _alu);

            // Incrementa PC se a instrução não for de salto
            if (!decodedInstruction.IsJumpInstruction && !decodedInstruction.IsBranchInstruction)
            {
                _pc.Value += 4; // Cada instrução tem 4 bytes (32 bits)
            }

            return !programComplete;
        }

        // Inicia a execução
        public void Start()
        {
            _isRunning = true;
        }

        // Pausa a execução
        public void Pause()
        {
            _isRunning = false;
        }

        // Reinicia a CPU
        public void Reset()
        {
            Initialize();
        }

        // Obtém o estado atual da CPU para a interface
        public SimulationState GetState()
        {
            SimulationState state = new SimulationState();

            // Copia PC
            state.PC = _pc.Value;

            // Copia instrução atual
            uint instructionAddress = _pc.Value;
            uint instruction = _memory.ReadWord(instructionAddress);
            state.CurrentInstruction = instruction;
            state.CurrentInstructionHex = $"0x{instruction:X8}";

            // Tenta decodificar a instrução para representação assembly
            try
            {
                var decodedInstruction = _decoder.Decode(instruction);
                state.CurrentInstructionAssembly = decodedInstruction.ToString();
            }
            catch
            {
                state.CurrentInstructionAssembly = "Instrução inválida";
            }

            // Copia registradores
            for (int i = 0; i < 32; i++)
            {
                state.Registers[i] = _registers.GetRegister(i);
            }

            // Copia memória (apenas parte relevante)
            // Aqui estamos copiando apenas os primeiros 1024 bytes como exemplo
            for (uint i = 0; i < 1024; i++)
            {
                state.Memory[i] = _memory.ReadByte(i);
            }

            return state;
        }

        // Verifica se a CPU está em execução
        public bool IsRunning()
        {
            return _isRunning;
        }
    }
}