// Translated from AVR to C# on 9/28/2021 at 10:33:55 AM by ASNA Monarch Nomad® version 16.0.24.0
// ASNA Monarch(R) version 10.0.48.0 at 9/28/2021
// Migrated source location: library ERCAP, file QRPGSRC, member CUSTCALC

using ASNA.QSys.Runtime;
using ASNA.DataGate.Common;
using SunFarm.CustomerApp_Job;

using System;
using ASNA.QSys.Runtime.JobSupport;


namespace SunFarm.CustomerApp
{
    [ActivationGroup("*DFTACTGRP")]
    [ProgramEntry("_ENTRY")]
    public partial class CUSTCALC : Program
    {

        //********************************************************************
        // JB   8/30/2004   Added option to display billing info.

        //********************************************************************
        DatabaseFile CUSTOMERL1;
        DatabaseFile CSMASTERL1;
        //********************************************************************
        //  ** ENTRY Parm List **
        FixedDecimal<_9, _0> Custh;
        FixedDecimal<_13, _2> Sales;
        FixedDecimal<_13, _2> TempAmt;
        FixedDecimal<_13, _2> Returns;
        FixedString<_9> CusthCh;
        FixedString<_13> SalesCh;
        FixedString<_13> ReturnsCh;

        FixedDecimal<_1, _0> SaleEvent;
        FixedDecimal<_1, _0> ReturnEvent;

        DataStructure CUSTSL;
        FixedDecimalArrayInDS<_12, _11, _2> SlsArray; 

        //********************************************************************
#region Constructor and Dispose 
        public CUSTCALC()
        {
            _instanceInit();
            CSMASTERL1.Open(CurrentJob.Database, AccessMode.Read, false, false, ServerCursors.Default);
            CUSTOMERL1.Open(CurrentJob.Database, AccessMode.Read, false, false, ServerCursors.Default);
        }

        override public void Dispose(bool disposing)
        {
            if (disposing)
            {
                
                CUSTOMERL1.Close();
                CSMASTERL1.Close();
            }
            base.Dispose(disposing);
        }


#endregion
        void StarEntry(int cparms)
        {
            do
            {
                Sales = 0;
                Returns = 0;

                // Get Customer Master Record

                Custh = CusthCh.MoveRight(Custh);
                _IN[90] = CUSTOMERL1.Chain(true, Custh) ? '0' : '1';
                //* Position Sales File to Customer
                if (!(bool)_IN[90])
                {
                    CSMASTERL1.Seek(SeekMode.SetLL, Custh);
                    _IN[3] = CSMASTERL1.ReadNextEqual(true, Custh) ? '0' : '1';
                    //*Read Sales Records
                    while (!(bool)_IN[3])
                    {
                        //*Sales
                        TempAmt = SlsArray.Sum();
                        if (CSTYPE == SaleEvent)
                            Sales = Sales + TempAmt;
                        //*Returns
                        if (CSTYPE == ReturnEvent)
                            Returns = Returns + TempAmt;
                        //*Read Next
                        _IN[3] = CSMASTERL1.ReadNextEqual(true, Custh) ? '0' : '1';
                    }
                }
                SalesCh = Sales.MoveRight(SalesCh);
                ReturnsCh = Returns.MoveRight(ReturnsCh);
                _INLR = '1';
                ///SPACE 3
                // * * * * * * * * * * ** *
                //  Initialize Event Fields
                // * * * * * * * * * * ** *
            } while (!(bool)_INLR);
        }
        void PROCESS_STAR_INZSR()
        {
            SaleEvent = 1;
            ReturnEvent = 2;
            _IN[3] = '0';
            Sales = 0;
            Returns = 0;
        }

#region Entry and activation methods for *ENTRY

        int _parms;

        bool _isInitialized;
        void __ENTRY(ref FixedString<_9> _CusthCh, ref FixedString<_13> _SalesCh, ref FixedString<_13> _ReturnsCh)
        {
            int cparms = 3;
            bool _cleanup = true;
            ReturnsCh = _ReturnsCh;
            SalesCh = _SalesCh;
            CusthCh = _CusthCh;
            try
            {
                _parms = cparms;
                if (!_isInitialized)
                {
                    PROCESS_STAR_INZSR();
                    _isInitialized = true;
                }
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
                    _CusthCh = CusthCh;
                    _SalesCh = SalesCh;
                    _ReturnsCh = ReturnsCh;
                }
            }
        }

        public static void _ENTRY(ICaller _caller, out Indicator __inLR, ref FixedString<_9> CusthCh, ref FixedString<_13> SalesCh, ref FixedString<_13> ReturnsCh)
        {
            FixedString<_9> _pCusthCh = CusthCh;
            FixedString<_13> _pSalesCh = SalesCh;
            FixedString<_13> _pReturnsCh = ReturnsCh;
            __inLR = RunProgram<CUSTCALC>(_caller, (CUSTCALC _instance) => _instance.__ENTRY(ref _pCusthCh, ref _pSalesCh, ref _pReturnsCh));
            CusthCh = _pCusthCh;
            SalesCh = _pSalesCh;
            ReturnsCh = _pReturnsCh;
        }
#endregion

        void _instanceInit()
        {
            CUSTOMERL1 = new DatabaseFile(PopulateBufferCUSTOMERL1, PopulateFieldsCUSTOMERL1, null, "CUSTOMERL1", "*LIBL/CUSTOMERL1", CUSTOMERL1FormatIDs)
            { IsDefaultRFN = true };
            CSMASTERL1 = new DatabaseFile(PopulateBufferCSMASTERL1, PopulateFieldsCSMASTERL1, null, "CSMASTERL1", "*LIBL/CSMASTERL1", CSMASTERL1FormatIDs)
            { IsDefaultRFN = true };
            CUSTSL = buildDSCUSTSL();
            SlsArray = new FixedDecimalArrayInDS<_12, _11, _2>(CUSTSL, LayoutType.Packed, 0);
        }
    }

}
