using Memory;
using System.Threading;
using System.Runtime.InteropServices;
using ezOverLay;

namespace eltrainer
{
    public partial class Form1 : Form
    {

        ez ez = new ez();

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
                ez.SetInvi(this);
                ez.DoStuff("AssaultCube", this);

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

                Form1 f = this;
                f.Refresh();

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

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen red = new Pen(Color.Red, 3);
            Pen green = new Pen(Color.Green, 3);

            foreach(var ent in entities.ToList())
            {
                var wtsFeet = m.WorldToScreen(m.ReadMatrix(), ent.feet, this.Width, this.Height);
                var wtsHead = m.WorldToScreen(m.ReadMatrix(), ent.head, this.Width, this.Height);

                if (wtsFeet.X > 0)
                {
                    if (localPlayer.team == ent.team)
                    {
                        g.DrawLine(green, new Point(Width / 2, Height), wtsFeet);
                        g.DrawRectangle(green, m.CalcRect(wtsFeet, wtsHead));
                    }

                    else
                    {
                        g.DrawLine(red, new Point(Width / 2, Height), wtsFeet);
                        g.DrawRectangle(red, m.CalcRect(wtsFeet, wtsHead));

                    }
                }
            }
        }
    }
}