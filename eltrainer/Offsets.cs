using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eltrainer
{
    public class Offsets
    {
        public static int
            iViewMatrix = 0x17DFFC,
            iLocalPlayer = 0x0018AC00,
            iEntityList = 0x00191FCC,

            // offsets from entities

            vHead = 0x4,
            vFeet = 0x30,
            vAngles = 0x34,
            iHealth = 0xEC,
            iDead = 0xB4,
            sName = 0x205,
            iTeam = 0x30C,
            iCurrentAmmo
            ;
    }
}
