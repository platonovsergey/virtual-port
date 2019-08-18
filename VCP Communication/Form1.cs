using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VCP_Communication
{
    public partial class VCP_Communication : Form
    {
        private static SerialDataReceivedEventHandler serialDataReceivedEventHandler;
        static bool btnIsClicked;
        int i = 0;
        Vcp vcp;

        public VCP_Communication()
        {
            InitializeComponent();
            vcp = new Vcp();
        }

        public void PortChat_Init()
        {
            _serialPort.PortName = cbPort.Text;
            _serialPort.BaudRate = int.Parse(cbBoudRate.Text);
            _serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), cbParity.Text);
            _serialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), cbStopBits.Text);
        }

        public static void SetTextUI(RichTextBox rtb, string data)
        {
            if (rtb.InvokeRequired)
            {
                Action putInText = () =>
                {
                    rtb.AppendText(data);
                    rtb.SelectionStart = rtb.Text.Length;
                };

                rtb.Invoke(putInText);
            }
            else
            {
                rtb.AppendText(data);
                rtb.SelectionStart = rtb.Text.Length;
            }
        }

        private void PortChat_Listen(object sender, SerialDataReceivedEventArgs e)
        {
            string data;
            string temp = _serialPort.ReadExisting();
            Console.WriteLine("start");
            Console.WriteLine(temp);
            Console.WriteLine("end");
            data = "Received: " + temp;

            SetTextUI(console, data);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnIsClicked = false;

            cbPort.Items.AddRange(vcp.portsName);

            foreach (int item in vcp.boudRate)
            {
                cbBoudRate.Items.Add(item);
            }

            foreach (KeyValuePair<Parity, String> item in vcp.parity)
            {
                cbParity.Items.Add(item.Key);
            }

            foreach (KeyValuePair<StopBits, string> item in vcp.stopBits)
            {
                cbStopBits.Items.Add(item.Key);
            }

            cbPort.SelectedIndex = cbPort.Items.Count - 1;
            cbBoudRate.SelectedIndex = 4;
            cbParity.SelectedIndex = 0; 
            cbStopBits.SelectedIndex = 1;

            PortChat_Init();
        }

        private void BtnConnection_Click(object sender, EventArgs e)
        {
            if (btnIsClicked == false)
            {
                _serialPort.Open();
                serialDataReceivedEventHandler = new SerialDataReceivedEventHandler(PortChat_Listen);
                _serialPort.DataReceived += serialDataReceivedEventHandler;
                btnIsClicked = true;
                console.AppendText("Connected to " + _serialPort.PortName + ", " + _serialPort.BaudRate + ", " + _serialPort.Parity + ", " + _serialPort.StopBits + "\n");
                btnConnection.Text = "Disconnect";
            }
            else
            {
                _serialPort.Close();
                _serialPort.DataReceived -= serialDataReceivedEventHandler;
                serialDataReceivedEventHandler = null;
                btnIsClicked = false;
                console.AppendText("Disconnected to " + _serialPort.PortName + "\n");
                btnConnection.Text = "Connect";
            }
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (_serialPort.IsOpen)
                {
                    //_serialPort.WriteLine(console.Text + Environment.NewLine);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            if (_serialPort.IsOpen)
            {
                console.Text = "Connected to " + _serialPort.PortName + ", " + _serialPort.BaudRate + ", " + _serialPort.Parity + ", " + _serialPort.StopBits + Environment.NewLine;
            }
            else
            {
                console.Text = "";
            }
        }
    }

    public class Vcp
    {
        public string[] portsName = SerialPort.GetPortNames();
        public List<int> boudRate = new List<int>();
        public Dictionary<StopBits, String> stopBits = new Dictionary<StopBits, string>();
        public Dictionary<Parity, String> parity = new Dictionary<Parity, string>();

        public Vcp()
        {
            boudRate.Add(600);
            boudRate.Add(1200);
            boudRate.Add(2400);
            boudRate.Add(4800);
            boudRate.Add(9600);
            boudRate.Add(14400);
            boudRate.Add(19200);
            boudRate.Add(28800);
            parity.Add(Parity.None, "None (Нет)");
            parity.Add(Parity.Odd, "Odd (Нечётность)");
            parity.Add(Parity.Even, "Even (Нечётность)");
            parity.Add(Parity.Mark, "Mark (Единица)");
            parity.Add(Parity.Space, "Space (Ноль)");
            stopBits.Add(StopBits.None, "None (0)");
            stopBits.Add(StopBits.One, "One (1)");
            stopBits.Add(StopBits.Two, "Two (2)");
            stopBits.Add(StopBits.OnePointFive, "OnePointFive (1,5)");
        }
    }
}
