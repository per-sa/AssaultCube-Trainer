using Memory;
using System.Threading;
using System.Runtime.InteropServices;

namespace eltrainer
{
    public partial class Form1 : Form
    {
        Mem meme = new Mem();

        string ammoAddress = "ac_client.exe+0x00195404,140";
        string pistolAmmoAddress = "ac_client.exe+0x0018AC00, 12C";
        string grenadeAddress = "ac_client.exe+0x0018AC00, 144";

        methods? m;
        Entity localPlayer = new Entity();
        List<Entity> entities = new List<Entity>();

        [DllImport("user32.dll")]

        static extern short GetAsyncKeyState(Keys vKey);

        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //int PID = meme.GetProcIdFromName("ac_client.exe");
            //if (PID > 0)
            //{
            //    meme.OpenProcess(PID);
            //    timer1.Start();
            //}

            CheckForIllegalCrossThreadCalls = false;
            m = new methods();

            if (m != null)
            {
                Thread thread = new Thread(Main) { IsBackground = true };
                thread.Start();
            }



            int i = 0;
        }

        void Main()
        {
            while (true)
            {
                localPlayer = m.ReadLocalPLayer();
                entities = m.ReadEntities(localPlayer);

                entities = entities.OrderBy(o => o.mag).ToList();

                if (GetAsyncKeyState(Keys.XButton1)<0)
                {
                    if (entities.Count > 0)
                    {
                        foreach(var ent in entities)
                        {
                            if (ent.team != localPlayer.team)
                            {
                                var angles = m.CalcAngles(localPlayer, ent);
                                m.Aim(localPlayer, angles.X, angles.Y);
                                break;
                            }
                        }
                    } 
                }

                Thread.Sleep(20);
            }

        }

        //protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        //{

            //if (keyData == Keys.F1)
            //{
            //    checkBox1.Checked = !checkBox1.Checked;
            //}

            //if (keyData == Keys.F2)
            //{
            //    checkBox2.Checked = !checkBox2.Checked;
            //}

            //return base.ProcessCmdKey(ref msg, keyData);
        //}

        private void timer1_Tick(object sender, EventArgs e)
        {
        //    if (checkBox1.Checked)
        //    {
        //        meme.WriteMemory(ammoAddress, "int", "9999");
        //        meme.WriteMemory(pistolAmmoAddress, "int", "9999");

        //    }

        //    if (checkBox2.Checked)
        //    {
        //        meme.WriteMemory(grenadeAddress, "int", "9999");
        //    }
        }

    }
}