using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ASNA.QSys.Expo.Model;

// Migrated on 9/28/2021 at 10:28 AM by ASNA Monarch(R) version 10.0.48.0
// Legacy location: library ERCAP, file QDDSSRC, member ORDHDSPF


namespace SunFarm.CustomerApp.CustomerAppViews
{
    [
        BindProperties,
        DisplayPage(FunctionKeys = "F12 12"),
        ExportSource(CCSID = 37)
    ]
    public class ORDHDSPF : DisplayPageModel
    {
        public MYWINDOW_Model MYWINDOW { get; set; }
        public ORDHREC_Model ORDHREC { get; set; }
        public SFLC_Model SFLC { get; set; }
        public KEYS_Model KEYS { get; set; }
        public MSGSFC_Model MSGSFC { get; set; }

        public ORDHDSPF()
        {
            MYWINDOW = new MYWINDOW_Model();
            ORDHREC = new ORDHREC_Model();
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
            Record(FunctionKeys = "F4 04;F11 11:!30")
        ]
        public class ORDHREC_Model : RecordModel
        {
            [Char(10)]
            private string CSRREC
            {
                get => CursorLocationFormatName;
                set { }
            }

            [Char(10)]
            private string CSRFLD
            {
                get => CursorLocationFieldName;
                set { }
            }

            [Dec(9, 0)]
            public decimal DTORDNUM { get; private set; }

            [Date]
            public DateTime DTORDDATE { get; set; } // ORDER DATE

            [Date]
            public DateTime DTSHPDATE { get; set; } // PRICE

            [Dec(8, 0)]
            public decimal DTDELDATE { get; set; }

            [Dec(3, 0)]
            public decimal DTSHPVIA { get; set; }

            [Char(18)]
            public string DTWEIGHT { get; private set; } // TOTAL ORDER WEIGHT

        }

        [
            SubfileControl(ClearRecords : "90",
                FunctionKeys = "F6 06;PageUp 51:!76;PageDown 50:!77",
                DisplayFields = "!90",
                DisplayRecords = "!90",
                Size = 11
            )
        ]
        public class SFLC_Model : SubfileControlModel
        {
            public List<SFL1_Model> SFL1 { get; set; } = new List<SFL1_Model>();

            [Char(50)]
            public string SCRCUST { get; private set; } // CUSTOMER NBR AND NAME

            [Char(20)]
            public string SCRPHONE { get; private set; }

            [Char(35)]
            public string CMADDR1 { get; private set; }

            [Char(14)]
            public string SCRFAX { get; private set; } // FAX NUMBER

            [Char(35)]
            public string CMADDR2 { get; private set; }

            [Char(1)]
            public string CMACTIVE { get; private set; }

            [Char(30)]
            public string CMCITY { get; private set; }

            [Char(2)]
            public string CMSTATE { get; private set; }

            [Char(10)]
            public string CMPOSTCODE { get; private set; }

            [Dec(9, 0)]
            public decimal SETORDNUM { get; set; }

            public class SFL1_Model : SubfileRecordModel
            {
                [Values(typeof(Decimal),"00","02","03","06")]
                [Dec(2, 0, Protect = "60")]
                public decimal SFSEL { get; set; }

                [Char(1, Protect = "*True")]
                public string SFCOLOR { get; set; }

                [Dec(9, 0)]
                public decimal SFORDNUM { get; private set; }

                [Date]
                public DateTime SFORDDATE { get; private set; } // ORDER DATE

                [Date]
                public DateTime SFSHPDATE { get; private set; } // PRICE

                [Dec(8, 0)]
                public decimal SFDELDATE { get; private set; }

                [Dec(13, 4)]
                public decimal SFORDAMT { get; private set; }

                [Char(4)]
                public string SFFILESTAT { get; private set; }

                [Dec(3, 0)]
                public decimal SFSHPVIA { get; private set; }

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
