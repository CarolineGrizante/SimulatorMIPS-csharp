using System;
using System.IO;
using System.Collections.Generic;

namespace SimuladorLogica
{
    class Program
    {
        // Array de registradores
        static uint[] registradores = new uint[32];

        // Memória
        static Dictionary<uint, byte> memoria = new Dictionary<uint, byte>();

        // PC - Program Counter
        static uint pc = 0;

        // Contadores de instruções
        static int instrucoesTipoR = 0;
        static int instrucoesTipoI = 0;
        static int instrucoesTipoJ = 0;

        // Nomes dos registradores
        static string[] nomeRegistradores = {
            "$zero", "$at", "$v0", "$v1", "$a0", "$a1", "$a2", "$a3",
            "$t0", "$t1", "$t2", "$t3", "$t4", "$t5", "$t6", "$t7",
            "$s0", "$s1", "$s2", "$s3", "$s4", "$s5", "$s6", "$s7",
            "$t8", "$t9", "$k0", "$k1", "$gp", "$sp", "$fp", "$ra"
        };

        static void Main(string[] args)
        {
            Console.WriteLine("           Simulador MIPS          ");
            Console.WriteLine("====================================");

            // Inicializa registradores
            for (int i = 0; i < 32; i++)
            {
                registradores[i] = 0;
            }

            // Inicializa memória com alguns valores para teste
            EscreveWord(100, 12345);  // Para lw $s0, 0($s1)
            EscreveHalfWord(12, 5678); // Para lhu $t1, 2($t2)
            EscreveByte(51, 99);      // Para lb $t5, 1($t6)

            // Carrega o arquivo de instruções
            string filePath = "dadosMIPS.txt";
            Console.WriteLine($"Carregando instruções do arquivo: {filePath}");

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Erro: Arquivo {filePath} não encontrado!");
                Console.WriteLine("Pressione Enter para sair...");
                Console.ReadLine();
                return;
            }

            string[] linhas = File.ReadAllLines(filePath);
            Console.WriteLine($"Arquivo carregado com {linhas.Length} instruções.");
            Console.WriteLine("Pressione Enter para iniciar a simulação...");
            Console.ReadLine();

            // Executa as instruções
            DateTime inicio = DateTime.Now;
            int contadorInstrucoes = 0;

            foreach (string linha in linhas)
            {
                string instrucao = linha.Trim();

                // Ignora linhas vazias
                if (string.IsNullOrWhiteSpace(instrucao))
                    continue;

                // Exibe informações da instrução atual
                Console.WriteLine($"Instrução #{contadorInstrucoes + 1}: {instrucao}");
                Console.WriteLine($"PC: 0x{pc:X8}");

                // Executa a instrução
                ExecutaInstrucao(instrucao);
                contadorInstrucoes++;

                // Exibe o estado dos registradores
                ExibeRegistradores();

                Console.WriteLine("\nPressione Enter para a próxima instrução...");
                Console.ReadLine();
                Console.WriteLine(new string('-', 50));
            }

            // Calcula o tempo de execução
            TimeSpan tempoExecucao = DateTime.Now - inicio;

            // Exibe o resumo final
            Console.WriteLine("\n=== RESUMO DA EXECUÇÃO ===");
            Console.WriteLine($"Instruções executadas: {contadorInstrucoes}");
            Console.WriteLine($"Tempo de execução: {tempoExecucao.TotalSeconds:F4} segundos");
            Console.WriteLine($"Instruções Tipo R: {instrucoesTipoR}");
            Console.WriteLine($"Instruções Tipo I: {instrucoesTipoI}");
            Console.WriteLine($"Instruções Tipo J: {instrucoesTipoJ}");

            Console.WriteLine("\nEstado final dos registradores:");
            ExibeRegistradores();

            Console.WriteLine("\nSimulação concluída! Pressione Enter para sair...");
            Console.ReadLine();
        }

        // Executa uma instrução MIPS
        static void ExecutaInstrucao(string instrucao)
        {
            string[] partes = instrucao.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (partes.Length == 0)
                return;

            string opcode = partes[0].ToLower();

            try
            {
                switch (opcode)
                {
                    // Instruções Tipo R
                    case "add":
                        {
                            int rd = ObtemIndiceRegistrador(partes[1]);
                            int rs = ObtemIndiceRegistrador(partes[2]);
                            int rt = ObtemIndiceRegistrador(partes[3]);
                            registradores[rd] = registradores[rs] + registradores[rt];
                            pc += 4;
                            instrucoesTipoR++;
                            Console.WriteLine($"Executado: {opcode} {partes[1]}, {partes[2]}, {partes[3]}");
                            Console.WriteLine($"Resultado: {partes[1]} = {registradores[rd]} (0x{registradores[rd]:X8})");
                        }
                        break;

                    case "sub":
                        {
                            int rd = ObtemIndiceRegistrador(partes[1]);
                            int rs = ObtemIndiceRegistrador(partes[2]);
                            int rt = ObtemIndiceRegistrador(partes[3]);
                            registradores[rd] = registradores[rs] - registradores[rt];
                            pc += 4;
                            instrucoesTipoR++;
                            Console.WriteLine($"Executado: {opcode} {partes[1]}, {partes[2]}, {partes[3]}");
                            Console.WriteLine($"Resultado: {partes[1]} = {registradores[rd]} (0x{registradores[rd]:X8})");
                        }
                        break;

                    case "and":
                        {
                            int rd = ObtemIndiceRegistrador(partes[1]);
                            int rs = ObtemIndiceRegistrador(partes[2]);
                            int rt = ObtemIndiceRegistrador(partes[3]);
                            registradores[rd] = registradores[rs] & registradores[rt];
                            pc += 4;
                            instrucoesTipoR++;
                            Console.WriteLine($"Executado: {opcode} {partes[1]}, {partes[2]}, {partes[3]}");
                            Console.WriteLine($"Resultado: {partes[1]} = {registradores[rd]} (0x{registradores[rd]:X8})");
                        }
                        break;

                    case "or":
                        {
                            int rd = ObtemIndiceRegistrador(partes[1]);
                            int rs = ObtemIndiceRegistrador(partes[2]);
                            int rt = ObtemIndiceRegistrador(partes[3]);
                            registradores[rd] = registradores[rs] | registradores[rt];
                            pc += 4;
                            instrucoesTipoR++;
                            Console.WriteLine($"Executado: {opcode} {partes[1]}, {partes[2]}, {partes[3]}");
                            Console.WriteLine($"Resultado: {partes[1]} = {registradores[rd]} (0x{registradores[rd]:X8})");
                        }
                        break;

                    case "nor":
                        {
                            int rd = ObtemIndiceRegistrador(partes[1]);
                            int rs = ObtemIndiceRegistrador(partes[2]);
                            int rt = ObtemIndiceRegistrador(partes[3]);
                            registradores[rd] = ~(registradores[rs] | registradores[rt]);
                            pc += 4;
                            instrucoesTipoR++;
                            Console.WriteLine($"Executado: {opcode} {partes[1]}, {partes[2]}, {partes[3]}");
                            Console.WriteLine($"Resultado: {partes[1]} = {registradores[rd]} (0x{registradores[rd]:X8})");
                        }
                        break;

                    case "sll":
                        {
                            int rd = ObtemIndiceRegistrador(partes[1]);
                            int rt = ObtemIndiceRegistrador(partes[2]);
                            int shamt = int.Parse(partes[3]);
                            registradores[rd] = registradores[rt] << shamt;
                            pc += 4;
                            instrucoesTipoR++;
                            Console.WriteLine($"Executado: {opcode} {partes[1]}, {partes[2]}, {partes[3]}");
                            Console.WriteLine($"Resultado: {partes[1]} = {registradores[rd]} (0x{registradores[rd]:X8})");
                        }
                        break;

                    case "srl":
                        {
                            int rd = ObtemIndiceRegistrador(partes[1]);
                            int rt = ObtemIndiceRegistrador(partes[2]);
                            int shamt = int.Parse(partes[3]);
                            registradores[rd] = registradores[rt] >> shamt;
                            pc += 4;
                            instrucoesTipoR++;
                            Console.WriteLine($"Executado: {opcode} {partes[1]}, {partes[2]}, {partes[3]}");
                            Console.WriteLine($"Resultado: {partes[1]} = {registradores[rd]} (0x{registradores[rd]:X8})");
                        }
                        break;

                    case "slt":
                        {
                            int rd = ObtemIndiceRegistrador(partes[1]);
                            int rs = ObtemIndiceRegistrador(partes[2]);
                            int rt = ObtemIndiceRegistrador(partes[3]);
                            registradores[rd] = (int)registradores[rs] < (int)registradores[rt] ? 1u : 0u;
                            pc += 4;
                            instrucoesTipoR++;
                            Console.WriteLine($"Executado: {opcode} {partes[1]}, {partes[2]}, {partes[3]}");
                            Console.WriteLine($"Resultado: {partes[1]} = {registradores[rd]} (0x{registradores[rd]:X8})");
                        }
                        break;

                    case "sltu":
                        {
                            int rd = ObtemIndiceRegistrador(partes[1]);
                            int rs = ObtemIndiceRegistrador(partes[2]);
                            int rt = ObtemIndiceRegistrador(partes[3]);
                            registradores[rd] = registradores[rs] < registradores[rt] ? 1u : 0u;
                            pc += 4;
                            instrucoesTipoR++;
                            Console.WriteLine($"Executado: {opcode} {partes[1]}, {partes[2]}, {partes[3]}");
                            Console.WriteLine($"Resultado: {partes[1]} = {registradores[rd]} (0x{registradores[rd]:X8})");
                        }
                        break;

                    case "jr":
                        {
                            int rs = ObtemIndiceRegistrador(partes[1]);
                            pc = registradores[rs];
                            instrucoesTipoR++;
                            Console.WriteLine($"Executado: {opcode} {partes[1]}");
                            Console.WriteLine($"Salto para PC = 0x{pc:X8}");
                        }
                        break;

                    // Instruções Tipo I
                    case "addi":
                        {
                            int rt = ObtemIndiceRegistrador(partes[1]);
                            int rs = ObtemIndiceRegistrador(partes[2]);
                            int imm = int.Parse(partes[3]);
                            registradores[rt] = registradores[rs] + (uint)imm;
                            pc += 4;
                            instrucoesTipoI++;
                            Console.WriteLine($"Executado: {opcode} {partes[1]}, {partes[2]}, {partes[3]}");
                            Console.WriteLine($"Resultado: {partes[1]} = {registradores[rt]} (0x{registradores[rt]:X8})");
                        }
                        break;

                    case "andi":
                        {
                            int rt = ObtemIndiceRegistrador(partes[1]);
                            int rs = ObtemIndiceRegistrador(partes[2]);
                            int imm = int.Parse(partes[3]);
                            registradores[rt] = registradores[rs] & (uint)imm;
                            pc += 4;
                            instrucoesTipoI++;
                            Console.WriteLine($"Executado: {opcode} {partes[1]}, {partes[2]}, {partes[3]}");
                            Console.WriteLine($"Resultado: {partes[1]} = {registradores[rt]} (0x{registradores[rt]:X8})");
                        }
                        break;

                    case "ori":
                        {
                            int rt = ObtemIndiceRegistrador(partes[1]);
                            int rs = ObtemIndiceRegistrador(partes[2]);
                            int imm = int.Parse(partes[3]);
                            registradores[rt] = registradores[rs] | (uint)imm;
                            pc += 4;
                            instrucoesTipoI++;
                            Console.WriteLine($"Executado: {opcode} {partes[1]}, {partes[2]}, {partes[3]}");
                            Console.WriteLine($"Resultado: {partes[1]} = {registradores[rt]} (0x{registradores[rt]:X8})");
                        }
                        break;

                    case "slti":
                        {
                            int rt = ObtemIndiceRegistrador(partes[1]);
                            int rs = ObtemIndiceRegistrador(partes[2]);
                            int imm = int.Parse(partes[3]);
                            registradores[rt] = (int)registradores[rs] < imm ? 1u : 0u;
                            pc += 4;
                            instrucoesTipoI++;
                            Console.WriteLine($"Executado: {opcode} {partes[1]}, {partes[2]}, {partes[3]}");
                            Console.WriteLine($"Resultado: {partes[1]} = {registradores[rt]} (0x{registradores[rt]:X8})");
                        }
                        break;

                    case "sltiu":
                        {
                            int rt = ObtemIndiceRegistrador(partes[1]);
                            int rs = ObtemIndiceRegistrador(partes[2]);
                            uint imm = uint.Parse(partes[3]);
                            registradores[rt] = registradores[rs] < imm ? 1u : 0u;
                            pc += 4;
                            instrucoesTipoI++;
                            Console.WriteLine($"Executado: {opcode} {partes[1]}, {partes[2]}, {partes[3]}");
                            Console.WriteLine($"Resultado: {partes[1]} = {registradores[rt]} (0x{registradores[rt]:X8})");
                        }
                        break;

                    case "lw":
                        {
                            int rt = ObtemIndiceRegistrador(partes[1]);
                            string[] offsetBase = partes[2].Split(new char[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                            int offset = int.Parse(offsetBase[0]);
                            int rs = ObtemIndiceRegistrador(offsetBase[1]);
                            uint endereco = registradores[rs] + (uint)offset;
                            registradores[rt] = LeWord(endereco);
                            pc += 4;
                            instrucoesTipoI++;
                            Console.WriteLine($"Executado: {opcode} {partes[1]}, {partes[2]}");
                            Console.WriteLine($"Carregado da memória[0x{endereco:X8}]: {registradores[rt]} (0x{registradores[rt]:X8})");
                        }
                        break;

                    case "lh":
                        {
                            int rt = ObtemIndiceRegistrador(partes[1]);
                            string[] offsetBase = partes[2].Split(new char[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                            int offset = int.Parse(offsetBase[0]);
                            int rs = ObtemIndiceRegistrador(offsetBase[1]);
                            uint endereco = registradores[rs] + (uint)offset;
                            ushort valor = LeHalfWord(endereco);
                            // Extensão de sinal
                            registradores[rt] = (valor & 0x8000) != 0 ? (uint)(valor | 0xFFFF0000) : valor;
                            pc += 4;
                            instrucoesTipoI++;
                            Console.WriteLine($"Executado: {opcode} {partes[1]}, {partes[2]}");
                            Console.WriteLine($"Carregado da memória[0x{endereco:X8}]: {registradores[rt]} (0x{registradores[rt]:X8})");
                        }
                        break;

                    case "lb":
                        {
                            int rt = ObtemIndiceRegistrador(partes[1]);
                            string[] offsetBase = partes[2].Split(new char[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                            int offset = int.Parse(offsetBase[0]);
                            int rs = ObtemIndiceRegistrador(offsetBase[1]);
                            uint endereco = registradores[rs] + (uint)offset;
                            byte valor = LeByte(endereco);
                            // Extensão de sinal
                            registradores[rt] = (valor & 0x80) != 0 ? (uint)(valor | 0xFFFFFF00) : valor;
                            pc += 4;
                            instrucoesTipoI++;
                            Console.WriteLine($"Executado: {opcode} {partes[1]}, {partes[2]}");
                            Console.WriteLine($"Carregado da memória[0x{endereco:X8}]: {registradores[rt]} (0x{registradores[rt]:X8})");
                        }
                        break;

                    case "lhu":
                        {
                            int rt = ObtemIndiceRegistrador(partes[1]);
                            string[] offsetBase = partes[2].Split(new char[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                            int offset = int.Parse(offsetBase[0]);
                            int rs = ObtemIndiceRegistrador(offsetBase[1]);
                            uint endereco = registradores[rs] + (uint)offset;
                            registradores[rt] = LeHalfWord(endereco);
                            pc += 4;
                            instrucoesTipoI++;
                            Console.WriteLine($"Executado: {opcode} {partes[1]}, {partes[2]}");
                            Console.WriteLine($"Carregado da memória[0x{endereco:X8}]: {registradores[rt]} (0x{registradores[rt]:X8})");
                        }
                        break;

                    case "sw":
                        {
                            int rt = ObtemIndiceRegistrador(partes[1]);
                            string[] offsetBase = partes[2].Split(new char[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                            int offset = int.Parse(offsetBase[0]);
                            int rs = ObtemIndiceRegistrador(offsetBase[1]);
                            uint endereco = registradores[rs] + (uint)offset;
                            EscreveWord(endereco, registradores[rt]);
                            pc += 4;
                            instrucoesTipoI++;
                            Console.WriteLine($"Executado: {opcode} {partes[1]}, {partes[2]}");
                            Console.WriteLine($"Armazenado na memória[0x{endereco:X8}]: {registradores[rt]} (0x{registradores[rt]:X8})");
                        }
                        break;

                    case "sh":
                        {
                            int rt = ObtemIndiceRegistrador(partes[1]);
                            string[] offsetBase = partes[2].Split(new char[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                            int offset = int.Parse(offsetBase[0]);
                            int rs = ObtemIndiceRegistrador(offsetBase[1]);
                            uint endereco = registradores[rs] + (uint)offset;
                            EscreveHalfWord(endereco, (ushort)(registradores[rt] & 0xFFFF));
                            pc += 4;
                            instrucoesTipoI++;
                            Console.WriteLine($"Executado: {opcode} {partes[1]}, {partes[2]}");
                            Console.WriteLine($"Armazenado na memória[0x{endereco:X8}]: {registradores[rt] & 0xFFFF} (0x{registradores[rt] & 0xFFFF:X4})");
                        }
                        break;

                    case "sb":
                        {
                            int rt = ObtemIndiceRegistrador(partes[1]);
                            string[] offsetBase = partes[2].Split(new char[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                            int offset = int.Parse(offsetBase[0]);
                            int rs = ObtemIndiceRegistrador(offsetBase[1]);
                            uint endereco = registradores[rs] + (uint)offset;
                            EscreveByte(endereco, (byte)(registradores[rt] & 0xFF));
                            pc += 4;
                            instrucoesTipoI++;
                            Console.WriteLine($"Executado: {opcode} {partes[1]}, {partes[2]}");
                            Console.WriteLine($"Armazenado na memória[0x{endereco:X8}]: {registradores[rt] & 0xFF} (0x{registradores[rt] & 0xFF:X2})");
                        }
                        break;

                    case "beq":
                        {
                            int rs = ObtemIndiceRegistrador(partes[1]);
                            int rt = ObtemIndiceRegistrador(partes[2]);
                            int offset = int.Parse(partes[3]);
                            if (registradores[rs] == registradores[rt])
                            {
                                pc += (uint)(offset * 4);
                                Console.WriteLine($"Desvio tomado para PC = 0x{pc:X8}");
                            }
                            else
                            {
                                pc += 4;
                                Console.WriteLine("Desvio não tomado");
                            }
                            instrucoesTipoI++;
                            Console.WriteLine($"Executado: {opcode} {partes[1]}, {partes[2]}, {partes[3]}");
                        }
                        break;

                    case "bne":
                        {
                            int rs = ObtemIndiceRegistrador(partes[1]);
                            int rt = ObtemIndiceRegistrador(partes[2]);
                            int offset = int.Parse(partes[3]);
                            if (registradores[rs] != registradores[rt])
                            {
                                pc += (uint)(offset * 4);
                                Console.WriteLine($"Desvio tomado para PC = 0x{pc:X8}");
                            }
                            else
                            {
                                pc += 4;
                                Console.WriteLine("Desvio não tomado");
                            }
                            instrucoesTipoI++;
                            Console.WriteLine($"Executado: {opcode} {partes[1]}, {partes[2]}, {partes[3]}");
                        }
                        break;

                    // Instruções Tipo J
                    case "j":
                        {
                            uint endereco = uint.Parse(partes[1]);
                            pc = (pc & 0xF0000000) | (endereco << 2);
                            instrucoesTipoJ++;
                            Console.WriteLine($"Executado: {opcode} {partes[1]}");
                            Console.WriteLine($"Salto para PC = 0x{pc:X8}");
                        }
                        break;

                    case "jal":
                        {
                            uint endereco = uint.Parse(partes[1]);
                            registradores[31] = pc + 4; // $ra = PC + 4
                            pc = (pc & 0xF0000000) | (endereco << 2);
                            instrucoesTipoJ++;
                            Console.WriteLine($"Executado: {opcode} {partes[1]}");
                            Console.WriteLine($"Salto para PC = 0x{pc:X8}, $ra = 0x{registradores[31]:X8}");
                        }
                        break;

                    default:
                        Console.WriteLine($"Instrução não reconhecida: {opcode}");
                        pc += 4;
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao executar instrução: {ex.Message}");
                pc += 4;
            }
        }

        // Obtém o índice do registrador a partir de seu nome
        static int ObtemIndiceRegistrador(string nome)
        {
            // Remove o $ se presente
            if (nome.StartsWith("$"))
                nome = nome.Substring(1);

            // Verifica se é um número direto
            if (int.TryParse(nome, out int indice))
            {
                if (indice >= 0 && indice < 32)
                    return indice;
            }

            // Verifica pelo nome
            if (nome == "zero") return 0;
            if (nome == "at") return 1;
            if (nome == "v0") return 2;
            if (nome == "v1") return 3;
            if (nome == "a0") return 4;
            if (nome == "a1") return 5;
            if (nome == "a2") return 6;
            if (nome == "a3") return 7;
            if (nome == "t0") return 8;
            if (nome == "t1") return 9;
            if (nome == "t2") return 10;
            if (nome == "t3") return 11;
            if (nome == "t4") return 12;
            if (nome == "t5") return 13;
            if (nome == "t6") return 14;
            if (nome == "t7") return 15;
            if (nome == "s0") return 16;
            if (nome == "s1") return 17;
            if (nome == "s2") return 18;
            if (nome == "s3") return 19;
            if (nome == "s4") return 20;
            if (nome == "s5") return 21;
            if (nome == "s6") return 22;
            if (nome == "s7") return 23;
            if (nome == "t8") return 24;
            if (nome == "t9") return 25;
            if (nome == "k0") return 26;
            if (nome == "k1") return 27;
            if (nome == "gp") return 28;
            if (nome == "sp") return 29;
            if (nome == "fp") return 30;
            if (nome == "ra") return 31;

            throw new ArgumentException($"Registrador inválido: {nome}");
        }

        // Exibe o estado dos registradores
        static void ExibeRegistradores()
        {
            Console.WriteLine("\nEstado dos registradores:");

            // Registradores temporários
            Console.WriteLine("Registradores temporários:");
            for (int i = 8; i <= 15; i++)
            {
                Console.WriteLine($"{nomeRegistradores[i]} (${i}): {registradores[i]} (0x{registradores[i]:X8})");
            }

            // Registradores salvos
            Console.WriteLine("\nRegistradores salvos:");
            for (int i = 16; i <= 23; i++)
            {
                Console.WriteLine($"{nomeRegistradores[i]} (${i}): {registradores[i]} (0x{registradores[i]:X8})");
            }

            // Outros registradores importantes
            Console.WriteLine($"\n$ra (${31}): {registradores[31]} (0x{registradores[31]:X8})");
        }

        // Funções de acesso à memória
        static byte LeByte(uint endereco)
        {
            if (memoria.TryGetValue(endereco, out byte valor))
                return valor;
            return 0;
        }

        static ushort LeHalfWord(uint endereco)
        {
            byte b0 = LeByte(endereco);
            byte b1 = LeByte(endereco + 1);
            return (ushort)((b1 << 8) | b0);
        }

        static uint LeWord(uint endereco)
        {
            byte b0 = LeByte(endereco);
            byte b1 = LeByte(endereco + 1);
            byte b2 = LeByte(endereco + 2);
            byte b3 = LeByte(endereco + 3);
            return (uint)((b3 << 24) | (b2 << 16) | (b1 << 8) | b0);
        }

        static void EscreveByte(uint endereco, byte valor)
        {
            memoria[endereco] = valor;
        }

        static void EscreveHalfWord(uint endereco, ushort valor)
        {
            EscreveByte(endereco, (byte)(valor & 0xFF));
            EscreveByte(endereco + 1, (byte)((valor >> 8) & 0xFF));
        }

        static void EscreveWord(uint endereco, uint valor)
        {
            EscreveByte(endereco, (byte)(valor & 0xFF));
            EscreveByte(endereco + 1, (byte)((valor >> 8) & 0xFF));
            EscreveByte(endereco + 2, (byte)((valor >> 16) & 0xFF));
            EscreveByte(endereco + 3, (byte)((valor >> 24) & 0xFF));
        }
    }
}