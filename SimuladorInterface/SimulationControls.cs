using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SimuladorComum;

namespace SimuladorInterface
{
    // Classe para controles de simulação (iniciar, pausar, parar)
    public class SimulationControls
    {
        // Componentes da interface
        private Grid _mainGrid;
        private StackPanel _controlPanel;
        private Button _loadButton;
        private Button _runButton;
        private Button _pauseButton;
        private Button _stepButton;
        private Button _resetButton;
        private TextBox _codeTextBox;
        private GroupBox _controlGroupBox;

        // Referência para o simulador
        private ISimulator _simulator;

        // Construtor
        public SimulationControls(Grid mainGrid, ISimulator simulator)
        {
            _mainGrid = mainGrid;
            _simulator = simulator;
            InitializeComponents();
        }

        // Inicializa os componentes da interface
        private void InitializeComponents()
        {
            // Cria o GroupBox para os controles
            _controlGroupBox = new GroupBox
            {
                Header = "Controles de Simulação",
                Margin = new Thickness(10),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            // Cria o painel de controles
            _controlPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(5)
            };

            // Cria o TextBox para o código assembly
            _codeTextBox = new TextBox
            {
                AcceptsReturn = true,
                AcceptsTab = true,
                TextWrapping = TextWrapping.Wrap,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                Height = 200,
                FontFamily = new FontFamily("Consolas"),
                Margin = new Thickness(0, 0, 0, 10)
            };

            // Cria os botões de controle
            _loadButton = new Button
            {
                Content = "Carregar Programa",
                Margin = new Thickness(5),
                Padding = new Thickness(10, 5, 10, 5)
            };

            _runButton = new Button
            {
                Content = "Executar",
                Margin = new Thickness(5),
                Padding = new Thickness(10, 5, 10, 5)
            };

            _pauseButton = new Button
            {
                Content = "Pausar",
                Margin = new Thickness(5),
                Padding = new Thickness(10, 5, 10, 5),
                IsEnabled = false
            };

            _stepButton = new Button
            {
                Content = "Passo a Passo",
                Margin = new Thickness(5),
                Padding = new Thickness(10, 5, 10, 5)
            };

            _resetButton = new Button
            {
                Content = "Reiniciar",
                Margin = new Thickness(5),
                Padding = new Thickness(10, 5, 10, 5)
            };

            // Adiciona os manipuladores de eventos
            _loadButton.Click += LoadButton_Click;
            _runButton.Click += RunButton_Click;
            _pauseButton.Click += PauseButton_Click;
            _stepButton.Click += StepButton_Click;
            _resetButton.Click += ResetButton_Click;

            // Cria um painel horizontal para os botões
            StackPanel buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            };

            // Adiciona os botões ao painel horizontal
            buttonPanel.Children.Add(_loadButton);
            buttonPanel.Children.Add(_runButton);
            buttonPanel.Children.Add(_pauseButton);
            buttonPanel.Children.Add(_stepButton);
            buttonPanel.Children.Add(_resetButton);

            // Adiciona o TextBox e o painel de botões ao painel de controles
            _controlPanel.Children.Add(new TextBlock { Text = "Código Assembly MIPS:", Margin = new Thickness(0, 0, 0, 5) });
            _controlPanel.Children.Add(_codeTextBox);
            _controlPanel.Children.Add(buttonPanel);

            // Adiciona o painel de controles ao GroupBox
            _controlGroupBox.Content = _controlPanel;

            // Adiciona o GroupBox ao Grid principal
            _mainGrid.Children.Add(_controlGroupBox);

            // Posiciona o GroupBox no Grid
            Grid.SetRow(_controlGroupBox, 0);
            Grid.SetColumn(_controlGroupBox, 0);
            Grid.SetColumnSpan(_controlGroupBox, 2);
        }

        // Manipuladores de eventos
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string code = _codeTextBox.Text;
                if (string.IsNullOrWhiteSpace(code))
                {
                    MessageBox.Show("Por favor, insira o código assembly MIPS.", "Simulador MIPS", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _simulator.LoadProgram(code);
                MessageBox.Show("Programa carregado com sucesso!", "Simulador MIPS", MessageBoxButton.OK, MessageBoxImage.Information);

                _runButton.IsEnabled = true;
                _stepButton.IsEnabled = true;
                _resetButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar programa: {ex.Message}", "Simulador MIPS", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            _simulator.Run();
            _runButton.IsEnabled = false;
            _pauseButton.IsEnabled = true;
            _stepButton.IsEnabled = false;
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            _simulator.Pause();
            _runButton.IsEnabled = true;
            _pauseButton.IsEnabled = false;
            _stepButton.IsEnabled = true;
        }

        private void StepButton_Click(object sender, RoutedEventArgs e)
        {
            _simulator.Step();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            _simulator.Reset();
            _runButton.IsEnabled = true;
            _pauseButton.IsEnabled = false;
            _stepButton.IsEnabled = true;
        }
    }
}