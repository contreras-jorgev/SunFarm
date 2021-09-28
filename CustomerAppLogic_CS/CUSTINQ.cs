// Translated from AVR to C# on 9/28/2021 at 10:33:56 AM by ASNA Monarch Nomad® version 16.0.24.0
// ASNA Monarch(R) version 10.0.48.0 at 9/28/2021
// Migrated source location: library ERCAP, file QRPGSRC, member CUSTINQ

using ASNA.QSys.Runtime;
using ASNA.DataGate.Common;
using SunFarm.CustomerApp_Job;

using System;
using ASNA.QSys.Runtime.JobSupport;


namespace YourCompany.YourApplication
{
    [ActivationGroup("*DFTACTGRP")]
    [ProgramEntry("_ENTRY")]
    public partial class CUSTINQ : Program
    {
        protected dynamic DynamicCaller_;

        //********************************************************************
        // JB   8/27/2004   Added F4 (Prompt) options.
        // JB   8/30/2004   Added 'submit job' option.
        // JB   9/01/2004   Fixed record locking problem.
        // JB   5/19/2005   Added colouring to the name/addr subfile.
        //                  Added cursor postioning to the detail format.
        //                  Added option to print sales online or in batch.
        //                  Changed the prompt display to a window.
        // JB   5/23/2005   Added /COPY for reading CUSTOMERL1.
        //                  Added customer name to the sales summary format.
        //                  Added renaming of CUSTOMERL1 record format.
        // JB   5/24/2005   Simplified the LoadSfl subroutine.
        // JB   5/31/2005   Added PageDown/RollUp processing.
        // JB   6/01/2005   Renamed CMMASTER* files to CUSTOMER*.
        // JB  14/02/2006   Fixed bug in page up on the customer subfile -
        //                  a customer record was skipped every time.
        //********************************************************************
        // INDICATORS:
        //   03     F3 pressed
        //   09     F9 pressed
        //   40-44  Cursor positioning
        //   50     PageUp pressed
        //   51     PageDown pressed
        //   66     EOF reading on the subfile
        //   76     BOF reading CUSTOMERL2
        //   77     EOF reading CUSTMOMER2
        //   88     LR seton in a called program
        //   99     General error indicator
        //********************************************************************
        WorkstationFile CUSTDSPF;
        //                                    HANDLER('ASNAWINGS')
        DatabaseFile CUSTOMERL2;
        DatabaseFile CUSTOMERL1;
        //********************************************************************
        DataStructure _DS0 = new (3);
        FixedDecimal<_3, _0> hpNbrs { get => _DS0.GetZoned(0, 3, 0); set => _DS0.SetZoned(value, 0, 3, 0); } 
        FixedString<_3> hpNbrsAlf { get => _DS0.GetString(0, 3); set => _DS0.SetString(((string)value).AsSpan(), 0, 3); } 

        FixedDecimalArray<_20, _9, _0> pNumbers;
        FixedStringArray<_20, _1> pTypes;
        FixedString<_7> MID;
        FixedString<_30> MTX;
        FixedDecimal<_13, _2> Sales;
        FixedDecimal<_13, _2> Returns;
        FixedString<_1> LockRec;
        FixedDecimal<_9, _0> CmCusth;
        FixedString<_13> SalesCh;
        FixedString<_13> ReturnsCh;
        FixedString<_9> CmCusthCH;
        FixedDecimal<_5, _0> X;
        DataStructure _DS1 = new (9);
        FixedDecimal<_9, _0> SVCUSTNO { get => _DS1.GetZoned(0, 9, 0); set => _DS1.SetZoned(value, 0, 9, 0); } 
        FixedString<_9> SVCUSTNOa { get => _DS1.GetString(0, 9); set => _DS1.SetString(((string)value).AsSpan(), 0, 9); } 

        // Customer DS
        DataStructure CUSTDS;

        //       Open Feedback Area
        //       Input/Output Feedback Information
        //                                                                         * 241-242 not used
        //       Display Specific Feedback Information
        //                                                                         *  cursor location
        //********************************************************************
        // Fields defined in main C-Specs declared now as Global fields (Monarch generated)
        FixedString<_1> AddUpd;
        FixedString<_40> Name40;
        FixedDecimal<_9, _0> ORDCUST;
        FixedDecimal<_9, _0> pNumber;
        FixedString<_10> pResult;
        FixedDecimal<_4, _0> savrrn;
        FixedDecimal<_4, _0> sflrrn;
        FixedDecimal<_9, _0> TEMPNO;

        // PLIST(s) relocated by Monarch
        
        // KLIST(s) relocated by Monarch
        
#region Constructor and Dispose 
        public CUSTINQ()
        {
            _instanceInit();
            // Initialization of Data Structure fields (Monarch generated)
            Reset_hpNbrs();
            CUSTOMERL1.Open(CurrentJob.Database, AccessMode.RWCD, false, false, ServerCursors.Default);
            CUSTOMERL2.Open(CurrentJob.Database, AccessMode.Read, false, false, ServerCursors.Default);
        }

        override public void Dispose(bool disposing)
        {
            if (disposing)
            {
                
                CUSTDSPF.Close();
                CUSTOMERL2.Close();
                CUSTOMERL1.Close();
            }
            base.Dispose(disposing);
        }


#endregion
        void StarEntry(int cparms)
        {
            Indicator _LR = '0';
            int _RrnTmp = 0;
            FixedString<_9> _SVCUSTNOaProxy = "";
            do
            {
                do
                {
                    // KLIST "KeyMastL2" moved by Monarch to global scope.
                    //********************************************************************
                    _IN[90] = '0';
                    CUSTDSPF.Write("MSGSFC", _IN.Array);
                    CUSTDSPF.Write("KEYS", _IN.Array);
                    CUSTDSPF.ExFmt("SFLC", _IN.Array);
                    ClearMsgs();
                    if ((bool)_IN[3])
                    {
                        //--------------------------------------------------------------------
                        _INLR = '1';
                        break;
                        // PageUp-RollDown
                    }
                    else if ((bool)_IN[50])
                    {
                        CUSTOMERL2.Seek(SeekMode.SetLL, CMNAME);
                        LoadSfl();
                    }
                    else if ((bool)_IN[9])
                    {
                        // Work with spooled files
                        DynamicCaller_.CallD("YourCompany.YourApplication.WWSPLF", out _LR);
                    }
                    else if (!SETNAME.IsBlanks())
                    {
                        Name40 = (string)SETNAME;
                        CUSTOMERL2.Seek(SeekMode.SetLL, Name40);
                        LoadSfl();
                    }
                    else if ((bool)_IN[51])
                    {
                        // PageDown-RollUp
                        ReadBack();
                        LoadSfl();
                    }
                    else if (!SETNAME.IsBlanks())
                    {
                        Name40 = (string)SETNAME;
                        CUSTOMERL2.Seek(SeekMode.SetLL, Name40);
                        LoadSfl();
                    }
                    else
                    {
                        _IN[30] = '0';
                        _IN[66] = '0';
                        savrrn = sflrrn;
                        pNumbers.Initialize(0);
                        hpNbrs = 0;
                        do
                        {
                            _RrnTmp = (int)sflrrn;
                            _IN[66] = CUSTDSPF.ReadNextChanged("SFL1", ref _RrnTmp, _IN.Array) ? '0' : '1';
                            sflrrn = _RrnTmp;
                            if (!(bool)_IN[66])
                            {
                                if (SFSEL == 10)
                                {
                                    // Print in batch.
                                    CUSTDSPF.ChainByRRN("SFL1", (int)sflrrn, _IN.Array);
                                    hpNbrs = hpNbrs + 1;
                                    pNumbers[(int)(hpNbrs - 1)] = (decimal)SFCUSTNO;
                                    pTypes[(int)(hpNbrs - 1)] = "P";
                                    ClearSel();
                                }
                                else if (SFSEL == 9)
                                {
                                    SVCUSTNO = (decimal)SFCUSTNO;
                                    _SVCUSTNOaProxy = SVCUSTNOa;
                                    DynamicCaller_.CallD("YourCompany.YourApplication.CUSTPRTS", out _IN.Array[88], ref _SVCUSTNOaProxy);
                                    SVCUSTNOa = _SVCUSTNOaProxy;
                                    MID = "CST0006";
                                    MTX = " ";
                                    DynamicCaller_.CallD("YourCompany.YourApplication.MSGLOD", out _LR, ref MID, ref MTX);
                                    ClearSel();
                                }
                                else if (SFSEL == 7)
                                {
                                    // Create sales rec.
                                    CUSTDSPF.ChainByRRN("SFL1", (int)sflrrn, _IN.Array);
                                    hpNbrs = hpNbrs + 1;
                                    pNumbers[(int)(hpNbrs - 1)] = (decimal)SFCUSTNO;
                                    pTypes[(int)(hpNbrs - 1)] = "C";
                                    ClearSel();
                                }
                                else if (SFSEL == 5)
                                {
                                    // Display delivery
                                    CUSTDSPF.ChainByRRN("SFL1", (int)sflrrn, _IN.Array); //  addresses.
                                    pNumber = (decimal)SFCUSTNO;
                                    DynamicCaller_.CallD("YourCompany.YourApplication.CUSTDELIV", out _IN.Array[88], ref pNumber);
                                    ClearSel();
                                }
                                else if (SFSEL == 3)
                                {
                                    // Display sales and
                                    CUSTDSPF.ChainByRRN("SFL1", (int)sflrrn, _IN.Array); //  returns totals.
                                    SalesInfo();
                                }
                                else if (SFSEL == 11)
                                {
                                    // Maintainance.
                                    CUSTDSPF.ChainByRRN("SFL1", (int)sflrrn, _IN.Array);
                                    ORDCUST = (decimal)SFCUSTNO;
                                    DynamicCaller_.CallD("YourCompany.YourApplication.ORDHINQ", out _LR, ref ORDCUST);
                                    ClearSel();
                                }
                                else if (SFSEL == 2)
                                {
                                    // Maintainance.
                                    CUSTDSPF.ChainByRRN("SFL1", (int)sflrrn, _IN.Array);
                                    RcdUpdate();
                                }
                            }
                        } while (!((bool)_IN[66]));
                        if (hpNbrs > 0)
                        {
                            //Are there any jobs
                            DynamicCaller_.CallD("YourCompany.YourApplication.CUSTSBMJOB", out _IN.Array[88], ref pNumbers, ref pTypes); // to submit to batch?
                            MID = "CST0005";
                            MTX = (string)hpNbrsAlf;
                            DynamicCaller_.CallD("YourCompany.YourApplication.MSGLOD", out _LR, ref MID, ref MTX);
                        }
                        sflrrn = savrrn;
                    }
                } while (!((bool)_IN[3]));
                //*********************************************************************
                // UPDATE THE CUSTOMER RECORD
                //*********************************************************************
            } while (!(bool)_INLR);
        }
        void RcdUpdate()
        {
            Indicator _LR = '0';
            ClearSel();
            AddUpd = "U";
            CMCUSTNO = (decimal)SFCUSTNO;
            LockRec = "N";
            CustChk();
            SFOLDNAME = CMNAME;
            SFNAME = CMNAME;
            SFADDR1 = CMADDR1;
            SFADDR2 = CMADDR2;
            SFCITY = CMCITY;
            SFSTATE = CMSTATE;
            SFPOSTCODE = CMPOSTCODE;
            SFFAX = CMFAX;
            SFPHONE = CMPHONE;
            SFSTATUS = CMACTIVE;
            SFCONTACT = CMCONTACT;
            SFCONEMAL = CMCONEMAL;
            SFYN01 = CMYN01;
            _IN[40] = '1'; // Place the cursor on
            _IN[41] = '0'; //  the name field.
            _IN[42] = '0';
            _IN[43] = '0';
            _IN[44] = '0';
            _IN[99] = '0';
            //-------------------------------------------------------
            do
            {
                CUSTDSPF.Write("MSGSFC", _IN.Array);
                CUSTDSPF.ExFmt("CUSTREC", _IN.Array);
                _IN[40] = '0';
                _IN[41] = '0';
                _IN[42] = '0';
                _IN[43] = '0';
                _IN[44] = '0';
                ClearMsgs();
                _IN[30] = '0';

                if ((bool)_IN[3])
                {
                    break;

                }
                else if ((bool)_IN[12])
                {
                    break;
                    // Prompt
                }
                else if ((bool)_IN[4])
                {
                    if (CSRFLD == "SFSTATE" || (CSRFLD == "SFSTATUS"))
                    {
                        DynamicCaller_.CallD("YourCompany.YourApplication.CUSTPRMPT", out _IN.Array[88], ref CSRFLD, ref pResult);
                        if (!pResult.IsBlanks())
                        {
                            if (CSRFLD == "SFSTATE")
                                SFSTATE = (string)pResult;
                            else
                                SFSTATUS = (string)pResult;
                        }
                    }
                    else
                    {
                        MID = "CST0004";
                        MTX = " ";
                        DynamicCaller_.CallD("YourCompany.YourApplication.MSGLOD", out _LR, ref MID, ref MTX);
                        _IN[4] = '0';
                    }
                }
                else if ((bool)_IN[6])
                {
                    SVCUSTNO = CMCUSTNO;
                    CUSTOMERL1.Seek(SeekMode.SetGT, 999999999m);
                    CUSTOMERL1.ReadPrevious(false);
                    TEMPNO = CMCUSTNO + 100;
                    CUSTDS.Clear();
                    CMCUSTNO = TEMPNO;
                    AddUpd = "A";
                    SFCUSTNO = (decimal)TEMPNO;
                    SFNAME = "";
                    SFADDR1 = "";
                    SFADDR2 = "";
                    SFCITY = "";
                    SFSTATE = "";
                    SFPOSTCODE = "";
                    SFFAX = 0;
                    SFPHONE = "";
                    SFSTATUS = "";
                    SFCONTACT = "";
                    SFCONEMAL = "";
                    SFYN01 = "";
                    _IN[30] = '1';
                    // Delete - leave sales hanging?
                }
                else if ((bool)_IN[11])
                {
                    LockRec = "Y";
                    CustChk();
                    if (!(bool)_IN[80])
                        CUSTOMERL1.Delete();
                    CUSTDSPF.ChainByRRN("SFL1", (int)sflrrn, _IN.Array);
                    SFCSZ = "";
                    SFNAME1 = "*** DELETED ***";
                    // Set the color
                    _IN[60] = '1';
                    CUSTDSPF.Update("SFL1", _IN.Array);
                    _IN[60] = '0';
                    // Delete msg
                    MID = "CST0003";
                    MTX = CMCUSTNO.MoveRight(MTX);
                    DynamicCaller_.CallD("YourCompany.YourApplication.MSGLOD", out _LR, ref MID, ref MTX);
                    break;
                    // Add/update on the Enter key
                }
                else
                {
                    if (AddUpd == "A")
                    {
                        EditFlds();
                        if ((bool)_IN[99])
                            // Any errors?
                            continue;
                        UpdDbFlds();
                        CUSTOMERL1.Write();
                        // Added message
                        MID = "CST0001";
                        MTX = CMCUSTNO.MoveRight(MTX);
                        DynamicCaller_.CallD("YourCompany.YourApplication.MSGLOD", out _LR, ref MID, ref MTX);
                        break;
                        // Update the database
                    }
                    else
                    {
                        EditFlds();
                        if ((bool)_IN[99])
                            // Any errors?
                            continue;
                        LockRec = "Y";
                        CustChk(); //Reread the record.
                        UpdDbFlds();
                        CUSTOMERL1.Update();
                        CUSTDSPF.ChainByRRN("SFL1", (int)sflrrn, _IN.Array);
                        SFNAME1 = CMNAME;
                        Fix_CSZ();
                        CUSTDSPF.Update("SFL1", _IN.Array);
                        // Updated message
                        MID = "CST0002";
                        MTX = CMCUSTNO.MoveLeft(MTX);
                        DynamicCaller_.CallD("YourCompany.YourApplication.MSGLOD", out _LR, ref MID, ref MTX);
                    }
                    // Re-open for the next update
                    LockRec = "N";
                    CustChk();
                }
            } while (!((bool)_IN[12]));
        }
        //*********************************************************************
        //  EDIT THE SCREEN FIELDS
        //*********************************************************************
        void EditFlds()
        {
            Indicator _LR = '0';
            _IN[40] = '0'; //Clear all error inds
            _IN[41] = '0';
            _IN[42] = '0';
            _IN[43] = '0';
            _IN[44] = '0';
            _IN[99] = '0';
            if (SFNAME.IsBlanks())
            {
                _IN[40] = '1';
                _IN[99] = '1';
                MID = "CST1001"; // Blank field.
                MTX = "customer name";
            }
            else if (SFADDR1.IsBlanks())
            {
                _IN[41] = '1';
                _IN[99] = '1';
                MID = "CST1001"; // Blank field.
                MTX = "first address line";
            }
            else if (SFCITY.IsBlanks())
            {
                _IN[42] = '1';
                _IN[99] = '1';
                MID = "CST1001"; // Blank field.
                MTX = "city";
            }
            else if (SFSTATE.IsBlanks())
            {
                _IN[43] = '1';
                _IN[99] = '1';
                MID = "CST1002"; // Blank field.
                MTX = "state";
            }
            else if (SFSTATUS.IsBlanks())
            {
                _IN[44] = '1';
                _IN[99] = '1';
                MID = "CST1002"; //Blank field.
                MTX = "status";
            }
            if ((bool)_IN[99])
            {
                DynamicCaller_.CallD("YourCompany.YourApplication.MSGLOD", out _LR, ref MID, ref MTX);
            }
        }
        //*********************************************************************
        //  UPDATE THE DATABASE FIELDS
        //*********************************************************************
        void UpdDbFlds()
        {
            CMCUSTNO = (decimal)SFCUSTNO;
            CMNAME = ((string)SFNAME).Trim();
            CMADDR1 = ((string)SFADDR1).Trim();
            CMADDR2 = ((string)SFADDR2).Trim();
            CMCITY = ((string)SFCITY).Trim();
            CMSTATE = SFSTATE;
            CMPOSTCODE = SFPOSTCODE;
            CMFAX = SFFAX;
            CMPHONE = SFPHONE;
            CMACTIVE = SFSTATUS;
            CMCONTACT = ((string)SFCONTACT).Trim();
            CMCONEMAL = ((string)SFCONEMAL).Trim();
            CMYN01 = ((string)SFYN01).Trim();
        }
        //*********************************************************************
        // Retrieve Calculated Sales Info
        //*********************************************************************
        void SalesInfo()
        {
            ClearSel();
            CMCUSTNO = (decimal)SFCUSTNO;
            LockRec = "N";
            CustChk();
            CmCusth = CMCUSTNO;
            Sales = 0;
            Returns = 0;
            CmCusthCH = CMCUSTNO.MoveRight(CmCusthCH);
            SalesCh = new string('0', 13);
            ReturnsCh = new string('0', 13);
            DynamicCaller_.CallD("YourCompany.YourApplication.CUSTCALC", out _IN.Array[88], ref CmCusthCH, ref SalesCh, ref ReturnsCh);
            SFNAME = CMNAME;
            SFSALES = SalesCh.ToZonedDecimal(13, 2);
            SFRETURNS = ReturnsCh.ToZonedDecimal(13, 2);
            do
            {
                CUSTDSPF.ExFmt("SALESREC", _IN.Array);
            } while (!(bool)_IN[3] && !(bool)_IN[12]);
            _IN[3] = '0'; // Reset the exit ind.
        }
        //*********************************************************************
        //  Load Sfl Subroutine
        //*********************************************************************
        void LoadSfl()
        {
            _IN[61] = '0'; //Start with green.
            _IN[90] = '1'; //Clear the subfile.
            CUSTDSPF.Write("SFLC", _IN.Array);
            _IN[76] = '0'; //Display records.
            _IN[90] = '0';
            sflrrn = 0;
            _IN[77] = CUSTOMERL2.ReadNext(true) ? '0' : '1';
            //----------------------------------------------------------
            while (!(bool)_IN[77] && (sflrrn < 14))
            {
                SFCUSTNO = (decimal)CMCUSTNO;
                SFNAME1 = CMNAME;
                Fix_CSZ();
                if ((bool)_IN[61])
                    //Save the color of
                    SFCOLOR = "W"; // the row.
                else
                    SFCOLOR = "G";
                sflrrn += 1;
                CUSTDSPF.WriteSubfile("SFL1", (int)sflrrn, _IN.Array);
                _IN[61] = (Indicator)(!(bool)_IN[61]); //Reverse the colour.
                _IN[77] = CUSTOMERL2.ReadNext(true) ? '0' : '1';
            }
            // Any records found?
            if (sflrrn == 0)
            {
                sflrrn = 1;
                SFCUSTNO = 0;
                CMCUSTNO = 0;
                SFNAME1 = "END OF FILE".MoveLeft(SFNAME1);
                SFCSZ = "";
                CUSTDSPF.WriteSubfile("SFL1", (int)sflrrn, _IN.Array);
            }
        }
        //*********************************************************************
        //  Read Backwards for a PageDown
        //*********************************************************************
        void ReadBack()
        {
            _IN[76] = '0';
            _IN[77] = '0';
            X = 0;
            CUSTDSPF.ChainByRRN("SFL1", 1, _IN.Array); //Get the top name and
            CMNAME = SFNAME1; // number.
            CMCUSTNO = (decimal)SFCUSTNO;
            CUSTOMERL2.Chain(true, CMNAME, CMCUSTNO);
            _IN[76] = CUSTOMERL2.ReadPrevious(true) ? '0' : '1';
            while (!(bool)_IN[76] && (X < 14))
            {
                /* EOF or full s/f. */
                X += 1;
                _IN[76] = CUSTOMERL2.ReadPrevious(true) ? '0' : '1';
            }
            if ((bool)_IN[76])
                //Any records found?
                CUSTOMERL2.Seek(SeekMode.SetLL, new string(char.MinValue, 40));
        }
        //*********************************************************************
        // Fix CSZ Subroutine
        //*********************************************************************
        void Fix_CSZ()
        {
            SFCSZ = CMCITY.TrimEnd() + ", " + CMSTATE + " " + CMPOSTCODE;
        }
        //*********************************************************************
        // CLEAR THE SELECTION NUMBER
        //*********************************************************************
        void ClearSel()
        {
            SFSEL = 0;
            _IN[61] = (Indicator)(SFCOLOR == "W");
            CUSTDSPF.Update("SFL1", _IN.Array);
        }
        //*********************************************************************
        // CLEAR THE MESSAGE QUEUE
        //*********************************************************************
        void ClearMsgs()
        {
            Indicator _LR = '0';
            DynamicCaller_.CallD("YourCompany.YourApplication.MSGCLR", out _LR);
            MID = "";
        }
        //*********************************************************************
        //     * Init Subroutine
        //*********************************************************************
        void PROCESS_STAR_INZSR()
        {
            Indicator _LR = '0';
            CUSTOMERL2.Seek(SeekMode.SetLL, new string(char.MinValue, 40));
            LoadSfl();
            _IN[75] = '1';
            MID = "";
            aPGMQ = "*";
            DynamicCaller_.CallD("YourCompany.YourApplication.MSGCLR", out _LR);
        }
#region CopyBook ".\Copybooks\ErcapCustchk.vr"
        // ASNA Monarch(R) version 10.0.48.0 at 9/28/2021
        // Migrated source location: library ERCAP, file QRPGSRC, member CUSTCHK

        ///Space 3
        //********************************************************************
        //   CHECK THE CUSTOMER NUMBER
        //********************************************************************
        void CustChk()
        {
            if (LockRec == "N")
                _IN[80] = CUSTOMERL1.Chain(false, CMCUSTNO) ? '0' : '1';
            else
                _IN[80] = CUSTOMERL1.Chain(true, CMCUSTNO) ? '0' : '1';
        }

#endregion


#region File Information Data Structures

        // Message handling parm list
        // PLIST "#PLMSG" moved by Monarch to global scope.
        int STATUS
        {
            get => CUSTDSPF.StatusCode;
            set => CUSTDSPF.StatusCode = value;
        }


#endregion


        void Reset_hpNbrs()
        {
            hpNbrs = 0;
        }

#region Entry and activation methods for *ENTRY

        int _parms;

        bool _isInitialized;
        void __ENTRY()
        {
            int cparms = 0;
            bool _cleanup = true;
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
                _ = _cleanup;
            }
        }

        public static void _ENTRY(ICaller _caller, out Indicator __inLR)
        {
            __inLR = RunProgram<CUSTINQ>(_caller, (CUSTINQ _instance) => _instance.__ENTRY());
        }
#endregion

        void _instanceInit()
        {
            X = 0;
            pTypes = new FixedStringArray<_20, _1>((FixedString<_1>[])null);
            pNumbers = new FixedDecimalArray<_20, _9, _0>((FixedDecimal<_9, _0>[])null);
            DynamicCaller_ = new DynamicCaller(this);
            CUSTDSPF = new WorkstationFile(PopulateBufferCUSTDSPF, PopulateFieldsCUSTDSPF, null, "CUSTDSPF", "/CustomerAppViews/CUSTDSPF");
            CUSTDSPF.Open();
            CUSTOMERL2 = new DatabaseFile(PopulateBufferCUSTOMERL2, PopulateFieldsCUSTOMERL2, null, "CUSTOMERL2", "*LIBL/CUSTOMERL2", CUSTOMERL2FormatIDs)
            { IsDefaultRFN = true };
            CUSTOMERL1 = new DatabaseFile(PopulateBufferCUSTOMERL1, PopulateFieldsCUSTOMERL1, null, "CUSTOMERL1", "*LIBL/CUSTOMERL1", CUSTOMERL1FormatIDs, blockingFactor : 0)
            { IsDefaultRFN = true };
            CUSTDS = buildDSCUSTDS();
        }
    }

}
