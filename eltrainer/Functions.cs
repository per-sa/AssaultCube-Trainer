using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

        public List<Entity> ReadEntities(Entity localPlayer)
        {
            var entities = new List<Entity>();
            var entityList = mem.ReadPointer(moduleBase, Offsets.iEntityList);

            for (int i = 0; i < 4; i++)
            {
                var currentEntBase = mem.ReadPointer(entityList, i * 0x4);
                var ent = ReadEntity(currentEntBase);
                ent.mag = CalcMag(localPlayer, ent);

                if (ent.health > 0 && ent.health < 101)
                {
                    entities.Add(ent);
                }

            }

            return entities;

        }

        public Vector2 CalcAngles(Entity localPlayer, Entity destEnt)
        {
            float x, y;

            var deltaX = destEnt.head.X - localPlayer.head.X;
            var deltaY = destEnt.head.Y - localPlayer.head.Y;

            x = (float)(Math.Atan2(deltaY, deltaX) * 180 / Math.PI) + 90;

            float deltaZ = destEnt.head.Z - localPlayer.head.Z;
            float dist = CalcDist(localPlayer, destEnt);

            y = (float)(Math.Atan2(deltaZ, dist) * 180 / Math.PI);
            return new Vector2(x, y);

        }

        public void Aim(Entity ent, float x, float y)
        {
            mem.WriteFloat(ent.baseAddress, Offsets.vAngles, x);
            mem.WriteFloat(ent.baseAddress, Offsets.vAngles + 0x4, y);
        }


        public static float CalcDist(Entity localPlayer, Entity destEnt)
        {
            return (float)
                Math.Sqrt(Math.Pow(destEnt.feet.X - localPlayer.feet.X, 2)
                + Math.Pow(destEnt.feet.Y - localPlayer.feet.Y, 2));
        }

        public static float CalcMag(Entity localPlayer, Entity destEnt)
        {
            return (float)
                Math.Sqrt(Math.Pow(destEnt.feet.X - localPlayer.feet.X, 2)
                + Math.Pow(destEnt.feet.Y - localPlayer.feet.Y, 2)
                + Math.Pow(destEnt.feet.Z - localPlayer.feet.Z, 2)
                );
        }


        public methods()
        {
            mem = new swed();
            mem.GetProcess("ac_client");
            moduleBase = mem.GetModuleBase(".exe");

        }
    }
}
