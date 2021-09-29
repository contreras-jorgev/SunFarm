// Translated from AVR to C# on 9/28/2021 at 10:33:56 AM by ASNA Monarch Nomad® version 16.0.24.0
// ASNA Monarch(R) version 10.0.48.0 at 9/28/2021
// Migrated source location: library ERCAP, file QRPGSRC, member ORDDTLINQ

using ASNA.QSys.Runtime;
using ASNA.DataGate.Common;
using SunFarm.CustomerApp_Job;

using System;
using ASNA.QSys.Runtime.JobSupport;


namespace SunFarm.CustomerApp
{
    [ActivationGroup("*DFTACTGRP")]
    [ProgramEntry("_ENTRY")]
    public partial class ORDDTLINQ : Program
    {
        protected dynamic DynamicCaller_;

        WorkstationFile ORDDTLINQD;
        DatabaseFile CUSTOMERL1;
        DatabaseFile ITEMMASTL1;
        DatabaseFile ORDERHDRL2;
        DatabaseFile ORDERLINL2;
        FixedString<_1> AddUpdDlt;
        FixedDecimal<_5, _0> HiLineNbr;
        FixedDecimal<_13, _4> OrderTotal;
        FixedDecimal<_13, _4> OrderWgt; //  Weight in ounces
        FixedDecimal<_11, _2> OrderLbs; //  Weight in pounds
        FixedDecimal<_5, _0> SflRRN;
        FixedDecimal<_13, _4> TempAmount;
        FixedDecimal<_13, _2> TempAmt2;
        FixedDecimal<_13, _4> TempWgt;
        FixedString<_22> WINTITLE;
        //********************************************************************

        // Fields defined in main C-Specs declared now as Global fields (Monarch generated)
        FixedDecimal<_9, _0> pCUSTNO;
        FixedString<_1> pDeleteAll;
        FixedDecimal<_9, _0> pItmNbr;
        FixedDecimal<_9, _0> pORDNBR;
        FixedString<_1> pSelOnly;

        // KLIST(s) relocated by Monarch
        
        
#region Constructor and Dispose 
        public ORDDTLINQ()
        {
            _instanceInit();
            CUSTOMERL1.Open(CurrentJob.Database, AccessMode.Read, false, false, ServerCursors.Default);
            ITEMMASTL1.Open(CurrentJob.Database, AccessMode.Read, false, false, ServerCursors.Default);
            ORDERHDRL2.Open(CurrentJob.Database, AccessMode.RWCD, false, false, ServerCursors.Default);
            ORDERLINL2.Open(CurrentJob.Database, AccessMode.RWCD, false, false, ServerCursors.Default);
        }

        override public void Dispose(bool disposing)
        {
            if (disposing)
            {
                
                ORDDTLINQD.Close();
                CUSTOMERL1.Close();
                ITEMMASTL1.Close();
                ORDERHDRL2.Close();
                ORDERLINL2.Close();
            }
            base.Dispose(disposing);
        }


#endregion
        void StarEntry(int cparms)
        {
            int _RrnTmp = 0;
            do
            {
                if (pDeleteAll == "Y")
                {
                    //********************************************************************
                    // KLIST "KeyHeadL2" moved by Monarch to global scope.
                    // KLIST "KeyLineL2" moved by Monarch to global scope.
                    // DELETE ALL THE DETAIL LINE ITEM RECORDS ***************************
                    ORDNUMBER = pORDNBR;
                    ORDLINNUM = 0;
                    ORDERLINL2.Seek(SeekMode.SetLL, ORDNUMBER, ORDLINNUM);
                    _INLR = ORDERLINL2.ReadNextEqual(true, ORDNUMBER) ? '0' : '1';
                    while (_INLR == '0')
                    {
                        //EOF?
                        ORDERLINL2.Delete();
                        _INLR = ORDERLINL2.ReadNextEqual(true, ORDNUMBER) ? '0' : '1';
                    }
                    return;
                }
                //********************************************************************
                while (!(bool)_IN[12])
                {
                    ORDDTLINQD.Write("KEYS", _IN.Array);
                    ORDDTLINQD.ExFmt("SFLC", _IN.Array);
                    if ((bool)_IN[12])
                    {
                        _INLR = '1';
                        break;
                    }
                    if ((bool)_IN[6])
                    {
                        // Add a line item.
                        AddUpdDlt = "A";
                        RcdUpdate();
                        _IN[12] = '0'; // Make sure it's off.
                        LoadData();
                        continue;
                    }
                    _RrnTmp = (int)SflRRN;
                    _IN[66] = ORDDTLINQD.ReadNextChanged("SFL1", ref _RrnTmp, _IN.Array) ? '0' : '1';
                    SflRRN = _RrnTmp;
                    if (!(bool)_IN[66])
                    {
                        if (SFSEL == 4)
                        {
                            // Delete the line.
                            AddUpdDlt = "D";
                            RcdDelete();
                            _IN[12] = '0'; // Make sure it's off.
                        }
                        if (SFSEL == 2)
                        {
                            // Change the line.
                            AddUpdDlt = "U";
                            RcdUpdate();
                            _IN[12] = '0'; // Make sure it's off.
                        }
                    }
                    LoadData();
                }
                //*********************************************************************
                _IN[66] = ORDERHDRL2.Chain(true, pCUSTNO, pORDNBR) ? '0' : '1';
                ORDAMOUNT = OrderTotal;
                ORDWEIGHT = OrderWgt;
                ORDERHDRL2.Update();
                //*********************************************************************
                // UPDATE THE DETAIL RECORD
                //*********************************************************************
            } while (!(bool)_INLR);
        }
        void RcdUpdate()
        {
            Indicator _LR = '0';
            if (AddUpdDlt == "U")
            {
                WINTITLE = "Line Item Update";
                SLINNBR = SFLINNBR;
                SITMNBR = SFITMNBR;
                SQTYORD = (decimal)SFQTYORD;
                _IN[88] = '1'; // Protect item nbr.
            }
            else
            {
                WINTITLE = "Add a Line Item";
                SLINNBR = 0;
                SITMNBR = 0;
                SQTYORD = 0;
                _IN[88] = '0';
            }
            _IN[40] = '0';
            _IN[41] = '0';
            _IN[99] = '0';
            _IN[89] = '0'; // Enable the qty fld.
            //-----------------------------------------------------------
            while (!(bool)_IN[12])
            {
                ORDDTLINQD.Write("MYWINDOW", _IN.Array);
                ORDDTLINQD.ExFmt("ORDLINE", _IN.Array);
                if ((bool)_IN[4])
                {
                    pSelOnly = "Y";
                    // Prompting?
                    DynamicCaller_.CallD("SunFarm.CustomerApp.ITEMINQ", out _LR, ref pSelOnly, ref pItmNbr);
                    SITMNBR = pItmNbr;
                    _IN[90] = '0'; // This fmt must be
                    ORDDTLINQD.Write("SFLC", _IN.Array); //  redisplayed again
                    continue;
                }
                if (!(bool)_IN[12])
                {
                    EditFlds();
                    if ((bool)_IN[99])
                        continue;
                    UpdDbFlds();
                    break;
                }
            }
            _IN[12] = '0'; //Reset the F12 key.
        }
        //*********************************************************************
        // DELETE THE DETAIL RECORD
        //*********************************************************************
        void RcdDelete()
        {
            ClearSel();
            WINTITLE = "Confirm Line Delete";
            SLINNBR = SFLINNBR;
            SITMNBR = SFITMNBR;
            SQTYORD = (decimal)SFQTYORD;
            _IN[40] = '0'; // Turn off errors.
            _IN[41] = '0';
            _IN[99] = '0';
            _IN[88] = '1'; // Protect fields.
            _IN[89] = '1';
            ORDDTLINQD.Write("MYWINDOW", _IN.Array);
            ORDDTLINQD.ExFmt("ORDLINE", _IN.Array);
            if (!(bool)_IN[12])
                UpdDbFlds();
        }
        //*********************************************************************
        //  EDIT THE SCREEN FIELDS
        //*********************************************************************
        void EditFlds()
        {
            _IN[40] = '0'; //Clear all error inds
            _IN[41] = '0';
            _IN[99] = '0';
            if (SITMNBR == 0)
            {
                _IN[40] = '1';
                _IN[99] = '1';
            }
            else if (SQTYORD == 0)
            {
                _IN[41] = '1';
                _IN[99] = '1';
            }
            else
            {
                ITEMNUMBER = SITMNBR;
                _IN[99] = ITEMMASTL1.Chain(true, ITEMNUMBER) ? '0' : '1';
                _IN[40] = '1';
            }
        }
        //*********************************************************************
        //  UPDATE THE DATABASE FIELDS
        //*********************************************************************
        void UpdDbFlds()
        {
            decimal _rem = 0;
            if (AddUpdDlt == "A")
            {
                ORDLINNUM = HiLineNbr + 1;
                ORDNUMBER = pORDNBR;
                ORDITEMNUM = SITMNBR;
                ORDLQTY = (decimal)SQTYORD;
                ORDLQTYBKO = 0;
                ORDLQTYSHP = 0;
                ORDLQTYDEL = 0;
                ORDERLINL2.Write();
                OrderTotal = OrderTotal + (SQTYORD * ITEMPRICE);
                OrderWgt = OrderWgt + (SQTYORD * ITEMWEIGHT);
            }
            else if (AddUpdDlt == "U")
            {
                ORDNUMBER = pORDNBR;
                ORDLINNUM = (decimal)SFLINNBR;
                ORDERLINL2.Chain(true, ORDNUMBER, ORDLINNUM);
                TempAmount = ORDLQTY * ITEMPRICE;
                TempWgt = ORDLQTY * ITEMWEIGHT;
                ORDITEMNUM = SITMNBR;
                ORDLQTY = (decimal)SQTYORD;
                ORDERLINL2.Update();
                OrderTotal = OrderTotal - TempAmount + (SQTYORD * ITEMPRICE);
                OrderWgt = OrderWgt - TempWgt + (SQTYORD * ITEMWEIGHT);
            }
            else if (AddUpdDlt == "D")
            {
                ORDNUMBER = pORDNBR;
                ORDLINNUM = (decimal)SFLINNBR;
                ORDERLINL2.Chain(true, ORDNUMBER, ORDLINNUM);
                ORDERLINL2.Delete();
                OrderTotal = OrderTotal - (SQTYORD * ITEMPRICE);
                OrderWgt = OrderWgt - (SQTYORD * ITEMWEIGHT);
            }
            TempAmt2 = (decimal)OrderTotal;
            SORDAMOUNT = "$" + EditCode.Apply(TempAmt2, 2, 13, EditCodes.One, "").Trim(); //   to show 2 decimal
            OrderLbs = DecimalOps.Divide(OrderWgt, 16, out _rem, 2, false, false );
            SORDWEIGHT = EditCode.Apply(OrderLbs, 2, 11, EditCodes.One, "").Trim() + " Lbs";
        }
        //*********************************************************************
        //  Load the Subfile Control and Subfile Records
        //*********************************************************************
        void LoadData()
        {
            _IN[90] = '1'; //Clear the subfile.
            ORDDTLINQD.Write("SFLC", _IN.Array);
            _IN[90] = '0';
            SFSEL = 0;
            SflRRN = 0;
            ORDNUMBER = pORDNBR;
            ORDLINNUM = 0;
            ORDERLINL2.Seek(SeekMode.SetLL, ORDNUMBER, ORDLINNUM);
            _IN[77] = ORDERLINL2.ReadNextEqual(true, ORDNUMBER) ? '0' : '1';
            //----------------------------------------------------------
            while (!(bool)_IN[77])
            {
                //EOF?
                HiLineNbr = (decimal)ORDLINNUM;
                SFLINNBR = (decimal)ORDLINNUM;
                SFITMNBR = ORDITEMNUM;
                _IN[66] = ITEMMASTL1.Chain(true, ORDITEMNUM) ? '0' : '1';
                SFITMDESC = ITEMSHRTDS;
                SFPRICE = ITEMPRICE;
                SFQTYORD = (decimal)ORDLQTY;
                SFEXTAMT = ORDLQTY * ITEMPRICE;
                SflRRN += 1;
                ORDDTLINQD.WriteSubfile("SFL1", (int)SflRRN, _IN.Array);
                _IN[77] = ORDERLINL2.ReadNextEqual(true, ORDNUMBER) ? '0' : '1';
            }
            if (SflRRN == 0)
            {
                SFITMDESC = "No Line Items Found";
                SflRRN += 1;
                ORDDTLINQD.WriteSubfile("SFL1", (int)SflRRN, _IN.Array);
            }
        }
        //*********************************************************************
        // CLEAR THE SELECTION NUMBER
        // Ignore the error if the update fails - it may be the first line.
        //*********************************************************************
        void ClearSel()
        {
            SFSEL = 0;
            ORDDTLINQD.Update("SFL1", _IN.Array, out _IN.Array[99]);
        }
        //*********************************************************************
        // Init Subroutine
        //*********************************************************************
        void PROCESS_STAR_INZSR()
        {
            decimal _rem = 0;
            _IN[66] = CUSTOMERL1.Chain(true, pCUSTNO) ? '0' : '1';
            if ((bool)_IN[66])
            {
                CMNAME = "? ? ?";
                CMCITY = "";
                CMSTATE = "";
            }
            SCRCUST = EditCode.Apply(CMCUSTNO, 0, 9, EditCodes.Z, "").Trim() + " " + ((string)CMNAME).Trim() + " (" + ((string)CMCITY).Trim() + ", " + ((string)CMSTATE).Trim() + ")";
            _IN[66] = ORDERHDRL2.Chain(false, pCUSTNO, pORDNBR) ? '0' : '1';
            if (!(bool)_IN[66])
            {
                SORDNUM = pORDNBR;
                SORDDATE = (DateTime)ORDDATE;
                OrderTotal = ORDAMOUNT;
                TempAmt2 = (decimal)ORDAMOUNT; // Change the total to
                SORDAMOUNT = "$" + EditCode.Apply(TempAmt2, 2, 13, EditCodes.One, "").Trim(); //   to show 2 decimal
                OrderWgt = ORDWEIGHT;
                OrderLbs = DecimalOps.Divide(OrderWgt, 16, out _rem, 2, false, false );
                SORDWEIGHT = EditCode.Apply(OrderLbs, 2, 11, EditCodes.One, "").Trim() + " Lbs";
            }
            SflRRN = 0;
            LoadData();
            _IN[75] = '1';
        }

#region Entry and activation methods for *ENTRY

        int _parms;

        bool _isInitialized;
        void __ENTRY(ref FixedDecimal<_9, _0> _pCUSTNO, ref FixedDecimal<_9, _0> _pORDNBR, ref FixedString<_1> _pDeleteAll)
        {
            int cparms = 3;
            bool _cleanup = true;
            pDeleteAll = _pDeleteAll;
            pORDNBR = _pORDNBR;
            pCUSTNO = _pCUSTNO;
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
                    _pCUSTNO = pCUSTNO;
                    _pORDNBR = pORDNBR;
                    _pDeleteAll = pDeleteAll;
                }
            }
        }

        public static void _ENTRY(ICaller _caller, out Indicator __inLR, ref FixedDecimal<_9, _0> pCUSTNO, ref FixedDecimal<_9, _0> pORDNBR, ref FixedString<_1> pDeleteAll)
        {
            FixedDecimal<_9, _0> _ppCUSTNO = pCUSTNO;
            FixedDecimal<_9, _0> _ppORDNBR = pORDNBR;
            FixedString<_1> _ppDeleteAll = pDeleteAll;
            __inLR = RunProgram<ORDDTLINQ>(_caller, (ORDDTLINQ _instance) => _instance.__ENTRY(ref _ppCUSTNO, ref _ppORDNBR, ref _ppDeleteAll));
            pCUSTNO = _ppCUSTNO;
            pORDNBR = _ppORDNBR;
            pDeleteAll = _ppDeleteAll;
        }
#endregion

        void _instanceInit()
        {
            WINTITLE = " ";
            TempWgt = 0;
            TempAmt2 = 0;
            TempAmount = 0;
            SflRRN = 0;
            OrderLbs = 0;
            OrderWgt = 0;
            OrderTotal = 0;
            HiLineNbr = 0;
            AddUpdDlt = " ";
            DynamicCaller_ = new DynamicCaller(this);
            ORDDTLINQD = new WorkstationFile(PopulateBufferORDDTLINQD, PopulateFieldsORDDTLINQD, null, "ORDDTLINQD", "/CustomerAppViews/ORDDTLINQD");
            ORDDTLINQD.Open();
            CUSTOMERL1 = new DatabaseFile(PopulateBufferCUSTOMERL1, PopulateFieldsCUSTOMERL1, null, "CUSTOMERL1", "*LIBL/CUSTOMERL1", CUSTOMERL1FormatIDs)
            { IsDefaultRFN = true };
            ITEMMASTL1 = new DatabaseFile(PopulateBufferITEMMASTL1, PopulateFieldsITEMMASTL1, null, "ITEMMASTL1", "*LIBL/ITEMMASTL1", ITEMMASTL1FormatIDs)
            { IsDefaultRFN = true };
            ORDERHDRL2 = new DatabaseFile(PopulateBufferORDERHDRL2, PopulateFieldsORDERHDRL2, null, "ORDERHDRL2", "*LIBL/ORDERHDRL2", ORDERHDRL2FormatIDs, blockingFactor : 0)
            { IsDefaultRFN = true };
            ORDERLINL2 = new DatabaseFile(PopulateBufferORDERLINL2, PopulateFieldsORDERLINL2, null, "ORDERLINL2", "*LIBL/ORDERLINL2", ORDERLINL2FormatIDs, blockingFactor : 0)
            { IsDefaultRFN = true };
        }
    }

}
