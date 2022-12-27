using Memory;

namespace eltrainer
{
    public partial class Form1 : Form
    {
        Mem meme = new Mem();

        string ammoAddress = "ac_client.exe+0x00195404,140";

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

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                meme.WriteMemory(ammoAddress, "int", "9999");
            }
        }

    }
}