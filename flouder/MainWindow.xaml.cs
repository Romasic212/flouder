using System.Linq;
using System.Windows;
using Microsoft.Win32;

namespace flouder
{
    public partial class MainWindow : Window
    {
        private Indexer _indexer = new Indexer();
        private FolderWatcher _watcher = new FolderWatcher();
        private TcpService _tcp = new TcpService();

        public MainWindow()
        {
            InitializeComponent();

            _watcher.OnChanged += Watcher_OnChanged;

            _tcp.OnMessageReceived += msg =>
            {
                Dispatcher.Invoke(() =>
                {
                    FilesList.Items.Add(msg);
                });
            };

            _tcp.StartServer();
        }

        private void BtnSelectFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                CheckFileExists = false,
                ValidateNames = false,
                FileName = "Выберите папку"
            };

            if (dialog.ShowDialog() == true)
            {
                var folderPath = System.IO.Path.GetDirectoryName(dialog.FileName);

                TxtFolder.Text = folderPath;

                var files = _indexer.Scan(folderPath);

                FilesList.ItemsSource = files.Select(f => f.Path).ToList();

                _watcher.Start(folderPath);
            }
        }

        private async void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            await _tcp.Connect(TxtIP.Text);
        }

        private void Watcher_OnChanged(string message)
        {
            Dispatcher.Invoke(() =>
            {
                FilesList.Items.Add(message);
            });
        }
    }
}