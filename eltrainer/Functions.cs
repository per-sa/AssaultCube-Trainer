using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using swed32;

namespace eltrainer
{
    public class methods
    {
        public swed mem;
        public IntPtr moduleBase;

        public Entity ReadLocalPLayer()
        {
            var localPlayer = ReadEntity(mem.ReadPointer(moduleBase, Offsets.iLocalPlayer));
            localPlayer.viewAngles.X = mem.ReadFloat(localPlayer.baseAddress, Offsets.vAngles);
            localPlayer.viewAngles.Y = mem.ReadFloat(localPlayer.baseAddress, Offsets.vAngles + 0x4);
            return localPlayer;
        }

        public Entity ReadEntity(IntPtr entBase)
        {
            var ent = new Entity();

            ent.baseAddress = entBase;

            ent.CurrentRifleAmmo = mem.ReadInt(ent.baseAddress, Offsets.iCurrentRifleAmmo);
            ent.health = mem.ReadInt(ent.baseAddress, Offsets.iHealth);
            ent.team = mem.ReadInt(ent.baseAddress, Offsets.iTeam);

            ent.feet = mem.ReadVector3(entBase, Offsets.vFeet);
            ent.head = mem.ReadVector3(ent.baseAddress, Offsets.vHead);

            ent.name = Encoding.UTF8.GetString(mem.ReadBytes(ent.baseAddress, Offsets.sName, 11));

            return ent;
        }


        public methods()
        {
            mem = new swed();
            mem.GetProcess("ac_client");
            moduleBase = mem.GetModuleBase(".exe");

        }
    }
}
