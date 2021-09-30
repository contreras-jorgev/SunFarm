using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ASNA.QSys.Expo.Model;

// Migrated on 9/28/2021 at 10:28 AM by ASNA Monarch(R) version 10.0.48.0
// Legacy location: library ERCAP, file QDDSSRC, member ITEMDSPF


namespace SunFarm.CustomerApp.CustomerAppViews
{
    [
        BindProperties,
        DisplayPage(FunctionKeys = "F3 03"),
        ExportSource(CCSID = 37)
    ]
    public class ITEMDSPF : DisplayPageModel
    {
        public SFLC_Model SFLC { get; set; }
        public ITEMREC_Model ITEMREC { get; set; }
        public KEYS_Model KEYS { get; set; }
        public MSGSFC_Model MSGSFC { get; set; }

        public ITEMDSPF()
        {
            SFLC = new SFLC_Model();
            ITEMREC = new ITEMREC_Model();
            KEYS = new KEYS_Model();
            MSGSFC = new MSGSFC_Model();
        }

        [
            SubfileControl(ClearRecords : "90",
                FunctionKeys = "PageUp 51:!76;PageDown 50:!77",
                DisplayFields = "!90",
                DisplayRecords = "!90",
                Size = 14,
                IsExpandable = false,
                EraseFormats = "ITEMREC"
            )
        ]
        public class SFLC_Model : SubfileControlModel
        {
            public List<SFL1_Model> SFL1 { get; set; } = new List<SFL1_Model>();

            [Char(20, OutputData=false)]
            public string SETITMDSC { get; set; }

            public class SFL1_Model : SubfileRecordModel
            {
                [Char(1, Protect = "*True")]
                public string SFCOLOR { get; set; }

                [Values(typeof(Decimal),"00","01")]
                [Dec(2, 0, Protect = "60")]
                public decimal SFSEL { get; set; }

                [Dec(9, 0)]
                public decimal SFITEMNUM { get; private set; }

                [Char(20)]
                public string SFITEMDESC { get; private set; }

                [Dec(9, 4)]
                public decimal SFITMPRIC { get; private set; }

                [Dec(12, 4)]
                public decimal SFITMAVAIL { get; private set; }

            }

        }

        [
            Record(FunctionKeys = "F4 04;F6 06:!30;F11 11:!30;F12 12",
                EraseFormats = "SFLC KEYS"
            )
        ]
        public class ITEMREC_Model : RecordModel
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

            [Char(10)]
            public string SCPGM { get; private set; }

            [Char(20)]
            public string SFOLDDESC { get; private set; }

            [Dec(9, 0)]
            public decimal SFITEM { get; private set; }

            [Char(50)]
            public string SFLNGDESC { get; set; }

            [Char(20)]
            public string SFDESC { get; set; }

            [Dec(9, 4)]
            public decimal SFPRICE { get; set; }

            [Char(10)]
            public string SFUNIT { get; set; }

            [Dec(7, 4)]
            public decimal SFWEIGHT { get; set; } // WEIGHT

            [Dec(12, 4)]
            public decimal SFAVAIL { get; set; }

            [Dec(12, 4)]
            public decimal SFONORDR { get; set; }

        }

        [
            Record(EraseFormats = "ITEMREC")
        ]
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
