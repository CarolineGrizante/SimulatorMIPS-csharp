using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SimuladorComum;

namespace SimuladorInterface
{
    // Classe para visualização do estado da memória
    public class MemoryVisualizer
    {
        // Componentes da interface
        private Grid _mainGrid;
        private DataGrid _memoryGrid;
        private GroupBox _memoryGroupBox;

        // Construtor
        public MemoryVisualizer(Grid mainGrid)
        {
            _mainGrid = mainGrid;
            InitializeComponents();
        }

        // Inicializa os componentes da interface
        private void InitializeComponents()
        {
            // Cria o GroupBox para a memória
            _memoryGroupBox = new GroupBox
            {
                Header = "Memória",
                Margin = new Thickness(10),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = 300
            };

            // Cria o DataGrid para exibir a memória
            _memoryGrid = new DataGrid
            {
                AutoGenerateColumns = false,
                IsReadOnly = true,
                CanUserAddRows = false,
                CanUserDeleteRows = false,
                CanUserReorderColumns = false,
                CanUserResizeRows = false,
                CanUserSortColumns = false,
                HeadersVisibility = DataGridHeadersVisibility.Column,
                GridLinesVisibility = DataGridGridLinesVisibility.All,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            // Adiciona as colunas ao DataGrid
            DataGridTextColumn addressColumn = new DataGridTextColumn
            {
                Header = "Endereço",
                Binding = new System.Windows.Data.Binding("Address"),
                Width = 80
            };

            DataGridTextColumn hexValueColumn = new DataGridTextColumn
            {
                Header = "Valor (Hex)",
                Binding = new System.Windows.Data.Binding("HexValue"),
                Width = 100
            };

            DataGridTextColumn decValueColumn = new DataGridTextColumn
            {
                Header = "Valor (Dec)",
                Binding = new System.Windows.Data.Binding("DecValue"),
                Width = 100
            };

            _memoryGrid.Columns.Add(addressColumn);
            _memoryGrid.Columns.Add(hexValueColumn);
            _memoryGrid.Columns.Add(decValueColumn);

            // Adiciona o DataGrid ao GroupBox
            _memoryGroupBox.Content = _memoryGrid;

            // Adiciona o GroupBox ao Grid principal
            _mainGrid.Children.Add(_memoryGroupBox);

            // Posiciona o GroupBox no Grid
            Grid.SetRow(_memoryGroupBox, 1);
            Grid.SetColumn(_memoryGroupBox, 0);
            Grid.SetRowSpan(_memoryGroupBox, 2);
        }

        // Atualiza a visualização da memória com o estado atual
        public void Update(SimulationState state)
        {
            // Limpa o DataGrid
            _memoryGrid.Items.Clear();

            // Adiciona os valores da memória ao DataGrid
            for (uint i = 0; i < state.Memory.Length; i += 4)
            {
                // Exibe apenas se houver algum valor diferente de zero
                if (state.GetWord(i) != 0)
                {
                    _memoryGrid.Items.Add(new MemoryItem
                    {
                        Address = $"0x{i:X8}",
                        HexValue = $"0x{state.GetWord(i):X8}",
                        DecValue = state.GetWord(i).ToString()
                    });
                }
            }
        }

        // Classe para representar um item da memória no DataGrid
        private class MemoryItem
        {
            public string Address { get; set; }
            public string HexValue { get; set; }
            public string DecValue { get; set; }
        }
    }
}