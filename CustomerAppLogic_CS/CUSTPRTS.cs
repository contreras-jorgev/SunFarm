// Translated from AVR to C# on 9/28/2021 at 10:33:56 AM by ASNA Monarch Nomad® version 16.0.24.0
// ASNA Monarch(R) version 10.0.48.0 at 9/28/2021
// Migrated source location: library ERCAP, file QRPGSRC, member CUSTPRTS

using ASNA.QSys.Runtime;
using ASNA.DataGate.Common;
using SunFarm.CustomerApp_Job;

using System;
using ASNA.QSys.Runtime.JobSupport;


namespace YourCompany.YourApplication
{
    [ActivationGroup("*DFTACTGRP")]
    [ProgramEntry("_ENTRY")]
    public partial class CUSTPRTS : Program
    {

        DatabaseFile CSMASTERL1;
        DatabaseFile CUSTOMERL1;
        PrintFile QPRINT;
        internal const decimal QPRINT_PrintLineHeight = 50; // Notes: Units are LOMETRIC (one hundredth of a centimeter). The constant used came from the Global Directive defaults.
        //********************************************************************
        //   U1 Print sales
        //   U2 Print credits
        //********************************************************************
        DataStructure pNumberAlf = new (9);
        FixedDecimal<_9, _0> pNumber { get => pNumberAlf.GetZoned(0, 9, 0); set => pNumberAlf.SetZoned(value, 0, 9, 0); } 

        class pNumberAlf_Class : LocalScopeDS
        {
            internal FixedDecimal<_9, _0> pNumber { get => DS.GetZoned(0, 9, 0); set => DS.SetZoned(value, 0, 9, 0); } 
            internal pNumberAlf_Class() : base(9){}
        }

        FixedDecimal<_7, _0> wCount;
        FixedDecimal<_4, _0> wPrevYr;
        FixedDecimal<_4, _0> wPrtYr;
        FixedString<Len<_1, _2, _0>> wUnderline;
        short X;
        DataStructure CUSTSL;
        FixedDecimalArrayInDS<_12, _11, _2> SlsArray; 

#region Program Status Data Structure
        DataStructure _DS3 = new (10);
        FixedString<_10> sUserId { get => _DS3.GetString(0, 10); set => _DS3.SetString(((string)value).AsSpan(), 0, 10); } 

#endregion
        //**********************************************************************
        //**********************************************************************
#region Constructor and Dispose 
        public CUSTPRTS()
        {
            _instanceInit();
            CSMASTERL1.Open(CurrentJob.Database, AccessMode.Read, false, false, ServerCursors.Default);
            CUSTOMERL1.Open(CurrentJob.Database, AccessMode.Read, false, false, ServerCursors.Default);
            QPRINT.Printer = "Microsoft Print to PDF";
            QPRINT.ManuscriptPath = Spooler.GetNewFilePath(QPRINT.DclPrintFileName);
            QPRINT.Open(CurrentJob.PrinterDB);
        }

        override public void Dispose(bool disposing)
        {
            if (disposing)
            {
                
                CSMASTERL1.Close();
                CUSTOMERL1.Close();
                QPRINT.Close();
            }
            base.Dispose(disposing);
        }


#endregion
        void StarEntry(int cparms)
        {
            do
            {
                //----------------------------------------------------------------------
                _IN[80] = CUSTOMERL1.Chain(true, pNumber) ? '0' : '1';
                if ((bool)_IN[80])
                    CMNAME = "????????".MoveLeft(CMNAME);
                QPRINT.Write("PrtHeading", _IN.Array);
                _INOF = (Indicator)QPRINT.InOverflow;
                CSMASTERL1.Seek(SeekMode.SetLL, pNumber);
                _INLR = CSMASTERL1.ReadNextEqual(true, pNumber) ? '0' : '1';
                //----------------------------------------------------------------------
                while (_INLR == '0')
                {
                    if (CSYEAR == wPrevYr)
                    {
                        wPrtYr = 0;
                    }
                    else
                    {
                        wPrtYr = CSYEAR;
                        wPrevYr = CSYEAR;
                    }
                    ChkTheInfo();
                    _INLR = CSMASTERL1.ReadNextEqual(true, pNumber) ? '0' : '1';
                }
                //----------------------------------------------------------------------
                QPRINT.Write("PrtCount", _IN.Array);
                _INOF = (Indicator)QPRINT.InOverflow;
                //**********************************************************************
                //**********************************************************************
            } while (!(bool)_INLR);
        }
        void ChkTheInfo()
        {
            for (X = 1; X <= 12; X++)
            {
                if (CurrentJob.GetSwitch(1) == '0' && (SlsArray[(int)X - 1] > 0))
                    SlsArray[(int)X - 1] = 0;
                if (CurrentJob.GetSwitch(2) == '0' && (SlsArray[(int)X - 1] < 0))
                    SlsArray[(int)X - 1] = 0;
            }
            // IS THERE ANYTHING TO PRINT? -----------------------------------------
            for (X = 1; X <= 12; X++)
            {
                if (SlsArray[(int)X - 1] != 0)
                {
                    QPRINT.Write("PrtDetail", _IN.Array);
                    _INOF = (Indicator)QPRINT.InOverflow;
                    wCount += 1;
                    break;
                }
            }
        }
        //**********************************************************************
        //**********************************************************************

        Indicator _INOF;
#region Entry and activation methods for *ENTRY

        int _parms;

        void __ENTRY(object _pNumberAlf)
        {
            int cparms = 1;
            bool _cleanup = true;
            if (_pNumberAlf is IMODS)
                pNumberAlf.Load((_pNumberAlf as IMODS).DumpAll());
            else if (_pNumberAlf is IDS)
                pNumberAlf.Load((_pNumberAlf as IDS).Dump());
            else
                pNumberAlf.Load(_pNumberAlf.ToString());
            try
            {
                _parms = cparms;
                StarEntry(cparms);
            }
            catch(Return)
            {
            }
            catch(System.Threading.ThreadAbortException)
            {
                _cleanup = false;
                _INLR = '1';
            }
            finally
            {
                if (_cleanup)
                {
                    if (_pNumberAlf is IMODS)
                        (_pNumberAlf as IMODS).LoadAll(pNumberAlf.Dump());
                    else if (_pNumberAlf is IDS)
                        (_pNumberAlf as IDS).Load(pNumberAlf.Dump());
                    else
                        _pNumberAlf = pNumberAlf.Dump();
                }
            }
        }

        public static void _ENTRY(ICaller _caller, out Indicator __inLR, object pNumberAlf)
        {
            __inLR = RunProgram<CUSTPRTS>(_caller, (CUSTPRTS _instance) => _instance.__ENTRY(pNumberAlf));
        }
#endregion

        void _instanceInit()
        {
            X = 0;
            wUnderline = new string('-', 120);
            wPrtYr = 9999;
            wPrevYr = 9999;
            wCount = 0;
            CSMASTERL1 = new DatabaseFile(PopulateBufferCSMASTERL1, PopulateFieldsCSMASTERL1, null, "CSMASTERL1", "*LIBL/CSMASTERL1", CSMASTERL1FormatIDs)
            { IsDefaultRFN = true };
            CUSTOMERL1 = new DatabaseFile(PopulateBufferCUSTOMERL1, PopulateFieldsCUSTOMERL1, null, "CUSTOMERL1", "*LIBL/CUSTOMERL1", CUSTOMERL1FormatIDs)
            { IsDefaultRFN = true };
            QPRINT = new PrintFile(PopulateBufferQPRINT, "QPRINT", "PRINTFILES\\CUSTPRTS", QPRINTFormatIDs);
            CUSTSL = buildDSCUSTSL();
            SlsArray = new FixedDecimalArrayInDS<_12, _11, _2>(CUSTSL, LayoutType.Packed, 0);
        }
    }

}
