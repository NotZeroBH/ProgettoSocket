using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Net;
using System.Net.Sockets;
//Abbiamo aggiunto queste 3 librerie per poter lavorare coi socket
namespace socket
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnCreaSocket_Click(object sender, RoutedEventArgs e)
        {
            IPEndPoint sourceSocket = new IPEndPoint(IPAddress.Parse("10.73.0.8"), 56000);

            
            btnInvia.IsEnabled = true;

            Thread ricezione = new Thread(new ParameterizedThreadStart(SocketReceive));

            ricezione.Start(sourceSocket);
        } //In questo metodo lavoriamo sull'instaurazione della comunicazione

        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {
            //aggiungere controlli sul contenuto delle textbox
            string ipAddress = txtIP.Text;
            int port = int.Parse(txtPort.Text);

            SocketSend(IPAddress.Parse(ipAddress), port, txtMsg.Text);
        }
        public async void SocketReceive(object socksource)
        {
            IPEndPoint ipendp = (IPEndPoint)socksource;

            Socket t = new Socket(ipendp.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            t.Bind(ipendp);

            Byte[] bytesRicevuti = new Byte[256];

            string message;

            int ContaCaratteri = 0;

            await Task.Run(() =>
            {
                while (true)
                {
                    if(t.Available > 0)
                    {
                        message = "";

                        ContaCaratteri = t.Receive(bytesRicevuti, bytesRicevuti.Length, 0);
                        message = message + Encoding.ASCII.GetString(bytesRicevuti, 0, ContaCaratteri);

                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            lblRicevi.Content = message;
                        }));
                    }
                }
            });
        } //In questo metodo andiamo a lavorare sulla recizione del messaggio 

        public void SocketSend(IPAddress dest, int destport, string message)
        {
            Byte[] byteInviati = Encoding.ASCII.GetBytes(message);

            Socket s = new Socket(dest.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            IPEndPoint remote_endpoint = new IPEndPoint(dest, destport);

            s.SendTo(byteInviati, remote_endpoint);

        }//In questo metodo andiamo a lavorare sull'invio del messaggio 
    }
}
