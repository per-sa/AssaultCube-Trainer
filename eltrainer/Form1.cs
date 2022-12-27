using Memory;

namespace eltrainer
{
    public partial class Form1 : Form
    {
        Mem meme = new Mem();

        string ammoAddress = "ac_client.exe+0x00195404,140";
        string pistolAmmoAddress = "ac_client.exe+0x0018AC00, 12C";
        string grenadeAddress = "ac_client.exe+0x0018AC00, 144";

        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int PID = meme.GetProcIdFromName("ac_client.exe");
            if (PID > 0)
            {
                meme.OpenProcess(PID);
                timer1.Start();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {

            if (keyData == Keys.F1)
            {
                checkBox1.Checked = !checkBox1.Checked;
            }

            if (keyData == Keys.F2)
            {
                checkBox2.Checked = !checkBox2.Checked;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                meme.WriteMemory(ammoAddress, "int", "9999");
                meme.WriteMemory(pistolAmmoAddress, "int", "9999");

            }

            if (checkBox2.Checked)
            {
                meme.WriteMemory(grenadeAddress, "int", "9999");
            }
        }

    }
}