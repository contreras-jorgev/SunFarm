// Translated from AVR to C# on 9/28/2021 at 10:33:56 AM by ASNA Monarch Nomad® version 16.0.24.0
// ASNA Monarch(R) version 10.0.48.0 at 9/28/2021
// Migrated source location: library ERCAP, file QRPGSRC, member CUSTDELIV

using ASNA.QSys.Runtime;
using ASNA.DataGate.Common;
using SunFarm.CustomerApp_Job;

using System;
using ASNA.QSys.Runtime.JobSupport;


namespace YourCompany.YourApplication
{
#warning Field name CUSTDELIV renamed to avoid a clash with the class name.
    [ActivationGroup("*DFTACTGRP")]
    [ProgramEntry("_ENTRY")]
    public partial class CUSTDELIV : Program
    {

        //********************************************************************
        // JB   8/31/2004   Created.

        //********************************************************************
        DatabaseFile CUSTOMERL1;
        DatabaseFile CAMASTER;
        WorkstationFile _fCUSTDELIV;
        //********************************************************************
        // Customer DS
        DataStructure CUSTDS;

        //********************************************************************
        // Fields defined in main C-Specs declared now as Global fields (Monarch generated)
        FixedDecimal<_9, _0> pNumber;
        FixedDecimal<_4, _0> savrrn;
        FixedDecimal<_4, _0> sflrrn;

#region Constructor and Dispose 
        public CUSTDELIV()
        {
            _instanceInit();
            CAMASTER.Open(CurrentJob.Database, AccessMode.Read, false, false, ServerCursors.Default);
            CUSTOMERL1.Open(CurrentJob.Database, AccessMode.Read, false, false, ServerCursors.Default);
        }

        override public void Dispose(bool disposing)
        {
            if (disposing)
            {
                
                CUSTOMERL1.Close();
                CAMASTER.Close();
                _fCUSTDELIV.Close();
            }
            base.Dispose(disposing);
        }


#endregion
        void StarEntry(int cparms)
        {
            int _RrnTmp = 0;
            do
            {
                while (!(bool)_IN[12])
                {
                    _IN[90] = '0';
                    _fCUSTDELIV.Write("KEYS", _IN.Array);
                    _fCUSTDELIV.ExFmt("SFLC", _IN.Array);
                    // exit
                    if ((bool)_IN[12])
                    {
                        _INLR = '1';
                        break;
                    }
                    _IN[30] = '0';
                    savrrn = sflrrn;
                    _RrnTmp = (int)sflrrn;
                    _IN[40] = _fCUSTDELIV.ReadNextChanged("SFL1", ref _RrnTmp, _IN.Array) ? '0' : '1';
                    sflrrn = _RrnTmp;
                    while (!(bool)_IN[40])
                    {
                        if (SFLSEL == "1")
                        {
                            _fCUSTDELIV.ChainByRRN("SFL1", (int)sflrrn, _IN.Array);
                            Something();
                            SFLSEL = "";
                            _fCUSTDELIV.Update("SFL1", _IN.Array);
                        }
                        _RrnTmp = (int)sflrrn;
                        _IN[40] = _fCUSTDELIV.ReadNextChanged("SFL1", ref _RrnTmp, _IN.Array) ? '0' : '1';
                        sflrrn = _RrnTmp;
                    }
                    sflrrn = savrrn;
                }
                //****************************************

                //****************************************
            } while (!(bool)_INLR);
        }
        void Something()
        {
        }
        //**********************
        //  Load Sfl Subroutine
        //**********************
        void LoadSfl()
        {
            _IN[99] = CAMASTER.ReadNextEqual(true, pNumber) ? '0' : '1';
            while (!(bool)_IN[99])
            {
                SFLCUSTh = CACUSTNO;
                SFLCUST = (string)CANAME;
                SFLCITY = CACITY.TrimEnd() + ", " + CASTATE;
                SFLZIP = (string)CAZIP;
                sflrrn += 1;
                _fCUSTDELIV.WriteSubfile("SFL1", (int)sflrrn, _IN.Array);
                _IN[99] = CAMASTER.ReadNextEqual(true, pNumber) ? '0' : '1';
            }
            // end of file
            if (sflrrn == 0)
            {
                sflrrn += 1;
                CMCUSTNO = 0;
                SFLCUST = "No Address Records Found";
                SFLCITY = "";
                SFLZIP = "";
                _fCUSTDELIV.WriteSubfile("SFL1", (int)sflrrn, _IN.Array);
            }
        }
        //*********************
        // Init Subroutine
        //*********************
        void PROCESS_STAR_INZSR()
        {
            sflrrn = 0;
            _IN[90] = '1';
            _fCUSTDELIV.Write("SFLC", _IN.Array);
            CAMASTER.Seek(SeekMode.SetLL, pNumber);
            LoadSfl();
        }

#region Entry and activation methods for *ENTRY

        int _parms;

        bool _isInitialized;
        void __ENTRY(ref FixedDecimal<_9, _0> _pNumber)
        {
            int cparms = 1;
            bool _cleanup = true;
            pNumber = _pNumber;
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
                    _pNumber = pNumber;
            }
        }

        public static void _ENTRY(ICaller _caller, out Indicator __inLR, ref FixedDecimal<_9, _0> pNumber)
        {
            FixedDecimal<_9, _0> _ppNumber = pNumber;
            __inLR = RunProgram<CUSTDELIV>(_caller, (CUSTDELIV _instance) => _instance.__ENTRY(ref _ppNumber));
            pNumber = _ppNumber;
        }
#endregion

        void _instanceInit()
        {
            CUSTOMERL1 = new DatabaseFile(PopulateBufferCUSTOMERL1, PopulateFieldsCUSTOMERL1, null, "CUSTOMERL1", "*LIBL/CUSTOMERL1", CUSTOMERL1FormatIDs)
            { IsDefaultRFN = true };
            CAMASTER = new DatabaseFile(PopulateBufferCAMASTER, PopulateFieldsCAMASTER, null, "CAMASTER", "*LIBL/CAMASTER", CAMASTERFormatIDs)
            { IsDefaultRFN = true };
            _fCUSTDELIV = new WorkstationFile(PopulateBuffer_fCUSTDELIV, PopulateFields_fCUSTDELIV, null, "_fCUSTDELIV", "/CustomerAppViews/CUSTDELIV");
            _fCUSTDELIV.Open();
            CUSTDS = buildDSCUSTDS();
        }
    }

}
