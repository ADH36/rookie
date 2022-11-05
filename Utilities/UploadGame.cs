﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidSideloader.Utilities
{
    class UploadGame
    {
        public UploadGame(string Uploadcommand, string Pckgcommand, string Uploadgamename, ulong Uploadversion, bool isUpdate)
        {
            this.Pckgcommand = Pckgcommand;
            this.Uploadgamename = Uploadgamename;
            this.Uploadversion = Uploadversion;
            this.isUpdate = isUpdate;
        }
        public UploadGame()
        {
            
        }
        public bool isUpdate { get; set; }

        public string Pckgcommand { get; set; }
        
        public string Uploadgamename { get; set; }

        public ulong Uploadversion { get; set; }

    }
}
