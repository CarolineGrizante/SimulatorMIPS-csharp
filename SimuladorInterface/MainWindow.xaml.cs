using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
using System.Globalization;

namespace SimuladorMIPSWPF
{
    public partial class MainWindow : Window
    {
        // Array de registradores
        private uint[] registradores = new uint[32];

        // Memória
        private Dictionary<uint, byte> memoria = new Dictionary<uint, byte>();

        // PC - Program Counter
        private uint pc = 0;

        // Contadores de instruções
        private int instrucoesTipoR = 0;
        private int instrucoesTipoI = 0;
        private int instrucoesTipoJ = 0;
        private int instrucoesExecutadas = 0;

        // Lista de instruções
        private List<string> instrucoes = new List<string>();
        private int instrucaoAtual = -1;

        // Clock frequency em MHz (valor padrão)
        private double clockMHz = 100.0;

        // Último endereço de memória acessado (para destacar na visualização)
        private uint ultimoEnderecoAcessado = 0;
        private bool memoriaAcessada = false; // Flag para indicar se a memória foi lida ou escrita

        // Classe para representar um registrador na interface
        public class Registrador
        {
            public string Nome { get; set; } = string.Empty;
            public string ValorDecimal { get; set; } = string.Empty;
            public string ValorHex { get; set; } = string.Empty;
        }

        // Classe para representar uma linha de memória agrupada
        public class LinhaMemoria
        {
            public string EnderecoHex { get; set; } = string.Empty;
            public uint EnderecoBase { get; set; } // uint não é anulável por padrão
            public string Valor0Hex { get; set; } = string.Empty;
            public string Valor4Hex { get; set; } = string.Empty;
            public string Valor8Hex { get; set; } = string.Empty;
            public string ValorCHex { get; set; } = string.Empty;
        }

        public MainWindow()
        {
            InitializeComponent();

            // Inicializa registradores
            for (int i = 0; i < 32; i++)
            {
                registradores[i] = 0;
            }

            // Inicializa a interface
            AtualizarRegistradores();
            AtualizarEstatisticas();
            AtualizarVisualizacaoMemoria(); // Inicializa a visualização da memória

            // Desabilita botões até que um programa seja carregado
            btnExecutarPasso.IsEnabled = false;
            btnExecutarTudo.IsEnabled = false;
            btnReiniciar.IsEnabled = false;
        }

        // Carrega o programa do arquivo
        private void btnCarregar_Click(object sender, RoutedEventArgs e)
        {
            string filePath = txtArquivo.Text.Trim();

            // Valida a frequência do clock ao carregar
            if (!double.TryParse(txtClockMHz.Text.Trim().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out clockMHz) || clockMHz <= 0)
            {
                MessageBox.Show("Frequência do Clock inválida! Usando valor padrão de 100 MHz.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                clockMHz = 100.0;
                txtClockMHz.Text = clockMHz.ToString(CultureInfo.InvariantCulture);
            }

            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"Arquivo '{filePath}' não encontrado!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Limpa o estado atual
                Reiniciar();

                // Lê as instruções do arquivo
                instrucoes = File.ReadAllLines(filePath)
                    .Select(line => line.Split('#')[0].Trim()) // Remove comentários e espaços extras
                    .Where(line => !string.IsNullOrWhiteSpace(line)) // Ignora linhas vazias ou só com comentários
                    .ToList();

                // Preenche a lista de instruções
                lstInstrucoes.Items.Clear();
                foreach (string instrucao in instrucoes)
                {
                    lstInstrucoes.Items.Add(instrucao);
                }

                // Atualiza a visualização da memória após inicialização (estará vazia)
                AtualizarVisualizacaoMemoria();

                // Habilita os botões
                btnExecutarPasso.IsEnabled = true;
                btnExecutarTudo.IsEnabled = true;
                btnReiniciar.IsEnabled = true;

                // Seleciona a primeira instrução
                if (lstInstrucoes.Items.Count > 0)
                {
                    lstInstrucoes.SelectedIndex = 0;
                    instrucaoAtual = 0;
                    txtInstrucaoAtual.Text = instrucoes[0];
                }
                else
                {
                    txtInstrucaoAtual.Text = "Nenhuma instrução carregada";
                }

                MessageBox.Show($"Programa carregado com sucesso! {instrucoes.Count} instruções encontradas. Clock definido para {clockMHz.ToString(CultureInfo.InvariantCulture)} MHz.",
                    "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar o arquivo: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // Atualiza as estatísticas após carregar (tempo será 0 inicialmente)
            AtualizarEstatisticas();
        }

        // Executa uma instrução
        private void btnExecutarPasso_Click(object sender, RoutedEventArgs e)
        {
            if (instrucaoAtual < 0 || instrucaoAtual >= instrucoes.Count)
                return;

            // Obtém a instrução atual
            string instrucao = instrucoes[instrucaoAtual];
            txtInstrucaoAtual.Text = instrucao;

            // Reseta a flag de acesso à memória
            memoriaAcessada = false;

            // Executa a instrução
            ExecutaInstrucao(instrucao);

            // Incrementa o contador de instruções executadas
            instrucoesExecutadas++;

            // Atualiza a interface
            AtualizarRegistradores();

            // Atualiza a visualização da memória se houve acesso
            if (memoriaAcessada)
            {
                AtualizarVisualizacaoMemoria();
            }

            AtualizarEstatisticas(); // Atualiza estatísticas APÓS executar

            // Avança para a próxima instrução
            instrucaoAtual++;

            // Seleciona a próxima instrução na lista
            if (instrucaoAtual < instrucoes.Count)
            {
                lstInstrucoes.SelectedIndex = instrucaoAtual;
            }
            else
            {
                // Fim do programa
                MessageBox.Show("Fim do programa atingido!", "Simulação Concluída", MessageBoxButton.OK, MessageBoxImage.Information);
                btnExecutarPasso.IsEnabled = false;
                btnExecutarTudo.IsEnabled = false;
            }
        }

        // Executa todas as instruções
        private void btnExecutarTudo_Click(object sender, RoutedEventArgs e)
        {
            // Desabilita os botões durante a execução
            btnExecutarPasso.IsEnabled = false;
            btnExecutarTudo.IsEnabled = false;
            btnCarregar.IsEnabled = false;

            bool houveAcessoMemoria = false; // Flag para atualizar memória no final

            try
            {
                // Executa todas as instruções restantes
                while (instrucaoAtual < instrucoes.Count)
                {
                    // Obtém a instrução atual
                    string instrucao = instrucoes[instrucaoAtual];

                    // Reseta a flag de acesso à memória para cada instrução
                    memoriaAcessada = false;

                    // Executa a instrução
                    ExecutaInstrucao(instrucao);

                    // Se houve acesso à memória em qualquer instrução, marca para atualizar no final
                    if (memoriaAcessada) houveAcessoMemoria = true;

                    // Incrementa o contador de instruções executadas
                    instrucoesExecutadas++;

                    // Avança para a próxima instrução
                    instrucaoAtual++;
                }

                // Atualiza a interface APÓS todas as execuções
                AtualizarRegistradores();

                // Atualiza a visualização da memória se houve acesso
                if (houveAcessoMemoria)
                {
                    AtualizarVisualizacaoMemoria();
                }

                AtualizarEstatisticas();

                txtInstrucaoAtual.Text = "Fim do Programa";
                lstInstrucoes.SelectedIndex = -1;

                MessageBox.Show("Todas as instruções foram executadas!", "Simulação Concluída", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro durante a execução: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);

                // Atualiza a interface mesmo em caso de erro
                AtualizarRegistradores();
                if (houveAcessoMemoria) AtualizarVisualizacaoMemoria();
                AtualizarEstatisticas();
            }
            finally
            {
                // Reabilita os botões
                btnCarregar.IsEnabled = true;
                btnReiniciar.IsEnabled = true;
                btnExecutarPasso.IsEnabled = false; // Desabilita passo após tudo
                btnExecutarTudo.IsEnabled = false; // Desabilita tudo após tudo
            }
        }

        // Reinicia a simulação
        private void btnReiniciar_Click(object sender, RoutedEventArgs e)
        {
            Reiniciar();

            // Recarrega as instruções na lista
            if (instrucoes.Count > 0)
            {
                lstInstrucoes.SelectedIndex = 0;
                instrucaoAtual = 0;
                txtInstrucaoAtual.Text = instrucoes[0];

                btnExecutarPasso.IsEnabled = true;
                btnExecutarTudo.IsEnabled = true;
            }
            else
            {
                btnExecutarPasso.IsEnabled = false;
                btnExecutarTudo.IsEnabled = false;
            }
            btnReiniciar.IsEnabled = true; // Manter reiniciar habilitado

            MessageBox.Show("Simulação reiniciada!", "Reiniciar", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Reinicia o estado da simulação
        private void Reiniciar()
        {
            // Reinicia registradores
            for (int i = 0; i < 32; i++)
            {
                registradores[i] = 0;
            }

            // Limpa a memória
            memoria.Clear();

            // Reinicia contadores
            pc = 0;
            instrucoesTipoR = 0;
            instrucoesTipoI = 0;
            instrucoesTipoJ = 0;
            instrucoesExecutadas = 0;
            instrucaoAtual = 0;
            ultimoEnderecoAcessado = 0;
            memoriaAcessada = false;

            // Atualiza a interface
            txtPC.Text = $"0x{pc:X8}";
            AtualizarRegistradores();
            AtualizarVisualizacaoMemoria(); // Atualiza a visualização da memória
            AtualizarEstatisticas(); // Tempo será 0 aqui

            // Limpa a lista de instruções na interface
            // lstInstrucoes.Items.Clear(); // Não limpar aqui, limpar ao carregar
            txtInstrucaoAtual.Text = "Nenhuma instrução carregada";
        }

        // Atualiza a exibição dos registradores
        private void AtualizarRegistradores()
        {
            // Atualiza o PC
            txtPC.Text = $"0x{pc:X8}";

            // Limpa a lista de registradores
            lstRegistradores.Items.Clear();

            // Adiciona os registradores zero, at, v0-v1, a0-a3
            for (int i = 0; i <= 7; i++)
            {
                string nome;
                switch (i)
                {
                    case 0: nome = "$zero"; break;
                    case 1: nome = "$at"; break;
                    case 2: nome = "$v0"; break;
                    case 3: nome = "$v1"; break;
                    case 4: nome = "$a0"; break;
                    case 5: nome = "$a1"; break;
                    case 6: nome = "$a2"; break;
                    case 7: nome = "$a3"; break;
                    default: nome = $"${i}"; break;
                }

                lstRegistradores.Items.Add(new Registrador
                {
                    Nome = $"{nome} (${i})",
                    ValorDecimal = registradores[i].ToString(),
                    ValorHex = $"0x{registradores[i]:X8}"
                });
            }

            // Adiciona os registradores temporários t0-t9
            for (int i = 8; i <= 15; i++)
            {
                lstRegistradores.Items.Add(new Registrador
                {
                    Nome = $"$t{i - 8} (${i})",
                    ValorDecimal = registradores[i].ToString(),
                    ValorHex = $"0x{registradores[i]:X8}"
                });
            }

            // Adiciona os registradores salvos s0-s7
            for (int i = 16; i <= 23; i++)
            {
                lstRegistradores.Items.Add(new Registrador
                {
                    Nome = $"$s{i - 16} (${i})",
                    ValorDecimal = registradores[i].ToString(),
                    ValorHex = $"0x{registradores[i]:X8}"
                });
            }

            // Adiciona os registradores t8-t9, k0-k1, gp, sp, fp, ra
            for (int i = 24; i <= 31; i++)
            {
                string nome;
                switch (i)
                {
                    case 24: nome = "$t8"; break;
                    case 25: nome = "$t9"; break;
                    case 26: nome = "$k0"; break;
                    case 27: nome = "$k1"; break;
                    case 28: nome = "$gp"; break;
                    case 29: nome = "$sp"; break;
                    case 30: nome = "$fp"; break;
                    case 31: nome = "$ra"; break;
                    default: nome = $"${i}"; break;
                }

                lstRegistradores.Items.Add(new Registrador
                {
                    Nome = $"{nome} (${i})",
                    ValorDecimal = registradores[i].ToString(),
                    ValorHex = $"0x{registradores[i]:X8}"
                });
            }
        }

        // Atualiza a visualização da memória agrupada
        private void AtualizarVisualizacaoMemoria()
        {
            if (lstMemoria == null) return;

            lstMemoria.Items.Clear();

            uint enderecoInicioAlinhado;
            uint enderecoFimAlinhado;
            uint enderecoFoco;
            const int maxLinhasVisiveis = 20; // Número de linhas (16 bytes cada) a exibir
            const uint tamanhoBloco = 16;

            if (memoria.Count == 0)
            {
                // Memória vazia: Exibe um range padrão inicial (ex: 0x0000 a 0x00FF)
                enderecoInicioAlinhado = 0;
                enderecoFimAlinhado = enderecoInicioAlinhado + (uint)(maxLinhasVisiveis * tamanhoBloco);
                enderecoFoco = 0; // Foca no início
            }
            else
            {
                // Memória contém dados
                var enderecos = memoria.Keys.ToList();
                uint enderecoMinimo = enderecos.Min();
                uint enderecoMaximo = enderecos.Max();

                // Define o foco: último acessado ou o mínimo se nenhum foi acessado ainda
                enderecoFoco = (ultimoEnderecoAcessado > 0) ? ultimoEnderecoAcessado : enderecoMinimo;

                // Calcula o bloco (linha de 16 bytes) onde está o foco
                uint blocoFoco = enderecoFoco & ~0xFu; // Alinha o foco para o início do bloco de 16 bytes

                // Calcula o início da janela de visualização, tentando centralizar o foco
                uint inicioCalculado = (blocoFoco > (maxLinhasVisiveis / 2) * tamanhoBloco)
                                        ? blocoFoco - (uint)((maxLinhasVisiveis / 2) * tamanhoBloco)
                                        : 0;
                inicioCalculado &= ~0xFu; // Garante alinhamento de 16 bytes

                // Calcula o fim da janela de visualização
                uint fimCalculado = inicioCalculado + (uint)(maxLinhasVisiveis * tamanhoBloco);

                // Define os limites reais da visualização baseados nos dados e na janela calculada
                enderecoInicioAlinhado = Math.Max(inicioCalculado, (enderecoMinimo & ~0xFu));
                enderecoInicioAlinhado &= ~0xFu; // Re-alinha por segurança

                // O fim é o menor entre o fim calculado e o fim real dos dados (alinhado e estendido para incluir o último bloco)
                uint fimRealAlinhado = ((enderecoMaximo + tamanhoBloco - 1) & ~0xFu) + tamanhoBloco; // Garante que o bloco do máximo seja incluído
                enderecoFimAlinhado = Math.Min(fimCalculado, fimRealAlinhado);
            }

            LinhaMemoria? linhaSelecionada = null;

            // Adiciona as linhas de memória à ListView
            for (uint enderecoBase = enderecoInicioAlinhado; enderecoBase < enderecoFimAlinhado; enderecoBase += tamanhoBloco)
            {
                LinhaMemoria linha = new LinhaMemoria
                {
                    EnderecoHex = $"0x{enderecoBase:X8}",
                    EnderecoBase = enderecoBase,
                    Valor0Hex = FormatarWord(enderecoBase + 0),
                    Valor4Hex = FormatarWord(enderecoBase + 4),
                    Valor8Hex = FormatarWord(enderecoBase + 8),
                    ValorCHex = FormatarWord(enderecoBase + 12)
                };

                lstMemoria.Items.Add(linha);

                // Verifica se esta linha contém o último endereço acessado para seleção
                if ((ultimoEnderecoAcessado & ~0xFu) == enderecoBase)
                {
                    linhaSelecionada = linha;
                }
            }

            // Seleciona e rola para a linha após adicionar todos os itens
            if (linhaSelecionada != null)
            {
                lstMemoria.SelectedItem = linhaSelecionada;
                lstMemoria.ScrollIntoView(linhaSelecionada);
            }
            else if (lstMemoria.Items.Count > 0)
            {
                // Se não encontrou a linha com o endereço acessado, seleciona a primeira linha
                lstMemoria.SelectedIndex = 0;
                lstMemoria.ScrollIntoView(lstMemoria.Items[0]);
            }
        }

        // Formata uma word (4 bytes) da memória para exibição
        private string FormatarWord(uint endereco)
        {
            uint valor = LeWord(endereco);
            return $"0x{valor:X8}";
        }

        // Atualiza as estatísticas
        private void AtualizarEstatisticas()
        {
            txtInstrucoesExecutadas.Text = instrucoesExecutadas.ToString();
            txtTipoR.Text = instrucoesTipoR.ToString();
            txtTipoI.Text = instrucoesTipoI.ToString();
            txtTipoJ.Text = instrucoesTipoJ.ToString();

            // Lê e valida o clock da caixa de texto SEMPRE que atualizar estatísticas
            double currentClockMHz = 100.0; // Valor padrão
            if (!double.TryParse(txtClockMHz.Text.Trim().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out currentClockMHz) || currentClockMHz <= 0)
            {
                // Se inválido durante a execução, usa o último valor válido ou padrão
                currentClockMHz = clockMHz; // Usa o último valor válido carregado
            }
            else
            {
                clockMHz = currentClockMHz; // Atualiza o valor da classe se válido
            }

            // Calcula o tempo simulado baseado no clock ATUAL e instruções executadas
            if (currentClockMHz > 0 && instrucoesExecutadas > 0)
            {
                double clockHz = currentClockMHz * 1_000_000;
                double tempoSimuladoSegundos = (double)instrucoesExecutadas / clockHz;
                double tempoMicrosegundos = tempoSimuladoSegundos * 1_000_000; // Converte para microssegundos
                txtTempo.Text = $"{tempoMicrosegundos:F3} µs"; // Exibe em µs com 3 casas decimais
            }
            else
            {
                txtTempo.Text = "0.000 µs"; // Exibição padrão em µs
            }
        }

        // Executa uma instrução MIPS
        private void ExecutaInstrucao(string instrucao)
        {
            string[] partes = instrucao.Split(new char[] { ' ', ',', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);

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
                        }
                        break;

                    case "jr":
                        {
                            int rs = ObtemIndiceRegistrador(partes[1]);
                            pc = registradores[rs];
                            instrucoesTipoR++;
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
                        }
                        break;

                    case "andi": // Adicionado andi que faltava
                        {
                            int rt = ObtemIndiceRegistrador(partes[1]);
                            int rs = ObtemIndiceRegistrador(partes[2]);
                            ushort imm = ushort.Parse(partes[3]); // Imediato é 16 bits unsigned
                            registradores[rt] = registradores[rs] & imm;
                            pc += 4;
                            instrucoesTipoI++;
                        }
                        break;

                    case "ori":
                        {
                            int rt = ObtemIndiceRegistrador(partes[1]);
                            int rs = ObtemIndiceRegistrador(partes[2]);
                            ushort imm = ushort.Parse(partes[3]); // Imediato é 16 bits unsigned
                            registradores[rt] = registradores[rs] | imm;
                            pc += 4;
                            instrucoesTipoI++;
                        }
                        break;

                    case "slti": // Corrigido slti que faltava
                        {
                            int rt = ObtemIndiceRegistrador(partes[1]);
                            int rs = ObtemIndiceRegistrador(partes[2]);
                            short imm = short.Parse(partes[3]); // Imediato é 16 bits signed
                            registradores[rt] = (int)registradores[rs] < imm ? 1u : 0u;
                            pc += 4;
                            instrucoesTipoI++;
                        }
                        break;

                    case "sltiu":
                        {
                            int rt = ObtemIndiceRegistrador(partes[1]);
                            int rs = ObtemIndiceRegistrador(partes[2]);
                            ushort imm = ushort.Parse(partes[3]); // Imediato é 16 bits unsigned
                            registradores[rt] = registradores[rs] < imm ? 1u : 0u;
                            pc += 4;
                            instrucoesTipoI++;
                        }
                        break;

                    case "lw":
                        {
                            int rt = ObtemIndiceRegistrador(partes[1]);
                            int offset = int.Parse(partes[2]);
                            int rs = ObtemIndiceRegistrador(partes[3]);
                            uint endereco = registradores[rs] + (uint)offset;
                            ultimoEnderecoAcessado = endereco;
                            memoriaAcessada = true;
                            registradores[rt] = LeWord(endereco);
                            pc += 4;
                            instrucoesTipoI++;
                        }
                        break;

                    case "lh":
                        {
                            int rt = ObtemIndiceRegistrador(partes[1]);
                            int offset = int.Parse(partes[2]);
                            int rs = ObtemIndiceRegistrador(partes[3]);
                            uint endereco = registradores[rs] + (uint)offset;
                            ultimoEnderecoAcessado = endereco;
                            memoriaAcessada = true;
                            ushort valor = LeHalfWord(endereco);
                            // Extensão de sinal
                            registradores[rt] = (valor & 0x8000) != 0 ? (uint)(valor | 0xFFFF0000) : valor;
                            pc += 4;
                            instrucoesTipoI++;
                        }
                        break;

                    case "lb":
                        {
                            int rt = ObtemIndiceRegistrador(partes[1]);
                            int offset = int.Parse(partes[2]);
                            int rs = ObtemIndiceRegistrador(partes[3]);
                            uint endereco = registradores[rs] + (uint)offset;
                            ultimoEnderecoAcessado = endereco;
                            memoriaAcessada = true;
                            byte valor = LeByte(endereco);
                            // Extensão de sinal
                            registradores[rt] = (valor & 0x80) != 0 ? (uint)(valor | 0xFFFFFF00) : valor;
                            pc += 4;
                            instrucoesTipoI++;
                        }
                        break;

                    case "lhu":
                        {
                            int rt = ObtemIndiceRegistrador(partes[1]);
                            int offset = int.Parse(partes[2]);
                            int rs = ObtemIndiceRegistrador(partes[3]);
                            uint endereco = registradores[rs] + (uint)offset;
                            ultimoEnderecoAcessado = endereco;
                            memoriaAcessada = true;
                            registradores[rt] = LeHalfWord(endereco);
                            pc += 4;
                            instrucoesTipoI++;
                        }
                        break;

                    case "sw":
                        {
                            int rt = ObtemIndiceRegistrador(partes[1]);
                            int offset = int.Parse(partes[2]);
                            int rs = ObtemIndiceRegistrador(partes[3]);
                            uint endereco = registradores[rs] + (uint)offset;
                            ultimoEnderecoAcessado = endereco;
                            memoriaAcessada = true;
                            EscreveWord(endereco, registradores[rt]);
                            pc += 4;
                            instrucoesTipoI++;
                        }
                        break;

                    case "sh":
                        {
                            int rt = ObtemIndiceRegistrador(partes[1]);
                            int offset = int.Parse(partes[2]);
                            int rs = ObtemIndiceRegistrador(partes[3]);
                            uint endereco = registradores[rs] + (uint)offset;
                            ultimoEnderecoAcessado = endereco;
                            memoriaAcessada = true;
                            EscreveHalfWord(endereco, (ushort)(registradores[rt] & 0xFFFF));
                            pc += 4;
                            instrucoesTipoI++;
                        }
                        break;

                    case "sb":
                        {
                            int rt = ObtemIndiceRegistrador(partes[1]);
                            int offset = int.Parse(partes[2]);
                            int rs = ObtemIndiceRegistrador(partes[3]);
                            uint endereco = registradores[rs] + (uint)offset;
                            ultimoEnderecoAcessado = endereco;
                            memoriaAcessada = true;
                            EscreveByte(endereco, (byte)(registradores[rt] & 0xFF));
                            pc += 4;
                            instrucoesTipoI++;
                        }
                        break;

                    case "beq":
                        {
                            int rs = ObtemIndiceRegistrador(partes[1]);
                            int rt = ObtemIndiceRegistrador(partes[2]);
                            int offset = int.Parse(partes[3]); // Offset em número de instruções
                            if (registradores[rs] == registradores[rt])
                            {
                                // PC = PC + 4 + offset * 4
                                pc = pc + 4 + (uint)(offset * 4);
                            }
                            else
                            {
                                pc += 4;
                            }
                            instrucoesTipoI++;
                        }
                        break;

                    case "bne":
                        {
                            int rs = ObtemIndiceRegistrador(partes[1]);
                            int rt = ObtemIndiceRegistrador(partes[2]);
                            int offset = int.Parse(partes[3]); // Offset em número de instruções
                            if (registradores[rs] != registradores[rt])
                            {
                                // PC = PC + 4 + offset * 4
                                pc = pc + 4 + (uint)(offset * 4);
                            }
                            else
                            {
                                pc += 4;
                            }
                            instrucoesTipoI++;
                        }
                        break;

                    // Instruções Tipo J
                    case "j":
                        {
                            uint endereco = uint.Parse(partes[1]); // Endereço alvo da instrução
                            // PC = (primeiros 4 bits do PC atual) | (endereco * 4)
                            pc = (pc & 0xF0000000) | (endereco << 2);
                            instrucoesTipoJ++;
                        }
                        break;

                    case "jal":
                        {
                            uint endereco = uint.Parse(partes[1]); // Endereço alvo da instrução
                            registradores[31] = pc + 4; // $ra = PC + 4 (endereço da próxima instrução)
                            // PC = (primeiros 4 bits do PC atual) | (endereco * 4)
                            pc = (pc & 0xF0000000) | (endereco << 2);
                            instrucoesTipoJ++;
                        }
                        break;

                    default:
                        MessageBox.Show($"Instrução não reconhecida: {opcode}", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                        pc += 4;
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao executar instrução '{instrucao}': {ex.Message}", "Erro de Execução", MessageBoxButton.OK, MessageBoxImage.Error);
                instrucaoAtual = instrucoes.Count; // Força o fim
                btnExecutarPasso.IsEnabled = false;
                btnExecutarTudo.IsEnabled = false;
            }
        }

        // Obtém o índice do registrador a partir de seu nome
        private int ObtemIndiceRegistrador(string nome)
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
            switch (nome.ToLower())
            {
                case "zero": return 0;
                case "at": return 1;
                case "v0": return 2;
                case "v1": return 3;
                case "a0": return 4;
                case "a1": return 5;
                case "a2": return 6;
                case "a3": return 7;
                case "t0": return 8;
                case "t1": return 9;
                case "t2": return 10;
                case "t3": return 11;
                case "t4": return 12;
                case "t5": return 13;
                case "t6": return 14;
                case "t7": return 15;
                case "s0": return 16;
                case "s1": return 17;
                case "s2": return 18;
                case "s3": return 19;
                case "s4": return 20;
                case "s5": return 21;
                case "s6": return 22;
                case "s7": return 23;
                case "t8": return 24;
                case "t9": return 25;
                case "k0": return 26;
                case "k1": return 27;
                case "gp": return 28;
                case "sp": return 29;
                case "fp": return 30;
                case "ra": return 31;
                default: throw new ArgumentException($"Registrador inválido: {nome}");
            }
        }

        // Funções de acesso à memória
        private byte LeByte(uint endereco)
        {
            if (memoria.TryGetValue(endereco, out byte valor))
                return valor;
            // Retorna 0 se o endereço não foi escrito, simulando memória inicializada com 0
            return 0;
        }

        private ushort LeHalfWord(uint endereco)
        {
            // Assumindo little-endian para dados, como é comum em muitas simulações.
            byte b0 = LeByte(endereco);       // Byte menos significativo
            byte b1 = LeByte(endereco + 1);   // Byte mais significativo
            return (ushort)((b1 << 8) | b0);
        }

        private uint LeWord(uint endereco)
        {
            // Assumindo little-endian
            byte b0 = LeByte(endereco);
            byte b1 = LeByte(endereco + 1);
            byte b2 = LeByte(endereco + 2);
            byte b3 = LeByte(endereco + 3);
            return (uint)((b3 << 24) | (b2 << 16) | (b1 << 8) | b0);
        }

        private void EscreveByte(uint endereco, byte valor)
        {
            memoria[endereco] = valor;
        }

        private void EscreveHalfWord(uint endereco, ushort valor)
        {
            // Assumindo little-endian
            EscreveByte(endereco, (byte)(valor & 0xFF));          // Byte menos significativo
            EscreveByte(endereco + 1, (byte)((valor >> 8) & 0xFF)); // Byte mais significativo
        }

        private void EscreveWord(uint endereco, uint valor)
        {
            // Assumindo little-endian
            EscreveByte(endereco, (byte)(valor & 0xFF));
            EscreveByte(endereco + 1, (byte)((valor >> 8) & 0xFF));
            EscreveByte(endereco + 2, (byte)((valor >> 16) & 0xFF));
            EscreveByte(endereco + 3, (byte)((valor >> 24) & 0xFF));
        }
    }
}