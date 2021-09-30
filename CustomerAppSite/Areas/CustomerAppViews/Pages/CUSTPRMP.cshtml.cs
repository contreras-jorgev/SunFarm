using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ASNA.QSys.Expo.Model;

// Migrated on 9/28/2021 at 10:28 AM by ASNA Monarch(R) version 10.0.48.0
// Legacy location: library ERCAP, file QDDSSRC, member CUSTPRMP


namespace SunFarm.CustomerApp.CustomerAppViews
{
    [
        BindProperties,
        DisplayPage(FunctionKeys = "F12 12"),
        ExportSource(CCSID = 37)
    ]
    public class CUSTPRMP : DisplayPageModel
    {
        public MYWINDOW_Model MYWINDOW { get; set; }
        public SFLC_Model SFLC { get; set; }
        public DUMMY_Model DUMMY { get; set; }

        public CUSTPRMP()
        {
            MYWINDOW = new MYWINDOW_Model();
            SFLC = new SFLC_Model();
            DUMMY = new DUMMY_Model();
        }

        [
            Record(EraseFormats = "DUMMY")
        ]
        public class MYWINDOW_Model : RecordModel
        {
            [Char(20)]
            public string WINTITLE { get; private set; }

        }

        [
            SubfileControl(ClearRecords : "90",
                DisplayFields = "!90",
                DisplayRecords = "!90",
                Size = 80
            )
        ]
        public class SFLC_Model : SubfileControlModel
        {
            public List<SFL1_Model> SFL1 { get; set; } = new List<SFL1_Model>();

            [Dec(4, 0)]
            public decimal SFLRRN { get; private set; }

            public class SFL1_Model : SubfileRecordModel
            {
                [Dec(1, 0)]
                public decimal SFLSEL { get; set; }

                [Char(5)]
                public string SFLVALUE { get; private set; }

                [Char(32)]
                public string SFLDESC { get; private set; }

            }

        }

        [
            Record(EraseFormats = "*ALL")
        ]
        public class DUMMY_Model : RecordModel
        {
        }

    }
}
