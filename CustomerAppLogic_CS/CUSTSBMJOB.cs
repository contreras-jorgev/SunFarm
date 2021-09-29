// Translated from AVR to C# on 9/28/2021 at 10:33:56 AM by ASNA Monarch Nomad® version 16.0.24.0
// ASNA Monarch(R) version 10.0.48.0 at 9/28/2021
// Migrated source location: library ERCAP, file QRPGSRC, member CUSTSBMJOB

using ASNA.QSys.Runtime;
using ASNA.DataGate.Common;
using SunFarm.CustomerApp_Job;

using System;
using ASNA.QSys.Runtime.JobSupport;


namespace SunFarm.CustomerApp
{
    [ActivationGroup("*DFTACTGRP")]
    [ProgramEntry("_ENTRY")]
    public partial class CUSTSBMJOB : Program
    {
        protected dynamic DynamicCaller_;

        //********************************************************************
        // This program updates multiple customers without sending a msg.    *
        //********************************************************************
        // JB   8/30/2004   Created.
        //********************************************************************
        DataStructure _DS4 = new (9);
        FixedDecimal<_9, _0> wkNumber9 { get => _DS4.GetZoned(0, 9, 0); set => _DS4.SetZoned(value, 0, 9, 0); } 
        FixedString<_9> wkAlpha9 { get => _DS4.GetString(0, 9); set => _DS4.SetString(((string)value).AsSpan(), 0, 9); } 

        FixedDecimalArray<_20, _9, _0> pNumbers;
        FixedStringArray<_20, _1> pTypes;
        //********************************************************************

        // Fields defined in main C-Specs declared now as Global fields (Monarch generated)
        FixedDecimal<_15, _5> pCmdLen;
        FixedString<_80> pString;
        FixedDecimal<_3, _0> X;

        void StarEntry(int cparms)
        {
            Indicator _LR = '0';
            do
            {
                pCmdLen = 80;
                _INLR = '1';
                //--------------------------------------------------------------------
                for (X = 1; X <= 20; X++)
                {
                    if (pNumbers[(int)(X - 1)] != 0)
                    {
                        wkNumber9 = pNumbers[(int)(X - 1)];
                        if (pTypes[(int)(X - 1)] == "C")
                            pString = "SbmJob Cmd(CALL CUSTCRTS Parm(\'" + wkAlpha9 + "\')) Job(CustCrt) ";
                        else
                            pString = "SbmJob Cmd(CALL CUSTPRTS Parm(\'" + wkAlpha9 + "\')) Job(CustPrint) ";
                        DynamicCaller_.CallD("?.QCMDEXC", out _LR, ref pString, ref pCmdLen);
                    }
                }
            } while (!(bool)_INLR);
        }
        //********************************************************************

#region Entry and activation methods for *ENTRY

        int _parms;

        void __ENTRY(FixedDecimalArray<_20, _9, _0> _pNumbers, FixedStringArray<_20, _1> _pTypes)
        {
            int cparms = 2;
            bool _cleanup = true;
            pTypes = _pTypes;
            pNumbers = _pNumbers;
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
                _ = _cleanup;
            }
        }

        public static void _ENTRY(ICaller _caller, out Indicator __inLR, FixedDecimalArray<_20, _9, _0> pNumbers, FixedStringArray<_20, _1> pTypes)
        {
            __inLR = RunProgram<CUSTSBMJOB>(_caller, (CUSTSBMJOB _instance) => _instance.__ENTRY(pNumbers, pTypes));
        }
#endregion

        public CUSTSBMJOB()
        {
            _instanceInit();
        }

        void _instanceInit()
        {
            pTypes = new FixedStringArray<_20, _1>((FixedString<_1>[])null);
            pNumbers = new FixedDecimalArray<_20, _9, _0>((FixedDecimal<_9, _0>[])null);
            DynamicCaller_ = new DynamicCaller(this);
        }
    }

}
