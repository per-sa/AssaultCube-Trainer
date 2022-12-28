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

        public Rectangle CalcRect(Point feet, Point head)
        {
            var rect = new Rectangle();

            rect.X = head.X - (feet.Y - head.Y) / 4;
            rect.Y = head.Y;

            rect.Width = (feet.Y - head.Y) / 2;
            rect.Height = feet.Y - head.Y;

            return rect;

        }

        public Point WorldToScreen(ViewMatrix mtx, Vector3 pos, int width, int height)
        {

            var twoD = new Point();

            float screenW = (mtx.m14 * pos.X) + (mtx.m24 * pos.Y) + (mtx.m34* pos.Z) + mtx.m44;

            if (screenW > 0.001f)
            {
                float screenX = (mtx.m11 * pos.X) + (mtx.m21 * pos.Y) + (mtx.m31 * pos.Z) + mtx.m41;

                float screenY = (mtx.m12 * pos.X) + (mtx.m22 * pos.Y) + (mtx.m32 * pos.Z) + mtx.m42;


                float camX = width / 2f;
                float camY = height / 2f;

                float X = camX + (camX * screenX / screenW);
                float Y = camY - (camY * screenY / screenW);

                twoD.X = (int)X;
                twoD.Y = (int)Y;

                return twoD;

            }

            else
            {
                return new Point(-99, -99);
            }



        }


        public ViewMatrix ReadMatrix()
        {
            var viewMatrix = new ViewMatrix();
            var mtx = mem.ReadMatrix(moduleBase + Offsets.iViewMatrix);

            viewMatrix.m11 = mtx[0];
            viewMatrix.m12 = mtx[1];
            viewMatrix.m13 = mtx[2];
            viewMatrix.m14 = mtx[3];

            viewMatrix.m21 = mtx[4];
            viewMatrix.m22 = mtx[5];
            viewMatrix.m23 = mtx[6];
            viewMatrix.m24 = mtx[7];

            viewMatrix.m31 = mtx[8];
            viewMatrix.m32 = mtx[9];
            viewMatrix.m33 = mtx[10];
            viewMatrix.m34 = mtx[11];

            viewMatrix.m41 = mtx[12];
            viewMatrix.m42 = mtx[13];
            viewMatrix.m43 = mtx[14];
            viewMatrix.m44 = mtx[15];

            return viewMatrix; 
        }


        public methods()
        {
            mem = new swed();
            mem.GetProcess("ac_client");
            moduleBase = mem.GetModuleBase(".exe");

        }
    }
}
