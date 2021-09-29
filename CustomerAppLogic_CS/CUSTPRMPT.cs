// Translated from AVR to C# on 9/28/2021 at 10:33:56 AM by ASNA Monarch Nomad® version 16.0.24.0
// ASNA Monarch(R) version 10.0.48.0 at 9/28/2021
// Migrated source location: library ERCAP, file QRPGSRC, member CUSTPRMPT

using ASNA.QSys.Runtime;
using ASNA.DataGate.Common;
using SunFarm.CustomerApp_Job;

using System;
using ASNA.QSys.Runtime.JobSupport;


namespace SunFarm.CustomerApp
{
    [ActivationGroup("*DFTACTGRP")]
    [ProgramEntry("_ENTRY")]
    public partial class CUSTPRMPT : Program
    {

        //********************************************************************
        // JB   8/27/2004   Created.
        // JB   5/23/2005   Added a window title field.
        //                  Changed the subfile page size from 10 to 8.
        //                  Fixed truncated description field.
        //                  Added a variable for the loop size in LoadSFL.
        // JB   5/25/2005   Changed cursor row to cursor field name.
        // JB   3/05/2008   Added code for displaying 'ship via' codes.
        //********************************************************************
        WorkstationFile CUSTPRMP;
        DatabaseFile SHIPPING;
        //********************************************************************
        //       Open Feedback Area
        //       Input/Output Feedback Information
        //                                                                         * 241-242 not used
        //       Display Specific Feedback Information
        //                                                                         *  cursor location
        //********************************************************************
        DataStructure _DS2 = new (32);
        FixedString<_32> WorkDS { get => _DS2.GetString(0, 32); set => _DS2.SetString(((string)value).AsSpan(), 0, 32); } 
        FixedString<_1> wkAbbrev2 { get => _DS2.GetString(0, 1); set => _DS2.SetString(((string)value).AsSpan(), 0, 1); } 
        FixedString<_2> wkAbbrev { get => _DS2.GetString(0, 2); set => _DS2.SetString(((string)value).AsSpan(), 0, 2); } 
        FixedString<_30> wkDesc { get => _DS2.GetString(2, 30); set => _DS2.SetString(((string)value).AsSpan(), 2, 30); } 
        FixedString<_31> wkDesc2 { get => _DS2.GetString(1, 31); set => _DS2.SetString(((string)value).AsSpan(), 1, 31); } 

        FixedDecimal<_5, _0> Counter;
        FixedString<_20> WinTitle;
        FixedDecimal<_5, _0> X;
        FixedStringArray<_58, _32> States;
        FixedStringArray<_5, _12> Status;
        //********************************************************************

        // Fields defined in main C-Specs declared now as Global fields (Monarch generated)
        FixedString<_10> pCsrFld;
        FixedString<_10> pResult;

#region Constructor and Dispose 
        public CUSTPRMPT()
        {
            _instanceInit();
            Load_CompileTime_Table_States();
            Load_CompileTime_Table_Status();

        }

        override public void Dispose(bool disposing)
        {
            if (disposing)
            {
                
                CUSTPRMP.Close();
                SHIPPING.Close();
            }
            base.Dispose(disposing);
        }


#endregion
        void StarEntry(int cparms)
        {
            int _RrnTmp = 0;
            do
            {
                //********************************************************************
                pResult = "";
                LoadSFL();
                _IN[90] = '0';
                SFLRRN = 1;
                CUSTPRMP.Write("MYWINDOW", _IN.Array);
                CUSTPRMP.ExFmt("SFLC", _IN.Array);
                if ((bool)_IN[12])
                {
                    _INLR = '1';
                    return;
                }
                //********************************************************************
                do
                {
                    _RrnTmp = (int)SFLRRN;
                    _IN[40] = CUSTPRMP.ReadNextChanged("SFL1", ref _RrnTmp, _IN.Array) ? '0' : '1';
                    SFLRRN = _RrnTmp;

                    if ((!(bool)_IN[40]) && SFLSEL == 1)
                    {
                        CUSTPRMP.ChainByRRN("SFL1", (int)SFLRRN, _IN.Array);
                        pResult = SFLVALUE.MoveLeft(pResult);
                        _INLR = '1';
                        return;
                    }
                } while (!((bool)_IN[40]));
                ///SPACE 3
                //********************************************************************
                //    LOAD THE SUBFILE WITH REQUIRED INFORMATION
                //********************************************************************
            } while (!(bool)_INLR);
        }
        void LoadSFL()
        {
            if (pCsrFld == "SFSTATE")
            {
                Counter = 58;
                WinTitle = "Select a State";
                for (X = 1; X <= Counter; X++)
                {
                    WorkDS = States[(int)(X - 1)];
                    SFLDESC = wkDesc.MoveLeft(SFLDESC);
                    SFLVALUE = wkAbbrev.MoveLeft(SFLVALUE);
                    SFLRRN = (decimal)X;
                    CUSTPRMP.WriteSubfile("SFL1", (int)SFLRRN, _IN.Array);
                }
            }
            else if (pCsrFld == "SFSTATUS")
            {
                Counter = 5;
                WinTitle = "Select a Status Code";
                for (X = 1; X <= Counter; X++)
                {
                    WorkDS = Status[(int)(X - 1)].MoveLeft(WorkDS);
                    SFLDESC = wkDesc2.MoveLeft(SFLDESC);
                    SFLVALUE = wkAbbrev2.MoveLeft(SFLVALUE);
                    SFLRRN = (decimal)X;
                    CUSTPRMP.WriteSubfile("SFL1", (int)SFLRRN, _IN.Array);
                }
            }
            else if (pCsrFld == "SHIPVIA")
            {
                SFLRRN = 0;
                WinTitle = "Select a Carrier";
                SHIPPING.Open(CurrentJob.Database, AccessMode.Read, false, false, ServerCursors.Default);
                _IN[66] = SHIPPING.ReadNext(true) ? '0' : '1';
                while (!(bool)_IN[66])
                {
                    SFLVALUE = EditCode.Apply(CARRIERCOD, 0, 3, EditCodes.Z, "").Trim();
                    SFLDESC = (string)CARRIERDES;
                    SFLRRN += 1;
                    CUSTPRMP.WriteSubfile("SFL1", (int)SFLRRN, _IN.Array);
                    _IN[66] = SHIPPING.ReadNext(true) ? '0' : '1';
                }
                SHIPPING.Close();
            }
        }
        //********************************************************************
#region ** Compile-time Data
        void Load_CompileTime_Table_States()
        {
            States[0] = "ALAlabama                       ";
            States[1] = "AKAlaska                        ";
            States[2] = "ASAmerican Somoa                ";
            States[3] = "AZArizona                       ";
            States[4] = "ARArkansas                      ";
            States[5] = "CACalifornia                    ";
            States[6] = "COColorado                      ";
            States[7] = "CTConnecticut                   ";
            States[8] = "DEDelaware                      ";
            States[9] = "DCDistrict of Columbia          ";
            States[10] = "FMFederated State of Micronesia ";
            States[11] = "FLFlorida                       ";
            States[12] = "GAGeorgia                       ";
            States[13] = "GUGuam                          ";
            States[14] = "HIHawaii                        ";
            States[15] = "IDIdaho                         ";
            States[16] = "ILIllinois                      ";
            States[17] = "INIndiana                       ";
            States[18] = "IAIowa                          ";
            States[19] = "KSKansas                        ";
            States[20] = "KYKentucky                      ";
            States[21] = "LALouisana                      ";
            States[22] = "MEMaine                         ";
            States[23] = "MHMarshall Islands              ";
            States[24] = "MDMaryland                      ";
            States[25] = "MAMassachussets                 ";
            States[26] = "MIMichigan                      ";
            States[27] = "MNMinnesota                     ";
            States[28] = "MSMississippi                   ";
            States[29] = "MOMissouri                      ";
            States[30] = "MTMontana                       ";
            States[31] = "NENebraska                      ";
            States[32] = "NVNevada                        ";
            States[33] = "NHNew Hampshire                 ";
            States[34] = "NJNew Jersey                    ";
            States[35] = "NMNew Mexico                    ";
            States[36] = "NYNew York                      ";
            States[37] = "NCNorth Carolina                ";
            States[38] = "NDNorth Dakota                  ";
            States[39] = "OHOhio                          ";
            States[40] = "OKOklahoma                      ";
            States[41] = "OROregon                        ";
            States[42] = "PWPalau                         ";
            States[43] = "PAPennsylvania                  ";
            States[44] = "PRPuerto Rico                   ";
            States[45] = "RIRhode Island                  ";
            States[46] = "SCSouth Carolina                ";
            States[47] = "SDSouth Dakota                  ";
            States[48] = "TNTennessee                     ";
            States[49] = "TXTexas                         ";
            States[50] = "UTUtah                          ";
            States[51] = "VTVermont                       ";
            States[52] = "VIVirgin Islands                ";
            States[53] = "VAVirgina                       ";
            States[54] = "WAWashington                    ";
            States[55] = "WVWest Virginia                 ";
            States[56] = "WIWisconsin                     ";
            States[57] = "WYWyoming                       ";
        }

        void Load_CompileTime_Table_Status()
        {
            Status[0] = "AActive     ";
            Status[1] = "CClosed     ";
            Status[2] = "OOver limit ";
            Status[3] = "RRefer      ";
            Status[4] = "SSuspended  ";
        }

#endregion

#region Entry and activation methods for *ENTRY

        int _parms;

        void __ENTRY(ref FixedString<_10> _pCsrFld, ref FixedString<_10> _pResult)
        {
            int cparms = 2;
            bool _cleanup = true;
            pResult = _pResult;
            pCsrFld = _pCsrFld;
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
                    _pCsrFld = pCsrFld;
                    _pResult = pResult;
                }
            }
        }

        public static void _ENTRY(ICaller _caller, out Indicator __inLR, ref FixedString<_10> pCsrFld, ref FixedString<_10> pResult)
        {
            FixedString<_10> _ppCsrFld = pCsrFld;
            FixedString<_10> _ppResult = pResult;
            __inLR = RunProgram<CUSTPRMPT>(_caller, (CUSTPRMPT _instance) => _instance.__ENTRY(ref _ppCsrFld, ref _ppResult));
            pCsrFld = _ppCsrFld;
            pResult = _ppResult;
        }
#endregion

        void _instanceInit()
        {
            Status = new FixedStringArray<_5, _12>((FixedString<_12>[])null);
            States = new FixedStringArray<_58, _32>((FixedString<_32>[])null);
            X = 0;
            CUSTPRMP = new WorkstationFile(PopulateBufferCUSTPRMP, PopulateFieldsCUSTPRMP, null, "CUSTPRMP", "/CustomerAppViews/CUSTPRMP");
            CUSTPRMP.Open();
            SHIPPING = new DatabaseFile(PopulateBufferSHIPPING, PopulateFieldsSHIPPING, null, "SHIPPING", "*LIBL/SHIPPING", SHIPPINGFormatIDs, isKeyed : false)
            { IsDefaultRFN = true };
        }
    }

}
