using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ASNA.QSys.Expo.Model;

// Migrated on 9/28/2021 at 10:28 AM by ASNA Monarch(R) version 10.0.48.0
// Legacy location: library ERCAP, file QDDSSRC, member CUSTDELIV


namespace SunFarm.CustomerApp.CustomerAppViews
{
    [
        BindProperties,
        DisplayPage(FunctionKeys = "F12 12"),
        ExportSource(CCSID = 37)
    ]
    public class CUSTDELIV : DisplayPageModel
    {
        public SFLC_Model SFLC { get; set; }
        public KEYS_Model KEYS { get; set; }

        public CUSTDELIV()
        {
            SFLC = new SFLC_Model();
            KEYS = new KEYS_Model();
        }

        [
            SubfileControl(ClearRecords : "90",
                FunctionKeys = "PageDown 50:!99",
                DisplayFields = "!90",
                DisplayRecords = "!90",
                Size = 20
            )
        ]
        public class SFLC_Model : SubfileControlModel
        {
            public List<SFL1_Model> SFL1 { get; set; } = new List<SFL1_Model>();

            public class SFL1_Model : SubfileRecordModel
            {
                [Char(1, Protect = "60")]
                public string SFLSEL { get; set; }

                [Dec(9, 0, Alias = "SFLCUST#")]
                public decimal SFLCUSTh { get; private set; }

                [Char(26)]
                public string SFLCUST { get; private set; }

                [Char(25)]
                public string SFLCITY { get; private set; }

                [Char(5)]
                public string SFLZIP { get; private set; }

            }

        }

        public class KEYS_Model : RecordModel
        {
        }

    }
}
