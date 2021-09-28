using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ASNA.QSys.Expo.Model;

// Migrated on 9/28/2021 at 10:28 AM by ASNA Monarch(R) version 10.0.48.0
// Legacy location: library ERCAP, file QDDSSRC, member ORDDTLINQD


namespace YourCompany.YourApplication.CustomerAppViews
{
    [
        BindProperties,
        DisplayPage(FunctionKeys = "F12 12"),
        ExportSource(CCSID = 37)
    ]
    public class ORDDTLINQD : DisplayPageModel
    {
        public MYWINDOW_Model MYWINDOW { get; set; }
        public ORDLINE_Model ORDLINE { get; set; }
        public SFLC_Model SFLC { get; set; }
        public KEYS_Model KEYS { get; set; }
        public MSGSFC_Model MSGSFC { get; set; }

        public ORDDTLINQD()
        {
            MYWINDOW = new MYWINDOW_Model();
            ORDLINE = new ORDLINE_Model();
            SFLC = new SFLC_Model();
            KEYS = new KEYS_Model();
            MSGSFC = new MSGSFC_Model();
        }

        [
            Record(EraseFormats = "SFL1 , SFLC , KEYS , MSGSF , MSGSFC")
        ]
        public class MYWINDOW_Model : RecordModel
        {
            [Char(22)]
            public string WINTITLE { get; private set; }

        }

        [
            Record(FunctionKeys = "F4 04:!88")
        ]
        public class ORDLINE_Model : RecordModel
        {
            [Char(1, Protect = "!89")]
            public string SDUMMY { get; set; }

            [Dec(3, 0)]
            public decimal SLINNBR { get; private set; }

            [Dec(9, 0, Protect = "88")]
            public decimal SITMNBR { get; set; }

            [Dec(9, 4, Protect = "89")]
            public decimal SQTYORD { get; set; }

        }

        [
            SubfileControl(ClearRecords : "90",
                FunctionKeys = "F6 06",
                DisplayFields = "!90",
                DisplayRecords = "!90",
                Size = 100
            )
        ]
        public class SFLC_Model : SubfileControlModel
        {
            public List<SFL1_Model> SFL1 { get; set; } = new List<SFL1_Model>();

            [Char(69)]
            public string SCRCUST { get; private set; } // CUST NBR, NAME AND CITY

            [Dec(9, 0)]
            public decimal SORDNUM { get; private set; } // ORDER NUMBER

            [Date(DateFormat = DateAttribute.DdsDateFormat.USA)]
            public DateTime SORDDATE { get; private set; } // ORDER DATE

            [Char(15)]
            public string SORDAMOUNT { get; private set; }

            [Char(15)]
            public string SORDWEIGHT { get; private set; }

            public class SFL1_Model : SubfileRecordModel
            {
                [Values(typeof(Decimal),"0","2","4")]
                [Dec(1, 0, Protect = "60")]
                public decimal SFSEL { get; set; }

                [Dec(3, 0)]
                public decimal SFLINNBR { get; private set; } // LINE NUMBER

                [Dec(9, 0)]
                public decimal SFITMNBR { get; private set; } // ITEM NBR

                [Char(20)]
                public string SFITMDESC { get; private set; } // ITEM DESC

                [Dec(9, 4)]
                public decimal SFPRICE { get; private set; } // PRICE

                [Dec(10, 4)]
                public decimal SFQTYORD { get; private set; } // ORDER QTY

                [Dec(10, 4)]
                public decimal SFEXTAMT { get; private set; }

            }

        }

        public class KEYS_Model : RecordModel
        {
        }

        [
            SubfileControl(ClearRecords : "",
                ProgramQ = "@PGMQ",
                DisplayFields = "*True",
                DisplayRecords = "*True",
                InitializeRecords = "*True",
                Size = 10
            )
        ]
        public class MSGSFC_Model : SubfileControlModel
        {
            public List<MSGSF_Model> MSGSF { get; set; } = new List<MSGSF_Model>();

            [Char(10, Alias = "@PGMQ")]
            public string aPGMQ { get; private set; }

            [
                SubfileRecord(IsMessageSubfile = true)
            ]
            public class MSGSF_Model : SubfileRecordModel
            {
                [Char(4, Alias = "@MSGKY")]
                public string aMSGKY { get; private set; }

                [Char(10, Alias = "@PGMQ")]
                public string aPGMQ { get; private set; }

                [Char(128)]
                public string MessageText { get; private set; }

            }

        }

    }
}
