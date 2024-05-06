using Fleck;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace projekt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<string> _messages;
        private WebSocketServer server;
        private List<IWebSocketConnection> allSockets;

        public MainWindow()
        {
            InitializeComponent();
            _messages = new ObservableCollection<string>();
            DataContext = _messages;
            StartServer();
        }


        private void StartServer()
        {
            allSockets = new List<IWebSocketConnection>();
            server = new WebSocketServer("ws://0.0.0.0:8181");
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Dispatcher.Invoke(() => _messages.Add("Client connected"));
                    allSockets.Add(socket);
                };
                socket.OnClose = () =>
                {
                    Dispatcher.Invoke(() => _messages.Add("Client disconnected"));
                    allSockets.Remove(socket);
                };
                socket.OnMessage = message =>
                {
                    Dispatcher.Invoke(() => _messages.Add(message));
                    foreach (var s in allSockets)
                    {
                        s.Send(message);
                    }
                };
            });
        }
    }
}